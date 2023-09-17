using Consumer1App.Controllers;
using Consumer1App.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer1App.MessageQueque
{
    public class RabbitMQConsumer : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;
        private readonly ILogger<RabbitMQConsumer> _logger;

        public RabbitMQConsumer(IOptions<RabbitMQConfig> options, ILogger<RabbitMQConsumer> logger)
        {
            var rabbitMQConfig = options.Value;
            var factory = new ConnectionFactory
            {
                HostName = rabbitMQConfig.Host,
                UserName = rabbitMQConfig.Username,
                Password = rabbitMQConfig.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = rabbitMQConfig.Queque;

            _channel.QueueDeclare(
                queue: rabbitMQConfig.Queque,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            this._logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (sender, eventArgs) => HandleMessage(eventArgs); 
                _channel.BasicConsume(_queueName, false, consumer);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"[RabbitMQ] Error. {ex.Message}");
            }
      
            return Task.CompletedTask;
        }

        private void HandleMessage(BasicDeliverEventArgs eventArgs)
        {
            try
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var user = JsonConvert.DeserializeObject<User>(message);
                HomeController.Users.Add(user);
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch (Exception ex)
            {   
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            return Task.CompletedTask;
        }
    }

}
