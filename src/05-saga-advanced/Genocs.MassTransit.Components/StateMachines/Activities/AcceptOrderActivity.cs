using Automatonymous;
using Genocs.MassTransit.Contracts;
using GreenPipes;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.StateMachines.Activities
{
    public class AcceptOrderActivity :
        Activity<OrderState, OrderAccepted>
    {
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
            Console.WriteLine("Executing, AcceptOrderActivity. Order is {0}", context.Data.OrderId);

            var consumeContext = context.GetPayload<ConsumeContext>();

            var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("queue:fulfill-order"));

            //await sendEndpoint.Send<FulfillOrder>(new
            //{
            //    context.Data.OrderId,
            //    context.Instance.CustomerNumber,
            //    context.Instance.PaymentCardNumber,
            //});

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context, Behavior<OrderState, OrderAccepted> next) where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
