namespace MRP.Endpoints.Media
{
    public static class ViewMedia
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleViewMedia(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            if (!int.TryParse(req.QueryString["mediaId"], out int mediaId))
            {
                await ClientError.Handle400(req, resp, "Missing 'mediaId' query parameter.");
                return;
            }

            try
            {
                await conn.OpenAsync();

                await using var checkMedia = new NpgsqlCommand("SELECT COUNT(*) FROM media WHERE id = @mediaId", conn);
                checkMedia.Parameters.AddWithValue("mediaId", mediaId);
                int count = Convert.ToInt32(await checkMedia.ExecuteScalarAsync());
                if (count == 0)
                {
                    await ClientError.Handle404(req, resp, "Media not found.");
                    return;
                }

                await using var cmd = new NpgsqlCommand("SELECT * FROM media WHERE id = @mediaId", conn);
                cmd.Parameters.AddWithValue("mediaId", mediaId);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var media = new MRP.Models.Media
                    {
                        id = reader.GetInt32(0),
                        title = reader.GetString(1),
                        description = reader.GetString(2),
                        media_type = reader.GetString(3),
                        release_year = reader.GetInt32(4),
                        genre = JsonSerializer.Deserialize<List<string>>(reader.GetString(5)) ?? new List<string>(),
                        age_restriction = reader.GetInt32(6),
                        created_by = reader.GetString(7)
                    };

                    string jsonResponse = JsonSerializer.Serialize(media);
                    await Success.Handle200(req, resp, jsonResponse);
                    return;
                }
            }
            catch (Exception ex)
            {
                await ServerError.Handle500(req, resp, "An error occurred while processing your request.");
                Console.WriteLine("Error: " + ex.Message);
                return;
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
            await ClientError.Handle404(req, resp, "Media not found.");
        }
    }
}