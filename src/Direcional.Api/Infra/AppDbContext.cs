using Direcional.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Direcional.Api.Infra
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Apartamento> Apartamentos => Set<Apartamento>();
        public DbSet<Venda> Vendas => Set<Venda>();
        public DbSet<Reserva> Reservas => Set<Reserva>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(e =>
            {
                e.ToTable("Clientes");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nome).IsRequired().HasMaxLength(150);
                e.Property(x => x.Email).HasMaxLength(150);
                e.Property(x => x.Telefone).HasMaxLength(30);
            });

            modelBuilder.Entity<Apartamento>(e =>
            {
                e.ToTable("Apartamentos");
                e.HasKey(x => x.Id);
                e.Property(x => x.Endereco).IsRequired().HasMaxLength(250);
                e.Property(x => x.NumeroQuartos).IsRequired();
                e.Property(x => x.Valor).HasPrecision(18, 2);
                e.Property(x => x.Disponivel).HasDefaultValue(true);
            });

            modelBuilder.Entity<Venda>(e =>
            {
                e.ToTable("Vendas");
                e.HasKey(x => x.Id);
                e.Property(x => x.DataVenda).IsRequired();
                e.Property(x => x.ValorFinal).HasPrecision(18, 2);

                e.HasOne(v => v.Cliente)
                    .WithMany(c => c.Vendas)
                    .HasForeignKey(v => v.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(v => v.Apartamento)
                    .WithMany(a => a.Vendas)
                    .HasForeignKey(v => v.IdApartamento)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.IdApartamento, x.DataVenda });
            });

            modelBuilder.Entity<Reserva>(e =>
            {
                e.ToTable("Reservas");
                e.HasKey(x => x.Id);
                e.Property(x => x.Status).HasConversion<int>().HasDefaultValue(ReservaStatus.Ativa);

                e.HasOne(r => r.Cliente)
                    .WithMany(c => c.Reservas)
                    .HasForeignKey(r => r.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Apartamento)
                    .WithMany(a => a.Reservas)
                    .HasForeignKey(r => r.IdApartamento)
                    .OnDelete(DeleteBehavior.Restrict);

                // UNIQUE apenas quando Status = Ativa (SQL Server)
                e.HasIndex(x => x.IdApartamento)
                .HasDatabaseName("UX_Reservas_Apto_Ativa")
                .IsUnique()
                .HasFilter("[Status] = 1");
            });
        }
    }
}
