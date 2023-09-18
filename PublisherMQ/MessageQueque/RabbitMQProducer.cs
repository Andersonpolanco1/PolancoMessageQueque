using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Producer.ConfigModel;
using Producer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;
using Action = Producer.Models.Action;

namespace Producer.MessageQueque
{
    public class RabbitMQProducer: IMessageQuequeProducer
    {
        private readonly RabbitMQConfig _rabbitConf;
        private readonly ILogger<RabbitMQProducer> _logger;
        private readonly IConnection _connection;

        public RabbitMQProducer(IOptions<RabbitMQConfig> rabbitOptions, ILogger<RabbitMQProducer> logger)
        {
            _rabbitConf = rabbitOptions.Value;
            _logger = logger;
            Init(_rabbitConf, out _connection);
        }

        private void Init(RabbitMQConfig rabbitMQConfig, out IConnection connection)
        {
            var conectionFactory = new ConnectionFactory()
            {
                HostName = _rabbitConf.Host,
                Port = _rabbitConf.Port,
                UserName = _rabbitConf.Username,
                Password = _rabbitConf.Password
            };
            connection = conectionFactory.CreateConnection();

            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(_rabbitConf.MainExchange, ExchangeType.Fanout, false);
                channel.ExchangeDeclare(_rabbitConf.StaticsExchange, ExchangeType.Direct, false);

                channel.QueueDeclare(_rabbitConf.NewUsersQueque,false, false,false);
                channel.QueueDeclare(_rabbitConf.DeletedUsersQueque, false, false,false);
                channel.QueueDeclare(_rabbitConf.UsersActionsQueue, false, false,false);

                channel.QueueBind(_rabbitConf.NewUsersQueque, _rabbitConf.MainExchange, _rabbitConf.RouterKey1);
                channel.QueueBind(_rabbitConf.DeletedUsersQueque, _rabbitConf.MainExchange, _rabbitConf.RouterKey2);
                channel.QueueBind(_rabbitConf.UsersActionsQueue, _rabbitConf.StaticsExchange, _rabbitConf.UsersActionsRouterKey);
            }
        }

        public void PublishUser(User user)
        {
            var objetoSerializado = JsonConvert.SerializeObject(user);

            if (Action.Deleted == user.UserAction)
                PublishMessage(objetoSerializado, _rabbitConf.MainExchange, _rabbitConf.RouterKey1);

            if(Action.Created == user.UserAction)
                PublishMessage(objetoSerializado, _rabbitConf.MainExchange, _rabbitConf.RouterKey2);

            PublishMessage(objetoSerializado, _rabbitConf.StaticsExchange, _rabbitConf.UsersActionsRouterKey);
        }

        private void PublishMessage(string message, string exchange, string routerkey)
        {
            var attemp = 1;
            Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>() 
            .WaitAndRetry(_rabbitConf.RetryPolicy, retryAttempt => TimeSpan.FromSeconds(Math.Pow(5, retryAttempt)), (ex, time) =>
            {
                _logger.LogError($"Error: {ex.Message}");
                _logger.LogWarning($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}. Retrying to publish in {time.TotalSeconds}s, retry {attemp}/{_rabbitConf.RetryPolicy}...");
                attemp++;
            })
            .Execute(() =>
            {
                using var channel = _connection.CreateModel();
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange, routerkey, basicProperties: null, body: body);
                _logger.LogInformation($"[RabbitMQ] Published to {exchange}.");
            });
        }

        public void Dispose()
        {
            if (_connection != null && _connection.IsOpen)
            {
                _connection.Close();
                _connection.Dispose(); 
            }
        }
    }
}
