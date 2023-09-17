using Producer.Models;

namespace Producer.MessageQueque
{
    public interface IMessageQuequeProducer
    {
        void PublishUser(User user);
    }
}
