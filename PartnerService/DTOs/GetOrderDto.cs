using DeliveryAPI.Core.Enums;
using DeliveryAPI.Core.Models;

namespace DeliveryAPI.PartnerService.DTOs
{
    public class GetOrderDto
    {
        public States State { get; set; }

        public AccessWindow AccessWindow { get; set; }

        public Recipient Recipient { get; set; }

        public Order Order { get; set; }
    }
}
