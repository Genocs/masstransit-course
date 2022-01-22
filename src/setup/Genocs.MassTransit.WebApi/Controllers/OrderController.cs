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

        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrderController(ILogger<OrderController> logger,
                                IPublishEndpoint publishEndpoint,
                                ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
        }

        [HttpGet(Name = "")]
        public async Task<IActionResult> Get(Guid orderId)
            => await Task.FromResult(Ok());

        [HttpPost(Name = "")]
        public async Task<IActionResult> Post(Guid orderId, string customerNumber)
        {
            _logger.LogInformation("Publish SubmitOrder {orderId} to Consumer.", orderId);

            await _publishEndpoint.Publish<SubmitOrder>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = customerNumber
            });

            _logger.LogInformation("{orderId} Sent to Consumer.", orderId);

            return await Task.FromResult(Ok());
        }

        [HttpPut(Name = "")]
        public async Task<IActionResult> Put(Guid orderId, string customerNumber)
            => await Task.FromResult(Ok());


    }
}