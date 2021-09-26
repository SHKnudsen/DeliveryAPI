using System.Threading.Tasks;
using DeliveryAPI.Core.Models;

namespace DeliveryAPI.UserService.Data
{
    public interface IOrderRepo
    {
        Task<Root> CreateBogusOrder();

        Task<(bool success, Root root)> TryApproveOrder(string orderId);

        Task<Root> CancelOrder(string orderId);

        Task SendEmail(Root root, string recipentEmail, string emailSubject, string emailBody);

        Task OrderCompleted(string orderId);
    }
}
