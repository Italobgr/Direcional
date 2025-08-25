using System.Net;
using System.Net.Http.Json;
using Direcional.Api.Domain;
using Direcional.Api.Infra;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Direcional.Tests.Integration;

public class FluxoReservaVendaTests : IClassFixture<CustomWebAppFactory>
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _client;

    public FluxoReservaVendaTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Reserva_Ativa_Confirma_Vira_Venda()
    {
        int clienteId, aptoId, reservaId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            (clienteId, aptoId) = TestHelpers.SeedClienteEApartamento(db);
        }

        // cria reserva
        var reservaDto = new
        {
            idCliente = clienteId,
            idApartamento = aptoId,
            dataReserva = DateTime.UtcNow,
            validade = DateTime.UtcNow.AddDays(7)
        };

        var resp = await _client.PostAsJsonAsync("/api/reservas", reservaDto);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        // pega id da reserva criada no banco
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            reservaId = db.Reservas.Single(r => r.IdApartamento == aptoId).Id;
        }

        // confirmar reserva -> cria venda
        var confirm = await _client.PostAsJsonAsync($"/api/reservas/{reservaId}/confirmar", 345000.00m);
        confirm.StatusCode.Should().Be(HttpStatusCode.Created);

        // assert venda existe
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Vendas.Count(v => v.IdApartamento == aptoId).Should().Be(1);
        }
    }

    [Fact]
    public async Task Nova_Reserva_Mesmo_Apto_Deve_Conflict()
    {
        int clienteId, aptoId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            (clienteId, aptoId) = TestHelpers.SeedClienteEApartamento(db);

            // reserva pré-existente
            db.Reservas.Add(new Reserva
            {
                IdCliente = clienteId,
                IdApartamento = aptoId,
                DataReserva = DateTime.UtcNow,
                Validade = DateTime.UtcNow.AddDays(3),
                Status = ReservaStatus.Ativa
            });
            db.Apartamentos.Find(aptoId)!.Disponivel = false;
            db.SaveChanges();
        }

        var body = new
        {
            idCliente = clienteId,
            idApartamento = aptoId,
            dataReserva = DateTime.UtcNow,
            validade = DateTime.UtcNow.AddDays(3)
        };
        var resp = await _client.PostAsJsonAsync("/api/reservas", body);
        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
