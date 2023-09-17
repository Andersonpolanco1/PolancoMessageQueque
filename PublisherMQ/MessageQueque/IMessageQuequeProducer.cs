using Producer.Models;

namespace Producer.MessageQueque
{
    public interface IMessageQuequeProducer
    {
        void PublishMessage(string message, string exchange, string routerkey);
        void PublishUser(User user, string exchange, string routerkey);
    }
}
