using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Xunit;

namespace MRP_API.Tests.Endpoints.Rating;

public class UpdateRatingTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private readonly HttpClient _client = new HttpClient();
    private readonly int _ratingId = 1;

    [Fact]
    public async Task HandleUpdateRating_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var updateData = new
        {
            stars = 3,
            comment = "Updated: Still a good movie but not as great as I first thought"
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/ratings/media?ratingID={_ratingId}", content);

        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task HandleUpdateRating_Unauthorized()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var updateData = new
        {
            stars = 3.5,
            comment = "Updated comment"
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/ratings/media?ratingID={_ratingId}", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HandleUpdateRating_RatingNotFound()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var updateData = new
        {
            stars = 3,
            comment = "This rating doesn't exist"
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/ratings/media?ratingID=9999", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task HandleUpdateRating_MissingRatingID()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var updateData = new
        {
            stars = 4,
            comment = "Updated comment"
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/ratings/media", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

}