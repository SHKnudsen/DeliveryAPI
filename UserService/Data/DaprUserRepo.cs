using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Client;
using DeliveryApi.Core.Helpers;
using DeliveryApi.Core.PubSubMessages;
using DeliveryAPI.Core.Enums;
using DeliveryAPI.Core.Models;
using DeliveryAPI.UserService.Utils;

namespace DeliveryAPI.UserService.Data
{
    public class DaprUserRepo : IOrderRepo
    {
        private const string DAPR_STORE_NAME = "statestore";
        private readonly DaprClient daprClient;
        private readonly IStateWatcher stateWatcher;

        public DaprUserRepo(DaprClient daprClient, IStateWatcher stateWatcher)
        {
            this.daprClient = daprClient;
            this.stateWatcher = stateWatcher;
            stateWatcher.OrderExpired += OnOrderExpired;
        }

        /// <summary>
        /// Creates a new order using fake data form
        /// the <see cref="BogusData"/> library
        /// </summary>
        /// <returns></returns>
        public async Task<Root> CreateBogusOrder()
        {
            var root = BogusData.GenerateBogusRoot();
            stateWatcher.AddWatcher(root.Order.OrderNumber, root.AccessWindow.StartTime, root.AccessWindow.EndTime);

            await daprClient.SaveStateAsync<Root>(
                DAPR_STORE_NAME, root.Order.OrderNumber, root);

            return root;
        }

        /// <summary>
        /// Try and approve an active order.
        /// Orders can only be approved if their current
        /// state is Created.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<(bool success, Root root)> TryApproveOrder(string orderId)
        {
            var order = await daprClient.GetStateAsync<Root>(
                DAPR_STORE_NAME, orderId);

            if (order is null ||
                order.State != States.Created)
            {
                return (false, order);
            }

            order.State = States.Approved;

            await daprClient.SaveStateAsync<Root>(
                DAPR_STORE_NAME, order.Order.OrderNumber, order);

            return (true, order);
        }

        /// <summary>
        /// Cancels an active order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<Root> CancelOrder(string orderId)
        {
            var order = await daprClient.GetStateAsync<Root>(
                DAPR_STORE_NAME, orderId);


            if (order is null)
            {
                return null;
            }

            if (order.State == States.Completed ||
                order.State == States.Expired ||
                order.State == States.Cancelled)
            {
                return order;
            }

            order.State = States.Cancelled;
            await daprClient.DeleteStateAsync(DAPR_STORE_NAME, orderId);
            stateWatcher.RemoveWatcher(orderId);

            return order;
        }

        public async Task OrderCompleted(string orderId)
        {
            var root = await GetOrder(orderId);
            root.State = States.Completed;
            stateWatcher.RemoveWatcher(orderId);
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

        #region Private Helpers

        private async Task<Root> GetOrder(string orderId)
        {
            var root = await daprClient.GetStateAsync<Root>(
                DAPR_STORE_NAME, orderId);

            return root;
        }

        private async void OnOrderExpired(string orderId)
        {
            var root = await GetOrder(orderId);
            if (root is null)
            {
                return;
            }

            var emailSubject = $"{root.Order.OrderNumber} has expired";
            var emailBody = EmailUtils.UserOrderExpiredEmailBody(root);

            await SendEmail(root, root.Recipient.Email, emailSubject, emailBody);

            await daprClient.DeleteStateAsync(
                DAPR_STORE_NAME, orderId);

            var data = new OrderRemovedMsg
            {
                OrderId = root.Order.OrderNumber
            };

            await daprClient.PublishEventAsync<OrderRemovedMsg>("pubsub", "orderexpired", data);
        }

        #endregion
    }
}
