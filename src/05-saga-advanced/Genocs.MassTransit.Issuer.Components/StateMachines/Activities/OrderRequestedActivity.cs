using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.StateMachines.Activities
{
    public class OrderRequestedActivity :
        IStateMachineActivity<OrderState, OrderRequested>
    {

        private readonly ILogger<OrderRequestedActivity> _logger;

        public OrderRequestedActivity(ILogger<OrderRequestedActivity> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("order-request");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderRequested> context, IBehavior<OrderState, OrderRequested> next)
        {
            _logger.LogInformation("Executing, OrderRequestedActivity. Order is {0}", context.Message.OrderId);

            var consumeContext = context.GetPayload<ConsumeContext>();

            var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("queue:fulfill-order"));

            await sendEndpoint.Send<FulfillOrder>(new
            {
                context.Message.OrderId,
                context.Message.CustomerNumber,
                context.Message.ShippingAddress,
                context.Saga.PaymentCardNumber
            });

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderRequested, TException> context, IBehavior<OrderState, OrderRequested> next) where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
