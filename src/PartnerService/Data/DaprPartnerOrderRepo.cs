using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Client;
using DeliveryApi.Core.Helpers;
using DeliveryAPI.Core.Models;

namespace DeliveryAPI.PartnerService.Data
{
    public class DaprPartnerOrderRepo : IPartnerOrdersRepo
    {
        private const string DAPR_STORE_NAME = "statestore";
        private readonly DaprClient daprClient;

        public DaprPartnerOrderRepo(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }

        /// <summary>
        /// Gets an order from the <see cref="DaprClient"/> statestore.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<Root> GetOrder(string orderId)
        {
            var root = await daprClient.GetStateAsync<Root>(
                DAPR_STORE_NAME, orderId);

            return root;
        }

        /// <summary>
        /// Saves an order to the <see cref="DaprClient"/> statestore.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public async Task SaveOrder(Root root)
        {
            await daprClient.SaveStateAsync<Root>(
                DAPR_STORE_NAME, root.Order.OrderNumber, root);
        }

        /// <summary>
        /// Try to complete an active order.
        /// Orders can only be completed if their current state is
        /// Approved.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<(bool success, Root root)> TryCompleteOrder(string orderId)
        {
            var order = await GetOrder(orderId);
            if (order is null)
            {
                return (false, null);
            }
            if (order.State != DeliveryAPI.Core.Enums.States.Approved)
            {
                return (false, order);
            }

            order.State = DeliveryAPI.Core.Enums.States.Completed;
            return (true, order);
        }

        /// <summary>
        /// Cancels an active order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<Root> CancelOrder(string orderId)
        {
            var root = await GetOrder(orderId);
            root.State = DeliveryAPI.Core.Enums.States.Cancelled;
            await daprClient.DeleteStateAsync(
                DAPR_STORE_NAME, orderId);

            var emailSubject = $"{root.Order.OrderNumber} has been canceled";
            var emailBody = EmailUtils.PartnerOrderCanceledEmailBody(root);
            await SendEmail(root, root.Order.SenderEmail, emailSubject, emailBody);

            return root;
        }

        public async Task<Root> OrderExpired(string orderId)
        {
            var root = await GetOrder(orderId);
            await daprClient.DeleteStateAsync(
                DAPR_STORE_NAME, orderId);

            var emailSubject = $"{root.Order.OrderNumber} has expired";
            var emailBody = EmailUtils.PartnerOrderExpiredEmailBody(root);
            await SendEmail(root, root.Order.SenderEmail, emailSubject, emailBody);

            return root;
        }

        public async Task SendEmail(Root root, string recipentEmail, string emailSubject, string emailBody)
        {
            var metadata = new Dictionary<string, string>
            {
                ["emailFrom"] = "noreply@deliveryapi.co",
                ["emailTo"] = recipentEmail,
                ["subject"] = emailSubject
            };

            await daprClient.InvokeBindingAsync("sendmail", "create", emailBody, metadata);
        }
    }
}
