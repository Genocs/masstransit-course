using Genocs.MassTransit.Components.Consumers;
using Genocs.MassTransit.Contracts;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Mvc;

namespace Genocs.MassTransit.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        private readonly ISendEndpointProvider _sendEndpointProvider;

        private readonly IRequestClient<OrderStatus> _checkOrderClient;
        private readonly IRequestClient<SubmitOrderWithResponse> _submitOrderWithResponseClient;


        public OrderController(ILogger<OrderController> logger,
            ISendEndpointProvider sendEndpointProvider,
            IRequestClient<OrderStatus> checkOrderClient,
            IRequestClient<SubmitOrderWithResponse> submitOrderWithResponseClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
            _checkOrderClient = checkOrderClient ?? throw new ArgumentNullException(nameof(checkOrderClient));
            _submitOrderWithResponseClient = submitOrderWithResponseClient ?? throw new ArgumentNullException(nameof(submitOrderWithResponseClient));
        }


        [HttpGet("GetOrder")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            var (orderStatus, notFound) = await _checkOrderClient.GetResponse<OrderStatus, OrderNotFound>(new
            {
                orderId
            });

            if (orderStatus.IsCompletedSuccessfully)
            {
                var response = await orderStatus;
                return Ok(response.Message);
            }
            else
            {
                var response = await notFound;
                return NotFound(response.Message);
            }
        }

        [HttpPost("InsertOrder")]
        public async Task<IActionResult> PostOrder(Guid orderId, string customerNumber)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));

            await endpoint.Send<SubmitOrder>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = customerNumber
            });

            return Ok(orderId);
        }

        [HttpPost("SubmitOrderWithResponse")]
        public async Task<IActionResult> PostSubmitOrderWithResponse(Guid orderId, string customerNumber)
        {
            var (OrderAccepted, orderRejected) = await _submitOrderWithResponseClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
            {
                orderId,
                InVar.Timestamp,
                CustomerNumber = customerNumber
            });

            if (OrderAccepted.IsCompletedSuccessfully)
            {
                var response = await OrderAccepted;
                return Ok(response.Message);
            }
            else
            {
                var response = await orderRejected;
                return NotFound(response.Message);
            }
        }

        [HttpPut("UpdateOrder")]
        public async Task<IActionResult> PutOrder(Guid orderId, string customerNumber)
            => await Task.FromResult(Ok());
    }
}