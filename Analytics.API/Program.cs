using Analytics.Application.Repositories;
using Analytics.Infrastructure.Context;
using Analytics.Infrastructure.Messaging.RabbitMQ.Config;
using Microsoft.EntityFrameworkCore;

// Não precisamos mais do Env.Load(), pois o Docker-Compose já define as variáveis.

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.
builder.Services.AddControllers(); 
builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Configuração do RabbitMQ ---
// Esta linha agora funciona perfeitamente. O .NET vai ler a variável de ambiente
// "RabbitMQ__Host" do docker-compose e vai mapeá-la para a propriedade "HostName" da classe RabbitMQSettings.
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// A injeção da conexão continua igual.
builder.Services.AddSingleton<RabbitMQConnection>();
// --- Fim da Configuração ---


// Adiciona a injeção de dependência para o DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

var app = builder.Build();

// Configura o pipeline de requisição HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization(); 
app.MapControllers(); 

app.Run();
