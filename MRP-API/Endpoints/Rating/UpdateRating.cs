namespace MRP.Endpoints.Rating
{
    public class UpdateRating
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task handleUpdateRating(HttpListenerRequest req, HttpListenerResponse resp)
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
                if (rating == null || rating.stars == 0 || req.QueryString["ratingId"] == null)
                {
                    await ClientError.Handle400(req, resp, "Missing ratingId or stars in request.");
                    return;
                }

                int ratingId = int.Parse(req.QueryString["ratingId"] ?? "0");
                string comment = rating.comment ?? "";

                string username = GetUsername.ExtractUsername(req.Headers["Authorization"] ?? "");

                await conn.OpenAsync();

                await using var checkCMD = new NpgsqlCommand("SELECT rating_by FROM ratings WHERE id = @ratingId", conn);
                checkCMD.Parameters.AddWithValue("ratingId", ratingId);
                var result = await checkCMD.ExecuteScalarAsync();
                if (result == null)
                {
                    await ClientError.Handle404(req, resp, "The specified rating does not exist.");
                    return;
                }

                string ratingBy = result.ToString() ?? "";
                if (ratingBy != username)
                {
                    await ClientError.Handle403(req, resp, "You are not authorized to update this rating.");
                    return;
                }

                await using var cmd = new NpgsqlCommand("UPDATE ratings SET stars = @rating, comment = @comment, comment_confirm = false WHERE id = @ratingId", conn);
                cmd.Parameters.AddWithValue("ratingId", ratingId);
                cmd.Parameters.AddWithValue("rating", rating.stars);
                cmd.Parameters.AddWithValue("comment", comment);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    await Success.Handle200(req, resp, "Rating updated successfully.");
                }
                else
                {
                    await ServerError.Handle500(req, resp, "Failed to update rating.");
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