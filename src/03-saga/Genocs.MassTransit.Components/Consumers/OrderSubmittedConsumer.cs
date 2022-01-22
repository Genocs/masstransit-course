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
            _logger?.Log(LogLevel.Debug, "OrderSubmittedConsumer: {CustomerNumber}", context.Message.CustomerNumber);

            if (context.Message.CustomerNumber.Contains("TEST"))
            {
                //if (context.RequestId != null)
                //    await context.RespondAsync<OrderSubmissionRejected>(new
                //    {
                //        InVar.Timestamp,
                //        context.Message.OrderId,
                //        context.Message.CustomerNumber,
                //        Reason = $"Test Customer cannot submit orders: {context.Message.CustomerNumber}"
                //    });

                return;
            }

            //MessageData<string> notes = context.Message.Notes;
            //if (notes?.HasValue ?? false)
            //{
            //    string notesValue = await notes.Value;

            //    Console.WriteLine("NOTES: {0}", notesValue);
            //}

            //await context.Publish<OrderSubmitted>(new
            //{
            //    context.Message.OrderId,
            //    context.Message.Timestamp,
            //    context.Message.CustomerNumber,
            //    context.Message.PaymentCardNumber,
            //    context.Message.Notes
            //});

            //if (context.RequestId != null)
            //    await context.RespondAsync<OrderSubmissionAccepted>(new
            //    {
            //        InVar.Timestamp,
            //        context.Message.OrderId,
            //        context.Message.CustomerNumber
            //    });
        }
    }
}
