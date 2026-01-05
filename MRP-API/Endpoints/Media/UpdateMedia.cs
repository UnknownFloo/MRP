namespace MRP.Endpoints.Media
{
    public static class UpdateMedia
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleUpdateMedia(HttpListenerRequest req, HttpListenerResponse resp)
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


            int mediaId = int.Parse(req.QueryString["mediaId"]!);
            string username = GetUsername.ExtractUsername(req.Headers["Authorization"] ?? "");

            using var body = req.InputStream;
            using var reader = new StreamReader(body, req.ContentEncoding);
            try
            {   
                
                await conn.OpenAsync();
                using var checkMedia = new NpgsqlCommand("SELECT COUNT(*) FROM media WHERE id = @mediaId", conn);
                checkMedia.Parameters.AddWithValue("mediaId", mediaId);
                int count = Convert.ToInt32(await checkMedia.ExecuteScalarAsync());
                if (count == 0)
                {
                    await ClientError.Handle404(req, resp, "You are not authorized to update this media or it does not exist.");
                    return;
                }

                using var checkOwner = new NpgsqlCommand("SELECT COUNT(*) FROM media WHERE id = @mediaId AND created_by = @created_by", conn);
                checkOwner.Parameters.AddWithValue("mediaId", mediaId);
                checkOwner.Parameters.AddWithValue("created_by", username);
                int ownerCount = Convert.ToInt32(await checkOwner.ExecuteScalarAsync());
                if (ownerCount == 0)
                {
                    await ClientError.Handle403(req, resp, "You are not authorized to update this media.");
                    return;
                }


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

                await using var cmd = new NpgsqlCommand(
                    "UPDATE media SET title = @title, description = @description, media_type = @mediaType, release_year = @releaseYear, genre = @genre::jsonb, age_restriction = @ageRestriction WHERE id = @mediaId AND created_by = @created_by", 
                    conn);
                    
                cmd.Parameters.AddWithValue("title", media.title);
                cmd.Parameters.AddWithValue("description", media.description);
                cmd.Parameters.AddWithValue("mediaType", media.media_type);
                cmd.Parameters.AddWithValue("releaseYear", media.release_year);
                cmd.Parameters.AddWithValue("genre", genresJson);
                cmd.Parameters.AddWithValue("ageRestriction", media.age_restriction);
                cmd.Parameters.AddWithValue("created_by", username);
                cmd.Parameters.AddWithValue("mediaId", mediaId);
                await cmd.ExecuteNonQueryAsync();

                await Success.Handle200(req, resp, "Media updated successfully.");
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