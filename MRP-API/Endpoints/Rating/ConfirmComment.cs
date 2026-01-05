namespace MRP.Endpoints.Rating
{
   public class ConfirmComment
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task handleConfirmComment(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            if(!int.TryParse(req.QueryString["ratingId"], out int ratingId))
            {
                await ClientError.Handle400(req, resp, "Missing or invalid ratingId in query parameters.");
                return;
            }

            try
            {
                string username = GetUsername.ExtractUsername(req.Headers["Authorization"] ?? "");

                await conn.OpenAsync();

                await using var getMedia = new NpgsqlCommand("SELECT media_id FROM ratings WHERE id = @ratingId", conn);
                getMedia.Parameters.AddWithValue("ratingId", ratingId);
                var mediaResult = await getMedia.ExecuteScalarAsync();
                if (mediaResult != null)
                {
                    int mediaId = (int)mediaResult;

                    await using var getMediaOwner = new NpgsqlCommand("SELECT created_by FROM media WHERE id = @mediaId", conn);
                    getMediaOwner.Parameters.AddWithValue("mediaId", mediaId);
                    var ownerResult = await getMediaOwner.ExecuteScalarAsync();
                    if (ownerResult != null)
                    {
                        string mediaOwner = ownerResult.ToString() ?? "";
                        if (mediaOwner == username)
                        {
                            await using var cmd = new NpgsqlCommand("UPDATE ratings SET comment_confirm = true WHERE id = @ratingId", conn);
                            cmd.Parameters.AddWithValue("ratingId", ratingId);

                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            if (rowsAffected > 0)
                            {
                                await Success.Handle200(req, resp, "Comment confirmed successfully.");
                            }
                            else
                            {
                                await ServerError.Handle500(req, resp, "Failed to confirm comment.");
                            }
                            
                        }
                        else
                        {
                            await ClientError.Handle403(req, resp, "You are not authorized to confirm comments for this rating.");
                            return;
                        }
                    }
                    else
                    {
                        await ClientError.Handle400(req, resp, "The associated media does not exist.");
                        return;
                    }
                    
                }
                else
                {
                    await ClientError.Handle404(req, resp, "The specified rating does not exist.");
                    return;
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