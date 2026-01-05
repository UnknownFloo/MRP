namespace MRP.Codes
{
    public static class ClientError
    {
        public static async Task Handle400(HttpListenerRequest req, HttpListenerResponse resp, string Message = "")
        {
            resp.StatusCode = 400;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"{Message}");
            resp.ContentType = "text/plain";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }
        public static async Task Handle401(HttpListenerRequest req, HttpListenerResponse resp, string Message = "")
        {
            resp.StatusCode = 401;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"{Message}");
            resp.ContentType = "text/plain";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }

        public static async Task Handle403(HttpListenerRequest req, HttpListenerResponse resp, string Message = "")
        {
            resp.StatusCode = 403;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"{Message}");
            resp.ContentType = "text/plain";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }
        public static async Task Handle404(HttpListenerRequest req, HttpListenerResponse resp, string Message = "")
        {
            resp.StatusCode = 404;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"{Message}");
            resp.ContentType = "text/plain";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }

        public static async Task Handle409(HttpListenerRequest req, HttpListenerResponse resp, string Message = "")
        {
            resp.StatusCode = 409;
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"{Message}");
            resp.ContentType = "text/plain";
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }        

        
    }
}   