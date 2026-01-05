namespace MRP.Endpoints.Auth
{
    public static class Login
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleLogin(HttpListenerRequest req, HttpListenerResponse resp)
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

                await using var cmd = new NpgsqlCommand("SELECT username FROM users WHERE username = @username AND password = @password", conn);
                cmd.Parameters.AddWithValue("username", user.username);
                cmd.Parameters.AddWithValue("password", user.password);
                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    string bearer_token = user.username + "-mrpToken";
                    await Success.Handle200(req, resp, $"Login successful. Bearer token: {bearer_token}");
                }
                else
                {
                    await ClientError.Handle403(req, resp, "Login Failed. Invalid username or password.");
                    return;                    
                }


            }catch (Exception ex)
            {
                Console.WriteLine("Error processing login: " + ex.Message);
                await ClientError.Handle400(req, resp, "An error occurred while processing the login request.");
                return;
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}