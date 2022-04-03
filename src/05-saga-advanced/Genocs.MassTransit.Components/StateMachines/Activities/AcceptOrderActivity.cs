using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.StateMachines.Activities
{
    public class AcceptOrderActivity :
        IStateMachineActivity<OrderState, OrderAccepted>
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


        public async Task Execute(BehaviorContext<OrderState, OrderAccepted> context, IBehavior<OrderState, OrderAccepted> next)
        {
            _logger.LogInformation("Executing, AcceptOrderActivity. Order is {0}", context.Message.OrderId);

            var consumeContext = context.GetPayload<ConsumeContext>();

            var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("queue:fulfill-order"));

            await sendEndpoint.Send<FulfillOrder>(new
            {
                context.Message.OrderId,
                context.Message.CustomerNumber,
                context.Saga.PaymentCardNumber
            });

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context, IBehavior<OrderState, OrderAccepted> next) where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
