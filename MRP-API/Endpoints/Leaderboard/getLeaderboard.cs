namespace MRP.Endpoints.Leaderboard
{
    
    public class getLeaderboard
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleGetLeaderboard(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            };

            try
            {   
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand(
                    "SELECT rating_by AS username, COUNT(rating_by) AS rating_count FROM ratings GROUP BY username ORDER BY rating_count DESC", 
                    conn);
                await using var reader = await cmd.ExecuteReaderAsync();
                
                List<object> leaderboard = new List<object>();
                    
                while (await reader.ReadAsync())
                {
                    var user = new 
                    {
                        username = reader.GetString(0),
                        ratings_count = reader.GetInt32(1)
                    };
                    leaderboard.Add(user);
                }

                await Success.Handle200(req, resp, JsonSerializer.Serialize(leaderboard));
            }
            catch (Exception ex)
            {
                await ServerError.Handle500(req, resp, "An error occurred while processing the request.");
                Console.WriteLine(ex.Message);
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}