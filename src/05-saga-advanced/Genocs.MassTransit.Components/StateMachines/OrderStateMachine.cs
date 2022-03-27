using Automatonymous;
using Genocs.MassTransit.Components.StateMachines.Activities;
using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Genocs.MassTransit.Components.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {

        readonly ILogger<OrderStateMachine> _logger;

        public OrderStateMachine(ILogger<OrderStateMachine> logger)
        {
            _logger = logger;

            // *****************************
            // Event Section
            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderAccepted, x => x.CorrelateById(m => m.Message.OrderId));

            Event(() => FulfillmentCompleted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => FulfillmentFaulted, x => x.CorrelateById(m => m.Message.OrderId));

            Event(() => FulfillOrderFaulted, x => x.CorrelateById(m => m.Message.Message.OrderId));

            Event(() => OrderStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<OrderNotFound>(new { context.Message.OrderId });
                    }
                }
               ));
            });

            Event(() => AccountClosed, x => x.CorrelateBy((saga, context) => saga.CustomerNumber == context.Message.CustomerNumber));

            //Schedule(() => HoldExpiration, x => x.HoldDurationToken, s =>
            //{
            //    s.Delay = TimeSpan.FromHours(1);
            //    s.Received = x => x.CorrelateById(m => m.Message.AllocationId);
            //});

            // *****************************
            // State Section
            InstanceState(x => x.CurrentState);

            // *****************************
            // State Machine Section
            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        _logger.Log(LogLevel.Debug, "OrderSubmitted: {CustomerNumber}", context.Data.CustomerNumber);
                        context.Instance.CustomerNumber = context.Data.CustomerNumber;
                        context.Instance.LastUpdate = DateTime.UtcNow;
                    })
                    .TransitionTo(Submitted)

                );

            During(Submitted,
                Ignore(OrderSubmitted),
                When(OrderAccepted)
                    .Activity(x => x.OfType<AcceptOrderActivity>())
                    .TransitionTo(Accepted));


            During(Accepted,
                When(FulfillmentFaulted)
                    .Then(context => _logger.Log(LogLevel.Debug, "FulfillmentFaulted: {OrderId}", context.Data.OrderId))
                    .TransitionTo(Faulted),
                When(FulfillOrderFaulted)
                    .Then(context => _logger.Log(LogLevel.Error, "Fulfill Order Faulted: {0}", context.Data.Exceptions.FirstOrDefault()?.Message))
                    .TransitionTo(Faulted),
                When(FulfillmentCompleted)
                    .Then(context => _logger.Log(LogLevel.Debug, "FulfillmentCompleted: {OrderId}", context.Data.OrderId))
                    .TransitionTo(Completed)
                    .Finalize());

            DuringAny(
                When(OrderStatusRequested)
                    .RespondAsync(x => x.Init<OrderStatus>(new
                    {
                        OrderId = x.Instance.CorrelationId,
                        CustomerNumber = x.Instance.CustomerNumber,
                        Status = x.Instance.CurrentState,
                    }))
                );

            SetCompletedWhenFinalized();

        }

        public State Submitted { get; private set; }
        public State Accepted { get; private set; }

        public State Canceled { get; private set; }
        public State Completed { get; private set; }
        public State Faulted { get; private set; }

        public Event<OrderSubmitted> OrderSubmitted { get; private set; }
        public Event<OrderAccepted> OrderAccepted { get; private set; }

        public Event<CustomerAccountClosed> AccountClosed { get; private set; }

        public Event<OrderFulfillmentCompleted> FulfillmentCompleted { get; private set; }
        public Event<OrderFulfillmentFaulted> FulfillmentFaulted { get; private set; }
        public Event<Fault<FulfillOrder>> FulfillOrderFaulted { get; private set; }
        public Event<OrderStatus> OrderStatusRequested { get; private set; }

        //public Schedule<OrderState, AllocationHoldDurationExpired> HoldExpiration { get; set; }
    }
}
