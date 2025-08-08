using Analytics.Application.Constants;
using Analytics.Application.DTOs;
using Analytics.Application.Repositories;
using Analytics.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Analytics.API.Services
{
    public class EventConsumerService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventConsumerService> _logger;

        public EventConsumerService(IConnection connection, IServiceProvider serviceProvider, ILogger<EventConsumerService> logger)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando consumo da fila '{Queue}'...", QueueNames.Events);

            var channel = await _connection.CreateChannelAsync(); 
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("Mensagem recebida: {Json}", json);

                try
                {
                    var evento = JsonSerializer.Deserialize<Event>(json);
                    if (evento != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                        await repo.NewEvent(evento);
                        _logger.LogInformation("Evento salvo no banco com sucesso.");
                    }
                    else
                    {
                        _logger.LogWarning("Falha ao desserializar evento: {Json}", json);
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem.");
                }
            };

            await channel.BasicConsumeAsync(
                queue: QueueNames.Events,
                autoAck: false,
                consumer: consumer
            );

            // Mantém o serviço rodando até o token ser cancelado
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }


    }
}
