namespace MRP.Endpoints.Media
{
    public static class CreateMedia
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleCreateMedia(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            using var body = req.InputStream;
            using var reader = new StreamReader(body, req.ContentEncoding);
            try
            {
                string JsonData = reader.ReadToEnd();
                MRP.Models.Media? media = JsonSerializer.Deserialize<MRP.Models.Media>(JsonData);

                if (media == null || 
                    string.IsNullOrEmpty(media.title) || 
                    string.IsNullOrEmpty(media.description) || 
                    string.IsNullOrEmpty(media.media_type) ||
                    media.release_year == 0 || 
                    media.genre == null || media.genre.Count == 0 ||
                    media.age_restriction == 0
                )
                {
                    await ClientError.Handle400(req, resp, "Missing Parameters in request body.");
                    return;
                }
                
                string genresJson = JsonSerializer.Serialize(media.genre);
                string username = GetUsername.ExtractUsername(req.Headers["Authorization"] ?? "");

                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand(
                    "INSERT INTO media (title, description, media_type, release_year, genre, age_restriction, created_by) VALUES (@title, @description, @mediaType, @releaseYear, @genre::jsonb, @ageRestriction, @created_by)", 
                    conn);
                    
                cmd.Parameters.AddWithValue("title", media.title);
                cmd.Parameters.AddWithValue("description", media.description);
                cmd.Parameters.AddWithValue("mediaType", media.media_type);
                cmd.Parameters.AddWithValue("releaseYear", media.release_year);
                cmd.Parameters.AddWithValue("genre", genresJson);
                cmd.Parameters.AddWithValue("ageRestriction", media.age_restriction);
                cmd.Parameters.AddWithValue("created_by", username);
                
                await cmd.ExecuteNonQueryAsync();

                await Success.Handle200(req, resp, "Media created successfully.");
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