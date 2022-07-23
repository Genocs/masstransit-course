using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.Consumers
{
    public class PaymentCompletedConsumer :
        IConsumer<PaymentCompleted>
    {
        readonly ILogger<PaymentCompletedConsumer> _logger;

        public PaymentCompletedConsumer()
        {
        }

        public PaymentCompletedConsumer(ILogger<PaymentCompletedConsumer> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<PaymentCompleted> context)
        {
            _logger?.Log(LogLevel.Debug, "PaymentCompleted: {PaymentCardNumber}", context.Message.PaymentCardNumber);
           
            await Task.CompletedTask;
        }
    }
}
