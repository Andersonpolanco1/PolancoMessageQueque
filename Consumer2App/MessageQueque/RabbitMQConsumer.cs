using Consumer2App.Controllers;
using Consumer2App.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer2App.MessageQueque
{
    public class RabbitMQConsumer : IMessageQueueConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly RabbitMQConfig _rabbitConfig;

        public RabbitMQConsumer(IOptions<RabbitMQConfig> options, ILogger<RabbitMQConsumer> logger)
        {
            _logger = logger;
            _rabbitConfig = options.Value;
            Init(_rabbitConfig, out _connection, out _channel, out _queueName);
        }

        private static void Init(RabbitMQConfig rabbitMQConfig, out IConnection _connection, out IModel _channel, out string _queueName)
        {
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
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += Message_OnReceived; 
                _channel.BasicConsume(_queueName, false, consumer);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"[RabbitMQ] Error. {ex.Message}");
                //Notificar que no se proceso el mensaje;
            }
      
            return Task.CompletedTask;
        }

        public void Message_OnReceived(object? sender,BasicDeliverEventArgs eventArgs)
        {
            try
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var user = JsonConvert.DeserializeObject<User>(message);

                if (user is null)
                {
                    _logger.LogInformation("[RabbitMQ] User to process is null.");
                    //enviar a cola de error aqui
                }
                else
                {
                    //Simula persistencia en bd (temporal)
                    HomeController.Users.Add(user);
                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                    _logger.LogInformation($"[RabbitMQ] procesado: UserId {user.Id}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"[RabbitMQ] Error. {ex.Message}");
                //enviar a cola de error aqui
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
