namespace MRP.Endpoints.Favorite
{
    public class UnFavorite
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleUnFavorite(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            string? mediaIdStr = req.QueryString["mediaId"];
            if (string.IsNullOrEmpty(mediaIdStr) || !int.TryParse(mediaIdStr, out int mediaId))
            {
                await ClientError.Handle400(req, resp, "Invalid or missing mediaId in query parameters.");
                return;
            }

            try
            {
                string username = GetUsername.ExtractUsername(req.Headers["Authorization"] ?? "");

                await conn.OpenAsync();
                
                // Prüfen ob das Media überhaupt in Favorites ist
                await using var checkCmd = new NpgsqlCommand("SELECT @mediaId = ANY(COALESCE(favorites, '{}')) FROM users WHERE username = @username", conn);
                checkCmd.Parameters.AddWithValue("mediaId", mediaId);
                checkCmd.Parameters.AddWithValue("username", username);
                var isFavorited = await checkCmd.ExecuteScalarAsync();

                if (isFavorited == null || !(bool)isFavorited)
                {
                    await ClientError.Handle400(req, resp, "Media is not in favorites.");
                    return;
                }

                // Aus Favorites entfernen
                await using var cmd = new NpgsqlCommand("UPDATE users SET favorites = array_remove(COALESCE(favorites, '{}'), @mediaId) WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("mediaId", mediaId);
                cmd.Parameters.AddWithValue("username", username);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Rows affected: {rowsAffected}");

                if (rowsAffected > 0)
                {
                    await Success.Handle200(req, resp, "Media unfavorited successfully.");
                }
                else
                {
                    await ClientError.Handle404(req, resp, "User not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error details: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                await ServerError.Handle500(req, resp, "An error occurred while unfavoriting the media.");
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}