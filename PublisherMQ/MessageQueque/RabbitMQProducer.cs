using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Producer.ConfigModel;
using Producer.Models;
using RabbitMQ.Client;
using System.Text;

namespace Producer.MessageQueque
{
    public class RabbitMQProducer: IMessageQuequeProducer
    {
        private readonly RabbitMQConfig _rabbitConf;
        private ILogger<RabbitMQProducer> _logger;
        private IConnection _connection;


        public RabbitMQProducer(IOptions<RabbitMQConfig> rabbitOptions, ILogger<RabbitMQProducer> logger)
        {
            _rabbitConf = rabbitOptions.Value;
            _logger = logger;
            Connect();

        }

        private void SetConnectionFactory()
        {
            if (_connection == null || (!_connection.IsOpen))
            {
                var conectionFactory = new ConnectionFactory()
                {
                    HostName = _rabbitConf.Host,
                    Port = _rabbitConf.Port,
                    UserName = _rabbitConf.Username,
                    Password = _rabbitConf.Password
                };

                _connection = conectionFactory.CreateConnection();
            }
        }

        private void Connect()
        {
            SetConnectionFactory();
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(_rabbitConf.MainExchange, ExchangeType.Direct, false); ;

                channel.QueueDeclare(queue: _rabbitConf.NewUsersQueque,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueDeclare(queue: _rabbitConf.DeletedUsersQueque,
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

                channel.QueueBind(_rabbitConf.NewUsersQueque, _rabbitConf.MainExchange, routingKey: _rabbitConf.RouterKey1);
                channel.QueueBind(_rabbitConf.DeletedUsersQueque, _rabbitConf.MainExchange, routingKey: _rabbitConf.RouterKey2);
            }

            _logger.LogInformation("[RabbitMQ] Connected.");
        }


        public void PublishUser(User user, string exchange, string routerkey)
        {
            var objetoSerializado = JsonConvert.SerializeObject(user);
            PublishMessage(objetoSerializado, exchange, routerkey);
        }

        public void PublishMessage(string message, string exchange, string routerkey)
        {
            try
            {
                using var channel = _connection.CreateModel();
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange, routerkey, basicProperties: null, body: body);
                _logger.LogInformation("[RabbitMQ] Mensaje enviado...");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("No se pudo enviar el mensaje..." + ex.Message);
                Dispose();
            }

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
