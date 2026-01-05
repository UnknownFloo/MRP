using MRP.Endpoints.Auth;
using MRP.Endpoints.Media;
using MRP.Endpoints.Users;
using MRP.Endpoints.Rating;
using MRP.Endpoints.Leaderboard;
using MRP.Endpoints.Favorite;
using MRP.Endpoints.Recommendations;

namespace MRP
{
    class MainClass
    {
        public static HttpListener listener = new HttpListener();

        public static string url = "http://localhost:8000/";

        public delegate Task RouteHandler(HttpListenerRequest req, HttpListenerResponse resp);

        private static readonly Dictionary<(string method, string path), RouteHandler> routes = new()
        {
            { ("GET", "/debug"), Debug }, // Placeholder

            { ("POST", "/api/auth/register"), Register.HandleRegister },
            { ("POST", "/api/auth/login"), Login.HandleLogin },

            { ("GET", "/api/users"), Profile.HandleProfile },
            { ("GET", "/api/users/ratings"), RatingHistory.HandleGetRatingHistory },
            { ("GET", "/api/users/favorites"), Favorites.HandleGetFavorites }, 
            { ("PUT", "/api/users/profile"), UpdateProfile.HandleUpdateProfile },

            { ("POST", "/api/media"), CreateMedia.HandleCreateMedia },
            { ("DELETE", "/api/media"), DeleteMedia.HandleDeleteMedia },
            { ("GET", "/api/media"), SearchMedia.HandleSearchMedia },
            { ("PUT", "/api/media"), UpdateMedia.HandleUpdateMedia },
            { ("GET", "/api/media/view"), ViewMedia.HandleViewMedia },

            { ("POST", "/api/ratings/media"), RateMedia.handleRating },
            { ("POST", "/api/ratings/like"), LikeRating.handleLike },
            { ("PUT", "/api/ratings/media"), UpdateRating.handleUpdateRating },
            { ("POST", "/api/ratings/confirm"), ConfirmComment.handleConfirmComment },
            { ("GET", "/api/leaderboard"), getLeaderboard.HandleGetLeaderboard },

            { ("POST", "/api/favorite"), FavoriteMedia.HandleFavoriteMedia },
            { ("DELETE", "/api/favorite"), UnFavorite.HandleUnFavorite },

            { ("GET", "/api/recommendations/genre"), byGenre.HandleByGenre },
            { ("GET", "/api/recommendations/content"), byContent.HandleByContent }
        };

        public static async Task Debug(HttpListenerRequest req, HttpListenerResponse resp)
        {
            await Success.Handle200(req, resp, "Debug endpoint is working!");
        }

        public static async Task HandleIncomingConnections()
        {

            while (true)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                if (req.Url?.AbsolutePath == "/favicon.ico") continue;
                Console.WriteLine("Request: {0}", req.Url?.AbsolutePath);
  
                if (routes.TryGetValue((req.HttpMethod, req.Url?.AbsolutePath ?? ""), out RouteHandler? handler))
                {
                    await handler(req, resp);
                }else
                {
                    await ClientError.Handle404(req, resp, "The requested resource was not found.");
                }
                                
                resp.Close();
            }
        }

        public static void Main(string[] args)
        {
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);
            
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();
            listener.Close();
        }
    }
}