using Genocs.MassTransit.Integrations.Contracts;
using MassTransit;

namespace Genocs.MassTransit.Integrations.Service.Consumers
{
    public class SettlementSubmittedConsumer : IConsumer<SettlementSubmitted>
    {
        private readonly ILogger<SettlementSubmitted> _logger;
        public SettlementSubmittedConsumer(ILogger<SettlementSubmitted> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<SettlementSubmitted> context)
        {
            _logger.LogInformation($"Received Settlement Submitted. Code: {context.Message.Code}");
            await Task.CompletedTask;
        }
    }
}
