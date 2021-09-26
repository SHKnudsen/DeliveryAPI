using System;
using System.Threading.Tasks;
using AutoMapper;
using Dapr;
using Dapr.Client;
using DeliveryApi.Core.Helpers;
using DeliveryApi.Core.PubSubMessages;
using DeliveryAPI.Core.Enums;
using DeliveryAPI.Core.Models;
using DeliveryAPI.UserService.Data;
using DeliveryAPI.UserService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DeliveryAPI.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserOrderController : ControllerBase
    {
        private readonly IOrderRepo repo;
        private readonly ILogger<UserOrderController> logger;
        private readonly IMapper mapper;

        public UserOrderController(IOrderRepo repo, ILogger<UserOrderController> logger, IMapper mapper)
        {
            this.repo = repo;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateBogusOrder([FromServices] DaprClient daprClient)
        {
            try
            {
                var root = await repo.CreateBogusOrder();
                logger.LogInformation("Order {0}, to {1} created", root.Order.OrderNumber, root.Order.Sender);
                await daprClient.PublishEventAsync<Root>("pubsub", "ordercreated", root);
                return Ok(mapper.Map<CreateOrderDto>(root));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error happened while creating order");
                return StatusCode(500);
            }
        }

        [HttpPut("{orderId}")]
        public async Task<ActionResult> AcceptOrder(string orderId, [FromServices] DaprClient daprClient)
        {
            try
            {
                var taskResult = await repo.TryApproveOrder(orderId);

                if (taskResult.root is null)
                {
                    return OrderNotFound(orderId);
                }

                if (!taskResult.success)
                {
                    return Conflict(new { message = $"The order can not be approved, when its state is: {taskResult.root.State}" });
                }

                await daprClient.PublishEventAsync<Root>("pubsub", "orderapproved", taskResult.root);

                return Ok(mapper.Map<CreateOrderDto>(taskResult.root));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error happened while updating order");
                return StatusCode(500);
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult> CancelOrder(string orderId, [FromServices] DaprClient daprClient)
        {
            var result = await CancelOrderAndSendMail(orderId);

            var data = new OrderRemovedMsg
            {
                OrderId = orderId
            };

            await daprClient.PublishEventAsync<OrderRemovedMsg>("pubsub", "ordercanceled", data);
            return result;
        }

        #region PubSub endpoints

        [Topic("pubsub", "ordercanceledbypartner")]
        [Route("ordercanceled")]
        [HttpPost()]
        public async Task<ActionResult> OrderCanceled(OrderRemovedMsg data)
        {
            return await CancelOrderAndSendMail(data.OrderId);
        }


        [Topic("pubsub", "ordercompletedbypartner")]
        [Route("ordercompleted")]
        [HttpPost()]
        public async Task<ActionResult> OrderCompleted(OrderCompletedMsg data)
        {
            await repo.OrderCompleted(data.OrderId);
            return NoContent();
        }

        #endregion

        #region Private Helpers

        private async Task<ActionResult> CancelOrderAndSendMail(string orderId)
        {
            var root = await repo.CancelOrder(orderId);
            if (root is null)
            {
                return OrderNotFound(orderId);
            }

            if (root.State != States.Cancelled)
            {
                return Conflict(new { message = $"Orders can not be canceled when its state is: {root.State}" });
            }

            var emailSubject = $"Your order: {root.Order.OrderNumber} has been canceled";
            var emailBody = EmailUtils.UserOrderCanceledEmailBody(root);

            await repo.SendEmail(root, root.Recipient.Email, emailSubject, emailBody);
            return Ok(mapper.Map<CreateOrderDto>(root));
        }

        private ActionResult OrderNotFound(string orderId)
        {
            return NotFound(new { message = $"No order with {orderId} was found" });
        }

        #endregion
    }
}
