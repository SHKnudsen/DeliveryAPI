using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryAPI.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DeliveryAPI.Core.Models
{
    public class Root
    {
        [JsonProperty("state")]
        public States State { get; set; }

        [JsonProperty("accessWindow")]
        public AccessWindow AccessWindow { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }

        [JsonProperty("order")]
        public Order Order { get; set; }

        [JsonConstructor]
        public Root(States state, AccessWindow accessWindow, Recipient recipient, Order order)
        {
            State = state;
            AccessWindow = accessWindow;
            Recipient = recipient;
            Order = order;
        }

        public Root(Recipient recipient, Order order)
        {
            Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
            Order = order ?? throw new ArgumentNullException(nameof(order));

            State = States.Created;
            AccessWindow = new AccessWindow();
        }

        public Root() { }

    }
}
