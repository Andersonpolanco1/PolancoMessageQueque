using RabbitMQ.Client.Events;

namespace Consumer1App.MessageQueque
{
    public interface IMessageQueueConsumer : IHostedService
    {
        void HandleMessage(object? sender, BasicDeliverEventArgs eventArgs);
    }
}
