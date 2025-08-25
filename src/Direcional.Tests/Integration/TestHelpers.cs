using Direcional.Api.Domain;
using Direcional.Api.Infra;

namespace Direcional.Tests.Integration;

public static class TestHelpers
{
    public static (int clienteId, int aptoId) SeedClienteEApartamento(AppDbContext db,
        string nome = "Cliente Teste", string endereco = "Rua X, 123", int quartos = 2, decimal valor = 350000m, bool disponivel = true)
    {
        var c = new Cliente { Nome = nome, Email = "x@x.com", Telefone = "31999999999" };
        var a = new Apartamento { Endereco = endereco, NumeroQuartos = quartos, Valor = valor, Disponivel = disponivel };

        db.Clientes.Add(c);
        db.Apartamentos.Add(a);
        db.SaveChanges();

        return (c.Id, a.Id);
    }
}
