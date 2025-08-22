using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Direcional.Tests.Integration;

public class AuthTests : IClassFixture<CustomWebAppFactory>
{

    // puxando o CustomWebAppFactory (tkx1) 
    private readonly HttpClient _client;
    public AuthTests(CustomWebAppFactory factory)
        => _client = factory.CreateClient(new() { BaseAddress = new Uri("http://localhost") });



    [Fact]
    public async Task Login_Deve_Retornar_Token()
    {
        var resp = await _client.PostAsJsonAsync("/api/Auth/login", new { username = "corretor", password = "123" });
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadFromJsonAsync<Dictionary<string,string>>();
        body.Should().NotBeNull();
        body!["token"].Should().NotBeNullOrWhiteSpace();
    }
}
