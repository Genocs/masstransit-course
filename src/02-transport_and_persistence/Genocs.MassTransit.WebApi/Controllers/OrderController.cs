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

        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrderController(ILogger<OrderController> logger,
            ISendEndpointProvider sendEndpointProvider,
            IPublishEndpoint publishEndpoint)
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
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));

            await endpoint.Send<SubmitOrder>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = customerNumber
            });

            return await Task.FromResult(Ok());
        }

        [HttpPut(Name = "")]
        public async Task<IActionResult> Put(Guid orderId, string customerNumber)
            => await Task.FromResult(Ok());
    }
}