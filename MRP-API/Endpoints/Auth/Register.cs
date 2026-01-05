using System.IO;

namespace MRP.Endpoints.Auth
{
    public static class Register
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleRegister(HttpListenerRequest req, HttpListenerResponse resp)
        {
            using var body = req.InputStream;
            using var reader = new StreamReader(body, req.ContentEncoding);
            try
            {
                string JsonData = reader.ReadToEnd();

                User? user = JsonSerializer.Deserialize<User>(JsonData);

                if (user == null || string.IsNullOrEmpty(user.username) || string.IsNullOrEmpty(user.password))
                {
                    await ClientError.Handle400(req, resp, "Missing username or password in request body.");
                    return;
                }

                await conn.OpenAsync();

                await using var checkCmd = new NpgsqlCommand("SELECT COUNT(username) FROM users WHERE username = @username", conn);
                checkCmd.Parameters.AddWithValue("username", user.username);
                var userExists = (long)(await checkCmd.ExecuteScalarAsync() ?? 0) > 0;

                if (!userExists)
                {
                    await using var cmd = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@username, @password)", conn);
                    cmd.Parameters.AddWithValue("username", user.username);
                    cmd.Parameters.AddWithValue("password", user.password);
                    await cmd.ExecuteNonQueryAsync();

                    conn.Close();

                    await Success.Handle200(req, resp, "User registered successfully.");                    
                }
                else
                {
                    await ClientError.Handle409(req, resp, "Username already exists.");
                    return;
                }

                
            }catch (Exception ex)
            {
                Console.WriteLine("Error processing registration: " + ex.Message);
                await ServerError.Handle500(req, resp, "Invalid request body format.");
                return;
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}