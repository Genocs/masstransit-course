using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Genocs.MassTransit.WebApi.Controllers;

/// <summary>
/// This Controller act as a payment system simulator
/// </summary>
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
    public async Task<IActionResult> Get(Guid paymentOrderId)
    {
        var (orderStatus, notFound) = await _checkPaymentClient.GetResponse<PaymentStatus, PaymentNotFound>(new
        {
            paymentOrderId
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
    public async Task<IActionResult> Post(Guid paymentOrderId, Guid orderId, string customerNumber, string paymentCardNumber)
    {
        // var interfaceType = typeof(PaymentRequest);
        // { KebabCaseEndpointNameFormatter.Instance.Consumer<PaymentRequest>()}
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentRequest"));

        await endpoint.Send<PaymentRequest>(new
        {
            PaymentOrderId = paymentOrderId,
            OrderId = orderId,
            InVar.Timestamp,
            CustomerNumber = customerNumber,
            PaymentCardNumber = paymentCardNumber
        });

        return Ok(paymentOrderId);
    }

    [HttpPut("Progress")]
    public async Task<IActionResult> PutProgress(Guid paymentOrderId)
    {
        // var interfaceType = typeof(PaymentProgress);
        // {KebabCaseEndpointNameFormatter.Instance.Consumer<PaymentProgress>()}
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentProgress"));

        await endpoint.Send<PaymentProgress>(new
        {
            PaymentOrderId = paymentOrderId,
            InVar.Timestamp
        });
        return Ok(paymentOrderId);
    }

    [HttpPut("Captured")]
    public async Task<IActionResult> PutCaptured(Guid paymentOrderId)
    {
        var interfaceType = typeof(PaymentCaptured);
        // { KebabCaseEndpointNameFormatter.Instance.Consumer<PaymentCaptured>()}
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentCaptured"));

        await endpoint.Send<PaymentCaptured>(new
        {
            PaymentOrderId = paymentOrderId,
            InVar.Timestamp
        });
        return Ok(paymentOrderId);
    }

    [HttpPut("Authorized")]
    public async Task<IActionResult> PutAuthorized(Guid paymentOrderId)
    {
        var interfaceType = typeof(PaymentAuthorized);
        // { KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentAuthorized"));

        await endpoint.Send<PaymentAuthorized>(new
        {
            PaymentOrderId = paymentOrderId,
            InVar.Timestamp
        });
        return Ok(paymentOrderId);
    }

    [HttpPut("NotCaptured")]
    public async Task<IActionResult> PutNotCaptured(Guid paymentOrderId)
    {
        var interfaceType = typeof(PaymentNotAuthorized);
        // { KebabCaseEndpointNameFormatter.Instance.Consumer<PaymentNotAuthorized>()}
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentNotAuthorized"));

        await endpoint.Send<PaymentNotAuthorized>(new
        {
            PaymentOrderId = paymentOrderId,
            InVar.Timestamp
        });
        return Ok(paymentOrderId);
    }

    [HttpPut("NotAuthorized")]
    public async Task<IActionResult> PutNotAuthorized(Guid paymentOrderId)
    {
        var interfaceType = typeof(PaymentNotAuthorized);
        // { KebabCaseEndpointNameFormatter.Instance.Consumer<PaymentNotAuthorized>()}
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:Genocs.MassTransit.Contracts:PaymentNotAuthorized"));

        await endpoint.Send<PaymentNotAuthorized>(new
        {
            PaymentOrderId = paymentOrderId,
            InVar.Timestamp
        });
        return Ok(paymentOrderId);
    }
}