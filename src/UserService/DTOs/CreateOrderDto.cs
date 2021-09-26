using System;
using System.ComponentModel.DataAnnotations;
using DeliveryAPI.Core.Enums;
using Newtonsoft.Json;

namespace DeliveryAPI.UserService.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public States State { get; set; }

        [Required]
        public string OrderId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
