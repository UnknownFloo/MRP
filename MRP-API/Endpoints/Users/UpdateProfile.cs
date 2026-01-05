namespace MRP.Endpoints.Users
{
    public class UpdateProfile
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleUpdateProfile(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            string username = GetUsername.ExtractUsername(req.Headers["Authorization"] ?? "");
            
            using var body = req.InputStream;
            using var reader = new StreamReader(body, req.ContentEncoding);
            string JsonData = reader.ReadToEnd();
            var updateData = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonData);

            if (updateData == null)
            {
                await ClientError.Handle400(req, resp, "Invalid request body format.");
                return;
            }

            if(!updateData.ContainsKey("new_description"))
            {
                await ClientError.Handle400(req, resp, "No fields to update. Provide 'new_description'.");
                return;
            }

            string newDescription = updateData["new_description"]?.ToString() ?? "";

            await conn.OpenAsync();
            try
            {
                await using var cmd = new NpgsqlCommand("UPDATE users SET description = @description WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("description", newDescription);
                cmd.Parameters.AddWithValue("username", username);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    await ClientError.Handle400(req, resp, "User not found.");
                    return;
                }
                else
                {
                    await Success.Handle200(req, resp, "User profile updated successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating profile: " + ex.Message);
                await ServerError.Handle500(req, resp, "Internal server error while updating profile.");
                return;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
     
    }
}