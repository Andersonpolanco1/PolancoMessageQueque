using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Producer.ConfigModel;
using Producer.MessageQueque;
using Producer.Models;

namespace PublisherMQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly ILogger<UsersController> _logger;
        private readonly IMessageQuequeProducer _producer;
        private readonly RabbitMQConfig _rabbitOptions;

        public UsersController(ILogger<UsersController> logger,
            IMessageQuequeProducer producer,
            IOptions<RabbitMQConfig> rabbitOptions
            )
        {
            _logger = logger;
            _producer = producer;
            _rabbitOptions = rabbitOptions.Value;
        }

        [HttpPost]
        public IActionResult Post([FromBody] User user, string? exchange, string? routerkey)
        {

            _producer.PublishUser(user);
            return Ok($"[RabbitMQ] Pusblished: UserId {user.Id}");
        }

    }
}
