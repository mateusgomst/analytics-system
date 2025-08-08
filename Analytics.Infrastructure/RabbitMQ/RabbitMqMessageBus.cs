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
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync<T>(string queueName, T message)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                _logger.LogWarning("Queue name cannot be null or empty");
                return;
            }

            if (message == null)
            {
                _logger.LogWarning("Message cannot be null");
                return;
            }

            if (!_connection.IsOpen)
            {
                _logger.LogError("RabbitMQ connection is not open");
                return;
            }

            try
            {
                using var channel = await _connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Message published to queue {QueueName}", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message to queue {QueueName}", queueName);
                // Não lança exceção!
            }
        }
    }
}