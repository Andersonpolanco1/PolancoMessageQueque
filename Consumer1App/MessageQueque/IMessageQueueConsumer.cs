using RabbitMQ.Client.Events;

namespace Consumer1App.MessageQueque
{
    public interface IMessageQueueConsumer : IHostedService
    {
        void Message_OnReceived(object? sender, BasicDeliverEventArgs eventArgs);
    }
}
