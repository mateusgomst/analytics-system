using Analytics.Application.Constants;
using Analytics.Application.DTOs;
using Analytics.Application.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Analytics.API.Services
{
    public class VendaConsumerService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<VendaConsumerService> _logger;

        public VendaConsumerService(IConnection connection, IServiceProvider serviceProvider, ILogger<VendaConsumerService> logger)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando consumo da fila '{Queue}'...", QueueNames.VendasNormal);

            var channel = await _connection.CreateChannelAsync(); 
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("Mensagem recebida: {Json}", json);

                try
                {
                    var venda = JsonSerializer.Deserialize<VendaDto>(json);
                    if (venda != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<IVendaRepository>();
                        await repo.AdcionarNovaVenda(venda);
                        _logger.LogInformation("Venda salva no banco com sucesso.");
                    }
                    else
                    {
                        _logger.LogWarning("Falha ao desserializar venda: {Json}", json);
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem.");
                }
            };

            await channel.BasicConsumeAsync(
                queue: QueueNames.VendasNormal,
                autoAck: false,
                consumer: consumer
            );

            // Mantém o serviço rodando até o token ser cancelado
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }


    }
}
