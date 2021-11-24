using System;

namespace MessageBrokerTestApp.Models.MessageModels
{
    public class SimpleMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Text { get; set; }
    }
}
