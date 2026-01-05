namespace MRP.Endpoints.Users
{
    public class RatingHistory
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleGetRatingHistory(HttpListenerRequest req, HttpListenerResponse resp)
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
                    m.title,
                    r.stars,
                    r.comment,
                    r.liked_by
                FROM ratings r
                INNER JOIN media m ON r.media_id = m.id
                WHERE r.rating_by = @username
                GROUP BY m.title, r.id
                ", conn);
                cmd.Parameters.AddWithValue("username", username);

                await using var reader = await cmd.ExecuteReaderAsync();

                List<object> ratingsList = new List<object>();

                while (await reader.ReadAsync())
                {
                    List<string> likedByList = new List<string>();
                    if (!reader.IsDBNull(3))
                    {
                        var likedByArray = (string[])reader.GetValue(3);
                        likedByList = likedByArray.ToList();
                    }
                    
                    var ratingObj = new 
                    {
                        title = reader.GetString(0),
                        stars = reader.GetInt32(1),
                        comment = reader.IsDBNull(2) ? null : reader.GetString(2),
                        liked_by = likedByList
                    };
                    ratingsList.Add(ratingObj);
                }

                await Success.Handle200(req, resp, JsonSerializer.Serialize(ratingsList));
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