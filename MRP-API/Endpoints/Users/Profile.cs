namespace MRP.Endpoints.Users
{
    public static class Profile
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleProfile(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            if (req.QueryString["username"] == null)
            {
                await ClientError.Handle400(req, resp, "Missing 'username' query parameter.");
                return;
            }
            string username = req.QueryString["username"]!;
            try
            {
                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand(@"
                    SELECT 
                        u.username,
                        u.description, 
                        COALESCE(COUNT(DISTINCT r.id), 0) AS ratings, 
                        COALESCE(COUNT(DISTINCT m.id), 0) AS created_medias 
                    FROM users u
                    LEFT JOIN ratings r ON u.username = r.rating_by
                    LEFT JOIN media m ON u.username = m.created_by
                    WHERE u.username = @username
                    GROUP BY u.username, u.description", conn);
                cmd.Parameters.AddWithValue("username", username);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var userProfile = new 
                    {
                        username = reader.GetString(0),
                        description = reader.GetString(1) ?? "",
                        ratings = reader.GetInt32(2),
                        created_medias = reader.GetInt32(3)
                    };
                    await Success.Handle200(req, resp, JsonSerializer.Serialize(userProfile));
                }
                else
                {
                    await ClientError.Handle404(req, resp, "User not found.");
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