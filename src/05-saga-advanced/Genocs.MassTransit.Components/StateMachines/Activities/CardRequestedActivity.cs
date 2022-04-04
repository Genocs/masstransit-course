using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.StateMachines.Activities
{
    public class CardRequestedActivity :
        IStateMachineActivity<OrderState, CardRequested>
    {

        private readonly ILogger<CardRequestedActivity> _logger;

        public CardRequestedActivity(ILogger<CardRequestedActivity> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("card-request");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, CardRequested> context, IBehavior<OrderState, CardRequested> next)
        {
            _logger.LogInformation("Executing, CardRequestedActivity. Order is {0}", context.Message.OrderId);

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

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, CardRequested, TException> context, IBehavior<OrderState, CardRequested> next) where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
