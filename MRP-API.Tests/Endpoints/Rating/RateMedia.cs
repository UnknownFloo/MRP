using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Xunit;

namespace MRP_API.Tests.Endpoints.Rating;

public class RateMediaTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private readonly HttpClient _client = new HttpClient();
    private readonly int _mediaId = 1;

    [Fact]
    public async Task HandleRateMedia_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var ratingData = new
        {
            stars = 4,
            comment = "Great movie, really enjoyed it!"
        };

        var json = JsonSerializer.Serialize(ratingData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/ratings/media?mediaID={_mediaId}", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HandleRateMedia_Unauthorized()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var ratingData = new
        {
            stars = 4.5,
            comment = "Great movie!"
        };

        var json = JsonSerializer.Serialize(ratingData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/ratings/media?mediaID={_mediaId}", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HandleRateMedia_MissingRating()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var ratingData = new
        {
            comment = "Great movie!"
        };

        var json = JsonSerializer.Serialize(ratingData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/ratings/media?mediaID={_mediaId}", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HandleRateMedia_MediaNotFound()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var ratingData = new
        {
            stars = 4.0,
            comment = "This media doesn't exist"
        };

        var json = JsonSerializer.Serialize(ratingData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/ratings/media?mediaID=9999", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}