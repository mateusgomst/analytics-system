namespace Analytics.Infrastructure.Messaging.RabbitMQ.Config
{
    // Esta classe agora é apenas um objeto de dados simples (POCO).
    // Os valores padrão serão usados apenas se a configuração não for encontrada.
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "admin";
        public string Password { get; set; } = "admin";
        public int Port { get; set; } = 5672;
    }
}
