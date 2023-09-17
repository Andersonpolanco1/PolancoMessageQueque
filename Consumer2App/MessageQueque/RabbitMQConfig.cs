namespace Consumer2App.MessageQueque
{
    public class RabbitMQConfig
    {
        public const string SectionName = "RabbitMQConfig";
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Exchange { get; set; }
        public string Queque { get; set; }
        public string RouterKey { get; set; }
    }
}
