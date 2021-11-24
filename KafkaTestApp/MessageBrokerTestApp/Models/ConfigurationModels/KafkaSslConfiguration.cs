namespace MessageBrokerTestApp.Models.ConfigurationModels
{
    public class KafkaSslConfiguration
    {
        public string KeyLocation { get; set; }
        public string KeyPassword { get; set; }
        public string CertificateLocation { get; set; }
        public string CaLocation { get; set; }
    }
}
