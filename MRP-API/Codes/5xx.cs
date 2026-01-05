namespace MRP.Codes
{
    public static class ServerError
    {
        public static async Task Handle500(HttpListenerRequest req, HttpListenerResponse resp, string Message)
        {
             resp.StatusCode = 500;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"{Message}");
            resp.ContentType = "text/plain";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }
    }
}