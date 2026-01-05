namespace MRP.Endpoints.Rating
{
    public class RateMedia
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task handleRating(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            using var body = req.InputStream;
            using var reader = new StreamReader(body, req.ContentEncoding);
            string JsonData = reader.ReadToEnd();
            MRP.Models.Rating? rating = JsonSerializer.Deserialize<MRP.Models.Rating>(JsonData);
            try
            {
                if (rating == null || rating.stars == 0 || req.QueryString["mediaId"] == null)
                {
                    await ClientError.Handle400(req, resp, "Missing mediaId or stars in request body.");
                    return;
                }

                int mediaId = int.Parse(req.QueryString["mediaId"] ?? "0");
                string comment = rating.comment ?? ""; 

                string username = GetUsername.ExtractUsername(req.Headers["Authorization"] ?? "");

                await conn.OpenAsync();

                await using var checkCMD = new NpgsqlCommand("SELECT COUNT(*) FROM media WHERE id = @mediaId", conn);
                checkCMD.Parameters.AddWithValue("mediaId", mediaId);
                var result = await checkCMD.ExecuteScalarAsync();
                var exists = result != null ? (long)result : 0L;
                if (exists == 0)
                {
                    await ClientError.Handle404(req, resp, "The specified media does not exist.");
                    return;
                }

                await using var checkExistingRatingCMD = new NpgsqlCommand("SELECT COUNT(*) FROM ratings WHERE media_id = @mediaId AND rating_by = @username", conn);
                checkExistingRatingCMD.Parameters.AddWithValue("mediaId", mediaId);
                checkExistingRatingCMD.Parameters.AddWithValue("username", username);
                var existingRatingResult = await checkExistingRatingCMD.ExecuteScalarAsync();
                var existingRatingCount = existingRatingResult != null ? (long)existingRatingResult : 0L;
                if (existingRatingCount > 0)
                {
                    await ClientError.Handle400(req, resp, "You have already rated this media.");
                    return;
                }

                await using var cmd = new NpgsqlCommand("INSERT INTO ratings (media_id, rating_by, stars, comment) VALUES (@mediaId, @username, @rating, @comment)", conn);
                cmd.Parameters.AddWithValue("mediaId", mediaId);
                cmd.Parameters.AddWithValue("rating", rating.stars);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("comment", comment);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    await Success.Handle200(req, resp, "Rating submitted successfully.");
                }
                else
                {
                    await ServerError.Handle500(req, resp, "Failed to submit rating.");
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