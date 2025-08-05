namespace Analytics.Application.Interfaces
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(string queueName, T message);
    }
}
