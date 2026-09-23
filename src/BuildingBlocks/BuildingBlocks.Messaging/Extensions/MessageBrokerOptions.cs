namespace BuildingBlocks.Messaging.Extensions
{
    public sealed class MessageBrokerOptions
    {
        public const string InMemory = "InMemory";
        public const string RabbitMq = "RabbitMq";

        public string Transport { get; set; } = InMemory;
        public string Host { get; set; } = "localhost";
        public string VirtualHost { get; set; } = "/";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
