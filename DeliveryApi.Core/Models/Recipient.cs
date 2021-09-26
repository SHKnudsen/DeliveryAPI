using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DeliveryAPI.Core.Models
{
    /// <summary>
    /// Defines the recipient of an order.
    /// </summary>
    public class Recipient
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonConstructor]
        public Recipient(string name, string address, string email, string phoneNumber)
        {
            Name = name;
            Address = address;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public Recipient() { }
    }
}
