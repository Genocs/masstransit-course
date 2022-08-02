using Genocs.MassTransit.Contracts;
using MassTransit;
using MassTransit.Courier.Contracts;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.Consumers
{
    public class FulfillOrderConsumer :
        IConsumer<FulfillOrder>
    {
        public async Task Consume(ConsumeContext<FulfillOrder> context)
        {
            if (context.Message.CustomerNumber.StartsWith("INVALID"))
            {
                throw new InvalidOperationException("FulfillOrder got error because of an invalid customer");
            }

            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            // Add the activities for the Routing slip Consumer

            // 1. Allocate Inventory Activity
            //{ KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}
            builder.AddActivity("AllocateInventory", new Uri("exchange:allocate-inventory_execute"), new
                {
                    ItemNumber = "ITEM123",
                    Quantity = 10.0m
                });

            // 2. Payment Activity
            builder.AddActivity("Payment", new Uri("exchange:payment_execute"),
                new
                {
                    CardNumber = context.Message.PaymentCardNumber ?? "5999-1234-5678-9012",
                    Amount = 1.99m
                });

            // 3. Delivery Order Activity
            builder.AddActivity("DeliveryOrder", new Uri("exchange:delivery-order_execute"),
                new
                {
                    ShippingAddress = context.Message.ShippingAddress ?? "Via",
                });

            // Add the variable, so it can be accessible into the Messages
            builder.AddVariable("OrderId", context.Message.OrderId);
            builder.AddVariable("CustomerNumber", context.Message.CustomerNumber);

            await builder.AddSubscription(context.SourceAddress,
                RoutingSlipEvents.Faulted | RoutingSlipEvents.Supplemental,
                RoutingSlipEventContents.None, x => x.Send<OrderFulfillmentFaulted>(new { context.Message.OrderId }));

            await builder.AddSubscription(context.SourceAddress,
                RoutingSlipEvents.Completed | RoutingSlipEvents.Supplemental,
                RoutingSlipEventContents.None, x => x.Send<OrderFulfillmentCompleted>(new { context.Message.OrderId }));

            var routingSlip = builder.Build();

            await context.Execute(routingSlip);
        }
    }
}
