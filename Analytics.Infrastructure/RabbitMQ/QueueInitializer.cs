// Analytics.Infrastructure/Messaging/QueueInitializer.cs
using Analytics.Application.Constants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Analytics.Infrastructure.Messaging
{
    public class QueueInitializer : IHostedService
    {
        private readonly IConnection _connection;
        private readonly ILogger<QueueInitializer> _logger;

        public QueueInitializer(IConnection connection, ILogger<QueueInitializer> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Inicializando filas...");

            await using var channel = await _connection.CreateChannelAsync();

            // Cria a fila de vendas normal
            await channel.QueueDeclareAsync(
                queue: QueueNames.VendasNormal,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            _logger.LogInformation("✅ Fila '{Queue}' criada/verificada com sucesso.", QueueNames.VendasNormal);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
