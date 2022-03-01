using Automatonymous;
using Genocs.MassTransit.Components.StateMachines.Activities;
using Genocs.MassTransit.Contracts;
using MassTransit;
using MassTransit.Saga;
using Microsoft.Extensions.Logging;
using System;

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

            DuringAny(
                When(OrderStatusRequested)
                    .RespondAsync(x => x.Init<OrderStatus>(new
                    {
                        OrderId = x.Instance.CorrelationId,
                        CustomerNumber = x.Instance.CustomerNumber,
                        Status = x.Instance.CurrentState,
                    }))
                );
        }

        public State Submitted { get; private set; }
        public State Accepted { get; private set; }

        public Event<OrderSubmitted> OrderSubmitted { get; private set; }
        public Event<OrderAccepted> OrderAccepted { get; private set; }

        public Event<OrderStatus> OrderStatusRequested { get; private set; }
    }

    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime LastUpdate { get; set; }
        public int Version { get; set; }
    }
}
