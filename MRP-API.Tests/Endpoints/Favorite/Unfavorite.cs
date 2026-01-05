namespace MRP_API.Tests.Endpoints.Favorite;

using System.Net.Http.Headers;

public class UnfavoriteTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private HttpClient _client = new HttpClient();

    private readonly int mediaID = 1; // Example media ID
    
    [Fact]
    public async Task HandleUnfavorite_Unauthorized()
    {
        _client.BaseAddress = new Uri(_baseUrl);
       
        var response = await _client.DeleteAsync($"/api/favorite?mediaID={mediaID}");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task HandleUnfavorite_MissingMediaID()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");
       
        var response = await _client.DeleteAsync($"/api/favorite");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HandleUnfavorite_MediaNotFoundInFavorites ()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");
       
        var response = await _client.DeleteAsync($"/api/favorite?mediaID=9999");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HandleUnfavorite_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var response = await _client.DeleteAsync($"/api/favorite?mediaID={mediaID}");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    // Test methods for Favorite endpoint would go here
}