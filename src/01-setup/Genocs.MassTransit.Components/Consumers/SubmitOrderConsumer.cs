using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.Consumers
{
    public class SubmitOrderConsumer :
           IConsumer<SubmitOrder>
    {
        readonly ILogger<SubmitOrderConsumer> _logger;

        public SubmitOrderConsumer()
        {
        }

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger?.Log(LogLevel.Debug, "SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);

            await context.Publish<OrderSubmitted>(new
            {
                context.Message.OrderId,
                context.Message.Timestamp,
                context.Message.CustomerNumber,
                context.Message.PaymentCardNumber,
                context.Message.Notes
            });

            if (context.RequestId != null)
                await context.RespondAsync<OrderSubmitted>(new
                {
                    InVar.Timestamp,
                    context.Message.OrderId,
                    context.Message.CustomerNumber
                });
        }
    }
}
