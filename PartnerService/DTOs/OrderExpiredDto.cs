using System.ComponentModel.DataAnnotations;
using DeliveryAPI.Core.Enums;
using DeliveryAPI.Core.Models;
using Newtonsoft.Json;

namespace DeliveryAPI.PartnerService.DTOs
{
    public class OrderExpiredDto
    {
        [JsonProperty("state")]
        [Required]
        public States State { get; set; }

        [Required]
        public string OrderId { get; set; }

        public Recipient Customer { get; set; }
    }
}
