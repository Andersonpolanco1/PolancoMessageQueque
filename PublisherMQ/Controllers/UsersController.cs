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

        public UsersController(ILogger<UsersController> logger, IMessageQuequeProducer producer)
        {
            _logger = logger;
            _producer = producer;
        }

        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            //Aqui haces las acciones que debas hacer antes de publicar a las colas
            _producer.PublishUser(user);
            _logger.LogInformation($"[RabbitMQ] Pusblished: UserId {user.Id}");
            return Ok();
        }

    }
}
