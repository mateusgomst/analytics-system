using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Analytics.Infrastructure.Messaging.RabbitMQ.Config
{
    public class RabbitMQConnection : IDisposable
    {
        public IConnection Connection { get; private set; }

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

            try
            {
                // Tenta criar a conexão RabbitMQ.
                // Com o healthcheck no Docker Compose, o RabbitMQ já deve estar pronto aqui.
                Connection = _factory.CreateConnectionAsync().GetAwaiter().GetResult();
                Console.WriteLine("Conexão com RabbitMQ estabelecida com sucesso.");
            }
            catch (Exception ex)
            {
                // Se a conexão falhar aqui, é um problema real de configuração ou rede,
                // pois o Docker Compose já garantiu que o RabbitMQ está saudável.
                Console.WriteLine($"Erro crítico ao conectar ao RabbitMQ: {ex.Message}");
                throw; // Relançar a exceção, pois a aplicação não pode prosseguir sem conexão.
            }
        }

        public void Dispose()
        {
            if (Connection != null && Connection.IsOpen)
            {
                Connection.CloseAsync();
                Connection.Dispose();
            }
        }
    }
}
