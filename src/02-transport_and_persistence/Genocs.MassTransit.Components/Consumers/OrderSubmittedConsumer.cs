using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.Consumers
{
    public class OrderSubmittedConsumer :
           IConsumer<OrderSubmitted>
    {
        readonly ILogger<OrderSubmittedConsumer> _logger;

        public OrderSubmittedConsumer()
        {
        }

        public OrderSubmittedConsumer(ILogger<OrderSubmittedConsumer> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<OrderSubmitted> context)
        {
            _logger.Log(LogLevel.Debug, "OrderSubmittedConsumer: {CustomerNumber}", context.Message.CustomerNumber);
            await Task.CompletedTask;
        }
    }
}
