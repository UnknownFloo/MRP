namespace MRP.Endpoints.Rating
{
    public class LikeRating
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task handleLike(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            if(!int.TryParse(req.QueryString["ratingId"], out int ratingId))
            {
                await ClientError.Handle400(req, resp, "Missing ratingId in query parameters.");
                return;
            }
            try
            {
                string username = GetUsername.ExtractUsername(req.Headers["Authorization"] ?? "");

                await conn.OpenAsync();

                await using var checkRating = new NpgsqlCommand("SELECT COUNT(*) FROM ratings WHERE id = @ratingId", conn);
                checkRating.Parameters.AddWithValue("ratingId", ratingId);
                int ratingCount = Convert.ToInt32(await checkRating.ExecuteScalarAsync());
                if (ratingCount == 0)
                {
                    await ClientError.Handle404(req, resp, "Rating not found.");
                    return;
                }

                await using var checkCMD = new NpgsqlCommand("SELECT liked_by FROM ratings WHERE id = @ratingId AND  @username = ANY(liked_by) ", conn); 
                checkCMD.Parameters.AddWithValue("ratingId", ratingId);
                checkCMD.Parameters.AddWithValue("username", username);
                var result = await checkCMD.ExecuteScalarAsync();
                if (result != null)
                {
                    await using var unlikecmd = new NpgsqlCommand("UPDATE ratings SET liked_by = array_remove(liked_by, @username) WHERE id = @ratingId", conn);
                    unlikecmd.Parameters.AddWithValue("ratingId", ratingId);
                    unlikecmd.Parameters.AddWithValue("username", username);
                    int rowsAffected = await unlikecmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        await Success.Handle200(req, resp, "Rating unliked successfully.");
                    }
                    else
                    {
                        await ServerError.Handle500(req, resp, "Failed to unlike rating.");
                    }
                }
                else
                {   
                    await using var likecmd = new NpgsqlCommand("UPDATE ratings SET liked_by = array_append(liked_by, @username) WHERE id = @ratingId", conn);
                    likecmd.Parameters.AddWithValue("ratingId", ratingId);
                    likecmd.Parameters.AddWithValue("username", username);

                    int rowsAffected = await likecmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        await Success.Handle200(req, resp, "Rating liked successfully.");
                    }
                    else
                    {
                        await ServerError.Handle500(req, resp, "Failed to like rating.");
                    }
                }

                
            }
            catch (Exception ex)
            {
                await ServerError.Handle500(req, resp, $"An error occurred: {ex.Message}");
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}