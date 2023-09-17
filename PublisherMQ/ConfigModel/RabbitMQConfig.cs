namespace Producer.ConfigModel
{
    public class RabbitMQConfig
    {
        public const string SectionName = "RabbitMQConfig";
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MainExchange { get; set; }
        public string NewUsersQueque { get; set; }
        public string RouterKey1 { get; set; }        
        public string DeletedUsersQueque { get; set; }
        public string RouterKey2 { get; set; }
        public string StaticsExchange { get; set; }
        public string UsersActionsQueue { get; set; }
        public string UsersActionsRouterKey { get; set; }
    }
}