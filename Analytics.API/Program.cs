using Analytics.API.Services;
using Analytics.Application.Interfaces; // Para IMessageBus
using Analytics.Application.Repositories; // Para IVendaRepository
using Analytics.Infrastructure.Context;
using Analytics.Infrastructure.Messaging; // Para RabbitMqMessageBus
using Analytics.Infrastructure.Messaging.RabbitMQ.Config; // Para RabbitMQSettings e RabbitMQConnection
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client; // Para IConnection e IConnectionFactory
using Microsoft.Extensions.DependencyInjection; // Para GetRequiredService
using Microsoft.Extensions.Configuration; // Para IConfiguration
using Microsoft.Extensions.Options; // Para IOptions

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.
builder.Services.AddControllers();
builder.Services.AddScoped<IEventRepository, EventRepository>();// Registro do Repositório
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Configuração do RabbitMQ ---
// 1. Configurações do RabbitMQ a partir do appsettings.json
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// 2. Registro da IConnectionFactory (necessário para criar conexões)
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value; // Usar IOptions
    return new ConnectionFactory()
    {
        HostName = settings.HostName, // CORRIGIDO: de settings.Host para settings.HostName
        UserName = settings.UserName, // CORRIGIDO: de settings.User para settings.UserName
        Password = settings.Password, // CORRIGIDO: de settings.Password para settings.Password (verificar se o nome da propriedade é 'Password' ou 'Pass')
    };
});

// 3. Registro da classe RabbitMQConnection (que gerencia a IConnection)
// A conexão será estabelecida no construtor de RabbitMQConnection.
builder.Services.AddSingleton<RabbitMQConnection>();
//inicializa as filas
builder.Services.AddHostedService<QueueInitializer>();

// 4. Registro da IConnection real, obtida da instância de RabbitMQConnection.
// Isso garante que a IConnection esteja disponível para outros serviços.
builder.Services.AddSingleton<IConnection>(sp =>
{
    var rabbitMqConnectionManager = sp.GetRequiredService<RabbitMQConnection>();
    // A conexão já foi estabelecida no construtor de RabbitMQConnection.
    // Apenas a expomos aqui.
    return rabbitMqConnectionManager.Connection;
});


// 5. Registro da implementação de IMessageBus
builder.Services.AddSingleton<IMessageBus, RabbitMqMessageBus>();

// 6. Registro do BackgroundService (EventConsumerService)
builder.Services.AddHostedService<EventConsumerService>();
builder.Services.AddScoped<AnalyticsService>(); 

// --- Fim da Configuração RabbitMQ ---


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
