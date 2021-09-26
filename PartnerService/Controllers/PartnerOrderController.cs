using System.Threading.Tasks;
using AutoMapper;
using Dapr;
using Dapr.Client;
using DeliveryApi.Core.Helpers;
using DeliveryApi.Core.PubSubMessages;
using DeliveryAPI.Core.Models;
using DeliveryAPI.PartnerService.Data;
using DeliveryAPI.PartnerService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DeliveryAPI.PartnerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerOrderController : ControllerBase
    {
        private readonly IPartnerOrdersRepo repo;
        private readonly DaprClient daprClient;
        private readonly ILogger<PartnerOrderController> logger;
        private readonly IMapper mapper;

        public PartnerOrderController(IPartnerOrdersRepo repo, DaprClient daprClient, ILogger<PartnerOrderController> logger, IMapper mapper)
        {
            this.repo = repo;
            this.daprClient = daprClient;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetOrder(string orderId)
        {
            var order = await repo.GetOrder(orderId);

            if (order is null)
            {
                return NotFound(new { mesage = $"No order with '{orderId}' was found" });
            }

            return Ok(mapper.Map<GetOrderDto>(order));
        }

        [HttpPut("{orderId}")]
        public async Task<ActionResult> CompleteOrder(string orderId)
        {
            var result = await repo.TryCompleteOrder(orderId);

            if (result.root is null)
            {
                return NotFound(new { mesage = $"No order with '{orderId}' was found" });
            }

            if (!result.success)
            {
                return Conflict(new { message = $"The order can not be completed, when its state is: {result.root.State}" });
            }

            var emailSubject = $"{result.root.Order.OrderNumber} has been completed";
            var emailBody = EmailUtils.PartnerOrderCompletedEmailBody(result.root);

            var data = new OrderCompletedMsg
            {
                OrderId = orderId
            };

            await daprClient.PublishEventAsync<OrderCompletedMsg>("pubsub", "ordercompletedbypartner", data);
            await repo.SendEmail(result.root, result.root.Recipient.Email, emailSubject, emailBody);

            return Ok(mapper.Map<OrderCompletedDto>(result.root));
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult> CancelOrder(string orderId)
        {
            var root = await repo.GetOrder(orderId);
            if (root is null)
            {
                return NotFound(new { mesage = $"No order with '{orderId}' was found" });
            }

            if (root.State != DeliveryAPI.Core.Enums.States.Created && 
                root.State != DeliveryAPI.Core.Enums.States.Approved)
            {
                return Conflict(new { message = $"The order can not be canceled, when its state is: {root.State}" });
            }

            var canceledRoot = await repo.CancelOrder(orderId);

            var data = new OrderRemovedMsg
            {
                OrderId = orderId
            };

            await daprClient.PublishEventAsync<OrderRemovedMsg>("pubsub", "ordercanceledbypartner", data);

            return Ok(mapper.Map<OrderExpiredDto>(canceledRoot));
        }

        #region PubSub endpoints

        [Topic("pubsub", "ordercreated")]
        [Route("orderrecived")]
        [HttpPost()]
        public async Task<ActionResult> OrderRecived(Root root)
        {
            await repo.SaveOrder(root);

            var emailSubject = $"New order revived {root.Order.OrderNumber}";
            var emailBody = EmailUtils.PartnerOrderRecivedEmailBody(root);
            await repo.SendEmail(root, root.Order.SenderEmail, emailSubject, emailBody);

            return Ok();
        }

        [Topic("pubsub", "orderapproved")]
        [Route("orderapproved")]
        [HttpPost()]
        public async Task<ActionResult> OrderApproved(Root root)
        {
            await repo.SaveOrder(root);

            var emailSubject = $"{root.Order.OrderNumber} has been approved";
            var emailBody = EmailUtils.PartnerOrderApprovedEmailBody(root);
            await repo.SendEmail(root, root.Order.SenderEmail, emailSubject, emailBody);

            return Ok();
        }

        [Topic("pubsub", "orderexpired")]
        [Route("orderexpired")]
        [HttpPost()]
        public async Task<ActionResult> OrderExpired(OrderRemovedMsg data)
        {
            var expiredRoot = await repo.OrderExpired(data.OrderId);

            return Ok(mapper.Map<OrderExpiredDto>(expiredRoot));
        }

        [Topic("pubsub", "ordercanceled")]
        [Route("ordercanceled")]
        [HttpPost()]
        public async Task<ActionResult> OrderCanceled(OrderRemovedMsg data)
        {
            var canceledRoot = await repo.CancelOrder(data.OrderId);

            return Ok(mapper.Map<OrderExpiredDto>(canceledRoot));
        }

        #endregion

    }
}
