using System.Linq;
using Direcional.Api.Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Direcional.Tests.Integration;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {


        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "MinhaChaveJWT-Segura-De-Teste-1234567890!",
                ["Jwt:Issuer"] = "Direcional",
                ["Jwt:Audience"] = "DirecionalUsers"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remova TUDO que for do AppDbContext registrado pela API
            var toRemove = services
                .Where(d =>
                       d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                       d.ServiceType == typeof(AppDbContext) ||
                       d.ServiceType == typeof(IDbContextFactory<AppDbContext>))
                .ToList();
            foreach (var d in toRemove) services.Remove(d);

            // Conexão SQLite em memória (compartilhada)
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            // >>> Isola o provedor do EF para evitar conflito com SqlServer <<<
            var efProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();

            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlite(_connection);
                opt.UseInternalServiceProvider(efProvider); // isolamento do provedor
            });

            // Cria o schema
            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Close();
        _connection?.Dispose();
    }
}
