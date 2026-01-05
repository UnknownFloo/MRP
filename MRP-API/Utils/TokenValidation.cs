

namespace MRP.Utils
{
    public static class TokenValidation
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        private static readonly string TokenSuffix = "-mrpToken";

        public static async Task<bool> IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;
            if (!token.EndsWith(TokenSuffix)) return false;

            token = token.Substring("Bearer ".Length);
            string username = token[..^TokenSuffix.Length];

            try
            {
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("username", username);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
    }
}