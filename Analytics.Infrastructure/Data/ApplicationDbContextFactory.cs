using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using DotNetEnv; // Adicione esta diretiva

namespace Analytics.Infrastructure.Context
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Carrega as variáveis do .env no tempo de design
            // O caminho pode precisar de ajuste dependendo de onde o .env está
            Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "Analytics.API", ".env"));
            // Se o comando for executado de dentro da pasta da API, pode ser:
            // Env.Load();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Analytics.API"))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("A string de conexão 'DefaultConnection' não foi encontrada ou está vazia. " +
                    "Verifique se o seu .env e appsettings.json estão configurados corretamente.");
            }

            optionsBuilder.UseNpgsql(connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
