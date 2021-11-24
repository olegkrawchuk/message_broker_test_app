namespace MessageBrokerTestApp.Models.ConfigurationModels
{
    public class RabbitMqConfiguration
    {
        public string Url { get; set; }
        public string VirtualHost { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
