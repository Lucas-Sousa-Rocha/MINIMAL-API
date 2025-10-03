using Microsoft.EntityFrameworkCore;
using MINIMAL_API.Dominio.Entidades;

namespace MINIMAL_API.Infraestrutura.Db
{
    public class DbContexto : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbContexto(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }
        }

        public DbSet<Administrador> Administradores { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura DataCriacao para ser gerado automaticamente pelo banco
            modelBuilder.Entity<Administrador>()
                .Property(a => a.DataCriacao)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    Id = 1,
                    Nome = "Admin",
                    Email = "admin@mail.com",
                    Senha = "123456",
                    Perfil = "Admin",
                }
                );
        }
    }
}
