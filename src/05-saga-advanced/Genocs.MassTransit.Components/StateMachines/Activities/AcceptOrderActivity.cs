using Automatonymous;
using Genocs.MassTransit.Contracts;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.StateMachines.Activities
{
    public class AcceptOrderActivity :
        Activity<OrderState, OrderAccepted>
    {

        private readonly ILogger<AcceptOrderActivity> _logger;

        public AcceptOrderActivity(ILogger<AcceptOrderActivity> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("accept-order");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderAccepted> context, Behavior<OrderState, OrderAccepted> next)
        {
            _logger.LogInformation("Executing, AcceptOrderActivity. Order is {0}", context.Data.OrderId);

            var consumeContext = context.GetPayload<ConsumeContext>();

            var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("queue:fulfill-order"));

            await sendEndpoint.Send<FulfillOrder>(new
            {
                context.Data.OrderId,
                context.Instance.CustomerNumber,
                PaymentCardNumber = "123456789",  
            });

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context, Behavior<OrderState, OrderAccepted> next) where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
