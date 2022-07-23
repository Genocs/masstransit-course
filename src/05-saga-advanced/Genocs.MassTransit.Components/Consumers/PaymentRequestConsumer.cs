using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.Consumers
{
    public class PaymentRequestConsumer :
        IConsumer<PaymentRequest>
    {
        readonly ILogger<PaymentRequestConsumer> _logger;

        public PaymentRequestConsumer()
        {
        }

        public PaymentRequestConsumer(ILogger<PaymentRequestConsumer> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<PaymentRequest> context)
        {
            _logger?.Log(LogLevel.Debug, "SubmitOrderConsumer: {PaymentCardNumber}", context.Message.PaymentCardNumber);

            // Customer Validation
            if (context.RequestId != null)
            {
                await context.RespondAsync<PaymentRejected>(new
                {
                    InVar.Timestamp,
                    context.Message.PaymentOrderId,
                    context.Message.PaymentCardNumber,
                    Reason = $"Inactive customer cannot request order. PaymentCardNumber: {context.Message.PaymentCardNumber}"
                });
            }

            await context.Publish<PaymentRequested>(new
            {
                context.Message.PaymentOrderId,
                context.Message.OrderId,
                context.Message.CustomerNumber,
                context.Message.Timestamp,
            });

            if (context.RequestId != null)
            {
                //await context.RespondAsync<PaymentAccepted>(new
                //{
                //    InVar.Timestamp,
                //    context.Message.OrderId,
                //    context.Message.PaymentCardNumber
                //});
            }
        }
    }
}
