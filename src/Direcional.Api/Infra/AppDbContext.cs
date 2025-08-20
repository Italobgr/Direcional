using Direcional.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Direcional.Api.Infra;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Apartamento> Apartamentos => Set<Apartamento>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Venda> Vendas => Set<Venda>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        /* modelBuilder.Entity<Teste>()
     .HasOne(v => v.Teste).WithMany().HasForeignKey(v => v.Testeid);*/



        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);



        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Cpf).IsUnique();

        modelBuilder.Entity<Apartamento>()
            .HasIndex(a => new { a.Bloco, a.Numero }).IsUnique();




        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Cliente).WithMany().HasForeignKey(r => r.ClienteId);
        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Apartamento).WithMany().HasForeignKey(r => r.ApartamentoId);

        modelBuilder.Entity<Venda>()
            .HasOne(v => v.Cliente).WithMany().HasForeignKey(v => v.ClienteId);
        modelBuilder.Entity<Venda>()
            .HasOne(v => v.Apartamento).WithMany().HasForeignKey(v => v.ApartamentoId);

        base.OnModelCreating(modelBuilder);
        
    }
}
