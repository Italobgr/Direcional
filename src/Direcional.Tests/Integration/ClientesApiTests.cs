using System.Net;
using System.Net.Http.Json;
using Direcional.Api.Dtos;
using Direcional.Api.Infra;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Direcional.Tests.Integration;

public class ClientesApiTests : IClassFixture<CustomWebAppFactory>
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _client;

    public ClientesApiTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_Criar_Listar_Cliente()
    {
        // create
        var create = new { nome = "Maria", email = "maria@ex.com", telefone = "3199" };
        var resp = await _client.PostAsJsonAsync("/api/clientes", create);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        // list
        var list = await _client.GetAsync("/api/clientes");
        list.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
