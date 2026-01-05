namespace MRP.Codes
{
    public static class Success
    {
        public static async Task Handle200(HttpListenerRequest req, HttpListenerResponse resp, string Message = "OK")
        {
            resp.StatusCode = 200;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"{Message}");
            resp.ContentType = "text/plain";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }   
    }
}