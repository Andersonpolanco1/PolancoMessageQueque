using RabbitMQ.Client.Events;

namespace Consumer2App.MessageQueque
{
    public interface IMessageQueueConsumer : IHostedService
    {
        void HandleMessage(object? sender, BasicDeliverEventArgs eventArgs);
    }
}
