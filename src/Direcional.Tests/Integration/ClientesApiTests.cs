using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Direcional.Tests.Integration;
public class ClientesApiTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;
    public ClientesApiTests(CustomWebAppFactory factory)
        => _client = factory.CreateClient(new() { BaseAddress = new Uri("http://localhost") });

    [Fact]
    public async Task CRUD_Clientes_Deve_Funcionar()
    {
        //usando o helper
        var token = await TestHelpers.GetJwtAsync(_client);
        _client.WithBearer(token);






        //CRUD de Clientes

        // Create
        var create = new { nome = "Maria", cpf = "12345678901", email = "maria@ex.com" };
        var r1 = await _client.PostAsJsonAsync("/api/Clientes", create);
        r1.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await r1.Content.ReadFromJsonAsync<dynamic>();
        int id = (int)created!.id;



        var list = await _client.GetFromJsonAsync<List<dynamic>>("/api/Clientes");
        list.Should().NotBeNull();
        list!.Any().Should().BeTrue();

        //  Update
        var update = new { nome = "Maria Silva", email = "maria.silva@ex.com" };
        var r2 = await _client.PutAsJsonAsync($"/api/Clientes/{id}", update);
        r2.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Delete
        var r3 = await _client.DeleteAsync($"/api/Clientes/{id}");
        r3.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
