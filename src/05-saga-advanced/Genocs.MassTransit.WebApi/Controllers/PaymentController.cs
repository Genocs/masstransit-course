using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Genocs.MassTransit.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        private readonly ISendEndpointProvider _sendEndpointProvider;

        private readonly IRequestClient<PaymentStatus> _checkPaymentClient;
        private readonly IRequestClient<PaymentRequest> _paymentRequestClient;

        public PaymentController(ILogger<PaymentController> logger,
            ISendEndpointProvider sendEndpointProvider,
            IRequestClient<PaymentStatus> checkPaymentClient,
            IRequestClient<PaymentRequest> paymentRequestClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
            _checkPaymentClient = checkPaymentClient ?? throw new ArgumentNullException(nameof(checkPaymentClient));
            _paymentRequestClient = paymentRequestClient ?? throw new ArgumentNullException(nameof(paymentRequestClient));
        }


        [HttpGet(Name = "PaymentStatus")]
        public async Task<IActionResult> Get(Guid orderId)
        {
            var (orderStatus, notFound) = await _checkPaymentClient.GetResponse<PaymentStatus, PaymentNotFound>(new
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

        [HttpPost(Name = "Submit")]
        public async Task<IActionResult> Post(Guid orderId, string customerNumber, string paymentCardNumber)
        {
            var interfaceType = typeof(PaymentRequest);
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentRequest"));

            await endpoint.Send<PaymentRequest>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = customerNumber,
                PaymentCardNumber = paymentCardNumber
            });

            return Ok(orderId);
        }

        [HttpPut("Progress")]
        public async Task<IActionResult> PutProgress(Guid orderId)
        {
            var interfaceType = typeof(PaymentProgress);
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentProgress"));

            await endpoint.Send<PaymentProgress>(new
            {
                OrderId = orderId,
                InVar.Timestamp
            });
            return Ok(orderId);
        }

        [HttpPut("Captured")]
        public async Task<IActionResult> PutCaptured(Guid orderId)
        {
            var interfaceType = typeof(PaymentCaptured);
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentCaptured"));

            await endpoint.Send<PaymentCaptured>(new
            {
                OrderId = orderId,
                InVar.Timestamp
            });
            return Ok(orderId);
        }

        [HttpPut("Authorized")]
        public async Task<IActionResult> PutAuthorized(Guid orderId)
        {
            var interfaceType = typeof(PaymentAuthorized);
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentAuthorized"));

            await endpoint.Send<PaymentAuthorized>(new
            {
                OrderId = orderId,
                InVar.Timestamp
            });
            return Ok(orderId);
        }

        [HttpPut("NotCaptured")]
        public async Task<IActionResult> PutNotCaptured(Guid orderId)
        {
            var interfaceType = typeof(PaymentNotAuthorized);
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentNotAuthorized"));

            await endpoint.Send<PaymentNotAuthorized>(new
            {
                OrderId = orderId,
                InVar.Timestamp
            });
            return Ok(orderId);
        }

        [HttpPut("NotAuthorized")]
        public async Task<IActionResult> PutNotAuthorized(Guid orderId)
        {
            var interfaceType = typeof(PaymentNotAuthorized);
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentNotAuthorized"));

            await endpoint.Send<PaymentNotAuthorized>(new
            {
                OrderId = orderId,
                InVar.Timestamp
            });
            return Ok(orderId);
        }
    }
}