namespace MRP.Utils
{
    public static class GetUsername
    {
        private static readonly string TokenSuffix = "-mrpToken";
        public static string ExtractUsername(string token)
        {
            if (string.IsNullOrEmpty(token) || !token.EndsWith(TokenSuffix))
                throw new ArgumentException("Invalid token format.");

            token = token.Substring("Bearer ".Length);
            return token[..^TokenSuffix.Length];
        }
    }
}