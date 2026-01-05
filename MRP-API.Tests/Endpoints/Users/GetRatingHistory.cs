using System.Net.Http.Headers;
using Xunit;

namespace MRP_API.Tests.Endpoints.Users;

public class GetRatingHistoryTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private readonly HttpClient _client = new HttpClient();

    [Fact]
    public async Task HandleGetRatingHistory_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var response = await _client.GetAsync("/api/users/ratings");

        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task HandleGetRatingHistory_Unauthorized()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var response = await _client.GetAsync("/api/users/ratings");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}