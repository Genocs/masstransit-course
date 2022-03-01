using Genocs.MassTransit.Contracts;
using MassTransit;
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

        public OrderController(ILogger<OrderController> logger,
            ISendEndpointProvider sendEndpointProvider, 
            IRequestClient<OrderStatus> checkOrderClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
            _checkOrderClient = checkOrderClient ?? throw new ArgumentNullException(nameof(checkOrderClient));
        }


        [HttpGet(Name = "")]
        public async Task<IActionResult> Get(Guid orderId)
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

        [HttpPost(Name = "")]
        public async Task<IActionResult> Post(Guid orderId, string customerNumber)
        {
            //Genocs.MassTransit.Contracts:OrderSubmitted
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:Genocs.MassTransit.Contracts:OrderSubmitted"));

            await endpoint.Send<OrderSubmitted>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = customerNumber
            });

            return Ok(orderId);
        }

        [HttpPut(Name = "")]
        public async Task<IActionResult> Put(Guid orderId, string customerNumber)
            => await Task.FromResult(Ok());
    }
}