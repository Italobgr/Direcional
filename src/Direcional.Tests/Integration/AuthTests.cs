using System.Net;
using FluentAssertions;
using Xunit;

namespace Direcional.Tests.Integration;

public class AuthTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebAppFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Rotas_Protegidas_Devem_Passar_Com_Auth_Fake()
    {
        var resp = await _client.GetAsync("/api/clientes");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
