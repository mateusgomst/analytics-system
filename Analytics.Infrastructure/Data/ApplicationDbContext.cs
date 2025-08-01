using Analytics.Domain.Entities; // Adicione esta referência para acessar as entidades do domínio
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        // Construtor necessário para o Entity Framework Core
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Defina os DbSet para as suas entidades do domínio
        public DbSet<Venda> Vendas { get; set; }
        // public DbSet<Cliente> Clientes { get; set; }
        // etc.

        // Opcional: Sobrescrever o método OnModelCreating para configurar o modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Exemplo de configuração de entidade, se necessário
            modelBuilder.Entity<Venda>()
                .Property(v => v.Valor)
                .HasColumnType("decimal(18,2)");
        }
    }
}
