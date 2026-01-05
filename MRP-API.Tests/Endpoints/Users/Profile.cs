using System.Net.Http.Headers;
using Xunit;

namespace MRP_API.Tests.Endpoints.Users;

public class ProfileTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private readonly HttpClient _client = new HttpClient();

    [Fact]
    public async Task HandleProfile_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var response = await _client.GetAsync("/api/users?username=admin");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HandleProfile_Unauthorized()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var response = await _client.GetAsync("/api/users?username=admin");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}