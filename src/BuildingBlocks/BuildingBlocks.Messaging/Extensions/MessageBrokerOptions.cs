namespace BuildingBlocks.Messaging.Extensions
{
    public sealed class MessageBrokerOptions
    {
        public string Host { get; set; } = "amqp://localhost:5672";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}