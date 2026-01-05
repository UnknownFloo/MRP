

namespace MRP.Endpoints.Favorite
{
    public class FavoriteMedia
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleFavoriteMedia(HttpListenerRequest req, HttpListenerResponse resp)
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
                await conn.OpenAsync();

                await using var checkForMedia = new NpgsqlCommand("SELECT COUNT(*) FROM media WHERE id = @mediaId", conn);
                checkForMedia.Parameters.AddWithValue("mediaId", mediaId);
                int mediaCount = Convert.ToInt32(await checkForMedia.ExecuteScalarAsync());
                if (mediaCount == 0)
                {
                    await ClientError.Handle404(req, resp, "Media not found.");
                    return;
                }

                string username = GetUsername.ExtractUsername(req.Headers["Authorization"]!);


                // Debug: Aktuelle Favorites anzeigen
                await using var debugCmd = new NpgsqlCommand("SELECT favorites FROM users WHERE username = @username", conn);
                debugCmd.Parameters.AddWithValue("username", username);
                var currentFavorites = await debugCmd.ExecuteScalarAsync();
                Console.WriteLine($"Current favorites for {username}: {currentFavorites}");

                // Prüfen ob bereits favorisiert
                await using var checkCmd = new NpgsqlCommand("SELECT @mediaId = ANY(COALESCE(favorites, '{}')) FROM users WHERE username = @username", conn);
                checkCmd.Parameters.AddWithValue("mediaId", mediaId);
                checkCmd.Parameters.AddWithValue("username", username);
                var alreadyFavorited = await checkCmd.ExecuteScalarAsync();

                if (alreadyFavorited != null && (bool)alreadyFavorited)
                {
                    await ClientError.Handle400(req, resp, "Media is already in favorites.");
                conn.Close();
                    return;
                }

                // Update ausführen
                await using var cmd = new NpgsqlCommand("UPDATE users SET favorites = array_append(COALESCE(favorites, '{}'), @mediaId) WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("mediaId", mediaId);
                cmd.Parameters.AddWithValue("username", username);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Rows affected: {rowsAffected}");

                if (rowsAffected > 0)
                {
                    // Debug: Neue Favorites anzeigen
                    var newFavorites = await debugCmd.ExecuteScalarAsync();
                    Console.WriteLine($"New favorites for {username}: {newFavorites}");
                    
                    await Success.Handle200(req, resp, "Media favorited successfully.");
                }
                else
                {
                    await ClientError.Handle404(req, resp, "User not found.");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error details: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                await ServerError.Handle500(req, resp, "An error occurred while favoriting the media.");
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}