using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DeliveryAPI.Core.Models
{
    /// <summary>
    /// Defines an order.
    /// </summary>
    public class Order
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("senderEmail")]
        public string SenderEmail { get; set; }

        [JsonConstructor]
        public Order(string orderNumber, string sender, string senderEmail)
        {
            OrderNumber = orderNumber;
            Sender = sender;
            SenderEmail = senderEmail;
        }

        public Order() { }
    }
}
