using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.Consumers
{
    public class SubmitOrderWithResponseConsumer :
           IConsumer<SubmitOrderWithResponse>
    {
        readonly ILogger<SubmitOrderWithResponseConsumer> _logger;

        public SubmitOrderWithResponseConsumer()
        {
        }

        public SubmitOrderWithResponseConsumer(ILogger<SubmitOrderWithResponseConsumer> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<SubmitOrderWithResponse> context)
        {
            _logger?.Log(LogLevel.Debug, "SubmitOrderWithResponse: {CustomerNumber}", context.Message.CustomerNumber);

            if (context.Message.CustomerNumber.Contains("TEST"))
            {
                if (context.RequestId != null)
                {

                    await context.RespondAsync<OrderSubmissionRejected>(new
                    {
                        InVar.Timestamp,
                        context.Message.OrderId,
                        context.Message.CustomerNumber,
                        Reason = $"Test Customer cannot submit orders: {context.Message.CustomerNumber}"
                    });
                }

                return;
            }

            if (context.RequestId != null)
            {
                await context.RespondAsync<OrderSubmissionAccepted>(new
                {
                    InVar.Timestamp,
                    context.Message.OrderId,
                    context.Message.CustomerNumber
                });
            }
        }
    }
}
