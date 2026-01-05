using MRP.Utils;

namespace MRP_API.Tests.Utils;

public class TokenValidationTests
{
    [Fact]
    public async Task IsTokenValid()
    {
        string Token = "Bearer admin-mrpToken";
        Assert.True(await TokenValidation.IsTokenValid(Token));
    }

    [Fact]
    public async Task IsTokenValid_InvalidToken()
    {
        string Token = "InvalidTokenFormat";
        Assert.False(await TokenValidation.IsTokenValid(Token));
    }

    [Fact]
    public async Task IsTokenInvalid()
    {
        string Token = "Bearer UserDoesntExist-mrpToken";
        Assert.False(await TokenValidation.IsTokenValid(Token));
    }

    [Fact]
    public async Task IsTokenValid_EmptyToken()
    {
        string Token = "";
        Assert.False(await TokenValidation.IsTokenValid(Token));
    }
}