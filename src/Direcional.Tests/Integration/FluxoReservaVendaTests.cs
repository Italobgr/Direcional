using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Direcional.Tests.Integration;

public class FluxoReservaVendaTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;

    public FluxoReservaVendaTests(CustomWebAppFactory factory)
    {
        _client = factory.CreateClient(new() { BaseAddress = new Uri("http://localhost") });
    }

    [Fact]
    public async Task Reservar_E_Vender_Apartamento_Deve_Atualizar_Status()
    {
        var token = await TestHelpers.GetJwtAsync(_client);
        //var test = TestHelpers.GetJwtAsync(_client);

        /* FluentActions.Invoking(() => _client.WithBearer(token))
            .Should().NotThrow(); */


        _client.WithBearer(token);

        // cria cliente
        var cResp = await _client.PostAsJsonAsync("/api/Clientes", new { nome = "João", cpf = "98765432100", email = "joao@ex.com" });
        var cObj = await cResp.Content.ReadFromJsonAsync<dynamic>();
        int clienteId = (int)cObj!.id;

        // cria apartamento (status default = Disponivel)
        var aResp = await _client.PostAsJsonAsync("/api/Apartamentos", new { bloco = "A", numero = "101", preco = 300000.00 });
        var aObj = await aResp.Content.ReadFromJsonAsync<dynamic>();
        int apId = (int)aObj!.id;

        // reservar
        var rResp = await _client.PostAsJsonAsync("/api/Reservas", new { clienteId, apartamentoId = apId });
        rResp.StatusCode.Should().Be(HttpStatusCode.Created);

        // checa status do ap -> Reservado
        var apDepoisReserva = await _client.GetFromJsonAsync<dynamic>($"/api/Apartamentos/{apId}");
        string status1 = (string)apDepoisReserva!.status.ToString(); // depende do seu retorno
        status1.Should().Contain("Reservado");

        // vender (converter reserva) — ajuste o endpoint conforme sua implementação
        var vResp = await _client.PostAsJsonAsync("/api/Vendas", new { clienteId, apartamentoId = apId, valorEntrada = 50000.00 });
        vResp.StatusCode.Should().Be(HttpStatusCode.Created);

        // checa status do ap -> Vendido
        var apDepoisVenda = await _client.GetFromJsonAsync<dynamic>($"/api/Apartamentos/{apId}");
        string status2 = (string)apDepoisVenda!.status.ToString();
        status2.Should().Contain("Vendido");
    }

    [Fact]
    public async Task Nao_Deve_Vender_Sem_Reserva_Ativa()
    {
        var token = await TestHelpers.GetJwtAsync(_client);
        _client.WithBearer(token);

        // cria cliente e apartamento
        var c = await (await _client.PostAsJsonAsync("/api/Clientes", new { nome = "Ana", cpf = "11122233344", email = "ana@ex.com" }))
            .Content.ReadFromJsonAsync<dynamic>();
        var a = await (await _client.PostAsJsonAsync("/api/Apartamentos", new { bloco = "B", numero = "202", preco = 250000.00 }))
            .Content.ReadFromJsonAsync<dynamic>();

        int clienteId = (int)c!.id;
        int apId = (int)a!.id;

        // tentar vender direto ---- mapear o status code d postman  tkx2
        var vResp = await _client.PostAsJsonAsync("/api/Vendas", new { clienteId, apartamentoId = apId, valorEntrada = 30000.00 });
        vResp.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Conflict);
    }
}
