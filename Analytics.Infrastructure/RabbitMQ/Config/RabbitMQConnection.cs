using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Analytics.Infrastructure.Messaging.RabbitMQ.Config
{
    public class RabbitMQConnection
    {
        private readonly ConnectionFactory _factory;

        public RabbitMQConnection(IOptions<RabbitMQSettings> options)
        {
            var settings = options.Value;
            _factory = new ConnectionFactory()
            {
                HostName = settings.HostName,
                Port = settings.Port,
                UserName = settings.UserName,
                Password = settings.Password
            };
        }

        public async Task<IConnection> CreateConnection()
        {
            return await _factory.CreateConnectionAsync();
        }
    }
}