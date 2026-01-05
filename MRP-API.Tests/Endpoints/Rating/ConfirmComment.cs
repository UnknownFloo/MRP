using System.Net.Http.Headers;
using Xunit;

namespace MRP_API.Tests.Endpoints.Rating;


public class ConfirmCommentTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private readonly HttpClient _client = new HttpClient();
    private readonly int _ratingId = 1;

    [Fact]
    public async Task HandleConfirmComment_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var response = await _client.PostAsync($"/api/ratings/confirm?ratingID={_ratingId}", null);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HandleConfirmComment_Unauthorized()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var response = await _client.PostAsync($"/api/ratings/confirm?ratingID={_ratingId}", null);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HandleConfirmComment_RatingNotFound()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var response = await _client.PostAsync("/api/ratings/confirm?ratingID=9999", null);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task HandleConfirmComment_MissingRatingID()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var response = await _client.PostAsync("/api/ratings/confirm", null);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HandleConfirmComment_InvalidRatingID()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var response = await _client.PostAsync("/api/ratings/confirm?ratingID=invalid", null);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}