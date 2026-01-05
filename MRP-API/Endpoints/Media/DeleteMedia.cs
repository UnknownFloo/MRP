namespace MRP.Endpoints.Media
{
    public static class DeleteMedia
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleDeleteMedia(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            if (req.QueryString["mediaId"] == null)
            {
                await ClientError.Handle400(req, resp, "Missing 'mediaId' query parameter.");
                return;
            }

            if (!int.TryParse(req.QueryString["mediaId"], out int mediaId))
            {
                await ClientError.Handle400(req, resp, "'mediaId' must be a valid integer.");
                return;
            }

            try
            {

                string username = GetUsername.ExtractUsername(req.Headers["Authorization"] ?? "");

                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand("DELETE FROM media WHERE id = @mediaId AND created_by = @username", conn);
                cmd.Parameters.AddWithValue("mediaId", mediaId);
                cmd.Parameters.AddWithValue("username", username);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    await Success.Handle200(req, resp, "Media item deleted successfully.");
                }
                else
                {
                    await ClientError.Handle404(req, resp, "Media item not found or you do not have permission to delete it.");
                }
            }
            catch (Exception ex)
            {
                await ServerError.Handle500(req, resp, "An error occurred while processing your request.");
                Console.WriteLine("Error: " + ex.Message);
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}