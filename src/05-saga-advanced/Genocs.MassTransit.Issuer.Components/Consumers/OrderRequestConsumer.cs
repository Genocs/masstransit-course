using Genocs.MassTransit.Components.HttpClients;
using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.Consumers
{
    public class OrderRequestConsumer :
        IConsumer<OrderRequest>
    {
        readonly ILogger<OrderRequestConsumer> _logger;
        private readonly CustomerClient customerClient;

        public OrderRequestConsumer()
        {
        }

        public OrderRequestConsumer(ILogger<OrderRequestConsumer> logger, CustomerClient customerClient)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.customerClient = customerClient ?? throw new System.ArgumentNullException(nameof(customerClient));
        }

        public async Task Consume(ConsumeContext<OrderRequest> context)
        {
            _logger?.Log(LogLevel.Debug, "SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);

            // Customer Validation
            var customer = await this.customerClient.GetCustomer(context.Message.CustomerNumber);
            if (!customer.Active)
            {
                if (context.RequestId != null)
                {
                    await context.RespondAsync<OrderRejected>(new
                    {
                        InVar.Timestamp,
                        context.Message.OrderId,
                        context.Message.CustomerNumber,
                        Reason = $"Inactive customer cannot request order. CustomerNumber: {context.Message.CustomerNumber}"
                    });
                }

                return;
            }

            await context.Publish<OrderRequested>(new
            {
                context.Message.OrderId,
                context.Message.Timestamp,
                context.Message.CustomerNumber,
                context.Message.ShippingAddress,
                context.Message.PaymentCardNumber,
            });

            if (context.RequestId != null)
            {
                //await context.RespondAsync<OrderAccepted>(new
                //{
                //    InVar.Timestamp,
                //    context.Message.OrderId,
                //    context.Message.CustomerNumber
                //});
            }
        }
    }
}
