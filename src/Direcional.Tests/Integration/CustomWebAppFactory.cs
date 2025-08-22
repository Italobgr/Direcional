using System.Linq;
using Direcional.Api.Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Direcional.Tests.Integration;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{

    private SqliteConnection? _connection;

    //troca para SQLite in-memory para testes
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // removendo o AppDbContext 
            var testedbDesc = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (testedbDesc != null) services.Remove(testedbDesc);

            
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlite(_connection);
            });





            // cria o banco em memeria - tkx
            using var scope = services.BuildServiceProvider().CreateScope();
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
