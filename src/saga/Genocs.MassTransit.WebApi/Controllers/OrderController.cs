using Genocs.MassTransit.Components.Consumers;
using Genocs.MassTransitContracts;
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
        //       private readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;
        private readonly IRequestClient<OrderStatus> _checkOrderClient;

        public OrderController(ILogger<OrderController> logger,
            ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint,
            IRequestClient<OrderStatus> checkOrderClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
            _checkOrderClient = checkOrderClient ?? throw new ArgumentNullException(nameof(checkOrderClient));
            //            _submitOrderRequestClient = submitOrderRequestClient;

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

        [HttpPut(Name = "")]
        public async Task<IActionResult> Put(Guid id, string customerNumber)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));

            await endpoint.Send<SubmitOrder>(new
            {
                OrderId = id,
                InVar.Timestamp,
                CustomerNumber = customerNumber
            });

            return await Task.FromResult(Ok());
        }
    }
}