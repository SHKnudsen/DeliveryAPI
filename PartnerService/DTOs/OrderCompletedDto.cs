using System.ComponentModel.DataAnnotations;
using DeliveryAPI.Core.Enums;
using Newtonsoft.Json;

namespace DeliveryAPI.PartnerService.DTOs
{
    public class OrderCompletedDto
    {
        [JsonProperty("state")]
        [Required]
        public States State { get; set; }

        [Required]
        public string OrderId { get; set; }
    }
}
