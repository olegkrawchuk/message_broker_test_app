using System;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace MessageBrokerTestApp.Models.MessageModels
{
    public class ExternalLoginRequestViewModel
    {
        public string Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AuthenticationState Status { get; set; } = AuthenticationState.Authenticated;

        public int ExpiresIn { get; set; } = 3600;

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public ExternalApplicationViewModel Application { get; set; }
    }
}
