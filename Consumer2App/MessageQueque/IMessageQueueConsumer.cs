using RabbitMQ.Client.Events;

namespace Consumer2App.MessageQueque
{
    public interface IMessageQueueConsumer : IHostedService
    {
        void Message_OnReceived(object? sender, BasicDeliverEventArgs eventArgs);
    }
}
