namespace MRP.Endpoints.Users
{
    public class Favorites
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleGetFavorites(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            string username = GetUsername.ExtractUsername(req.Headers["Authorization"] ?? "");

            try
            {
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand(@"
                SELECT 
                    m.id,
                    m.title,
                    m.description,
                    m.media_type,
                    m.genre,
                    m.release_year,
                    m.age_restriction
                FROM users u
                INNER JOIN media m ON m.id = ANY(u.favorites)
                WHERE u.username = @username
                GROUP BY m.id, m.title, m.description, m.media_type, m.genre, m.release_year, m.age_restriction
                ORDER BY m.title
                ", conn);
                cmd.Parameters.AddWithValue("username", username);

                await using var reader = await cmd.ExecuteReaderAsync();

                List<MRP.Models.Media> favoritesList = new List<MRP.Models.Media>();

                while (await reader.ReadAsync())
                {
                    var media = new MRP.Models.Media
                    {
                        id = reader.GetInt32(0),
                        title = reader.GetString(1),
                        description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        media_type = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        genre = reader.IsDBNull(4) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(reader.GetString(4)) ?? new List<string>(),
                        release_year = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                        age_restriction = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
                    };
                    favoritesList.Add(media);
                }

                await Success.Handle200(req, resp, JsonSerializer.Serialize(favoritesList));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await ServerError.Handle500(req, resp, "An error occurred while retrieving rating history.");
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}