using System.Threading.Tasks;
using DeliveryAPI.Core.Models;

namespace DeliveryAPI.PartnerService.Data
{
    public interface IPartnerOrdersRepo
    {
        Task<Root> GetOrder(string orderId);
        Task SaveOrder(Root root);
        Task<(bool success, Root root)> TryCompleteOrder(string orderId);
        Task<Root> CancelOrder(string orderId);
        Task<Root> OrderExpired(string orderId);
        Task SendEmail(Root root, string recipentEmail, string emailSubject, string emailBody);

    }
}
