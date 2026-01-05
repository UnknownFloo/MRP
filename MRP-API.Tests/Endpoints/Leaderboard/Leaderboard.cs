
using System.Net.Http.Headers;

namespace MRP_API.Tests.Endpoints.Leaderboard;


public class LeaderboardTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private HttpClient _client = new HttpClient();

    [Fact]
    public async Task HandleLeaderboard_Unauthorized()
    {
       _client.BaseAddress = new Uri(_baseUrl);
       
        var response = await _client.GetAsync("/api/leaderboard");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HandleLeaderboard_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var response = await _client.GetAsync("/api/leaderboard");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}