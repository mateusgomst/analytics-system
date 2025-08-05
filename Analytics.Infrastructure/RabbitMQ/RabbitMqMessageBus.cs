using System.Text;
using System.Text.Json;
using Analytics.Application.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Analytics.Infrastructure.Messaging
{
    public class RabbitMqMessageBus : IMessageBus
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMqMessageBus> _logger;

        public RabbitMqMessageBus(IConnection connection, ILogger<RabbitMqMessageBus> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task PublishAsync<T>(string queueName, T message)
        {
            try
            {
                using var channel = await _connection.CreateChannelAsync();
                
                // Declara a fila se não existir
                await channel.QueueDeclareAsync(
                    queue: queueName, 
                    durable: true, 
                    exclusive: false, 
                    autoDelete: false);

                // Serializa e converte para bytes
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);
                
                // Configura propriedades da mensagem para persistência
                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };
                
                // Publica a mensagem
                await channel.BasicPublishAsync(
                    exchange: "", 
                    routingKey: queueName, 
                    mandatory: false,
                    basicProperties: properties, 
                    body: body);

                _logger.LogInformation("Message published to queue {QueueName}: {Message}", queueName, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message to queue {QueueName}", queueName);
                throw;
            }
        }
    }
}