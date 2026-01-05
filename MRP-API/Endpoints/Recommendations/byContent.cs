namespace MRP.Endpoints.Recommendations
{
    public class byContent
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleByContent(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            string? mediaType = req.QueryString["mediaType"];
            if (string.IsNullOrEmpty(mediaType))
            {
                await ClientError.Handle400(req, resp, "Missing mediaType in query parameters.");
                return;
            }

            try
            {
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand("SELECT m.*, AVG(r.stars) as average_rating FROM media m INNER JOIN ratings r ON m.id = r.media_id WHERE m.media_type = @mediaType GROUP BY m.id ORDER BY AVG(r.stars) DESC", conn);
                cmd.Parameters.AddWithValue("mediaType", mediaType);
                var reader = await cmd.ExecuteReaderAsync();
                var recommendations = new List<Dictionary<string, object>>();
                while (await reader.ReadAsync())
                {
                    var mediaItem = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        mediaItem[reader.GetName(i)] = reader.GetValue(i);
                    }
                    recommendations.Add(mediaItem);
                }
                
                await Success.Handle200(req, resp, JsonSerializer.Serialize(recommendations));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await ServerError.Handle500(req, resp, "An error occurred while fetching recommendations.");
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}