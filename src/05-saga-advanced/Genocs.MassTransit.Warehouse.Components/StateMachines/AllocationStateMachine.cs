using Genocs.MassTransit.Warehouse.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;

namespace Genocs.MassTransit.Warehouse.Components.StateMachines
{
    public class AllocationStateMachine :
        MassTransitStateMachine<AllocationState>
    {
        public AllocationStateMachine(ILogger<AllocationStateMachine> logger)
        {
            Event(() => AllocationCreated, x => x.CorrelateById(m => m.Message.AllocationId));
            Event(() => ReleaseRequested, x => x.CorrelateById(m => m.Message.AllocationId));
            Event(() => AllocationConfirmed, x => x.CorrelateById(m => m.Message.AllocationId));


            Schedule(() => HoldExpiration, x => x.HoldDurationToken, s =>
            {
                s.Delay = TimeSpan.FromSeconds(15);
                s.Received = x => x.CorrelateById(m => m.Message.AllocationId);
            });

            InstanceState(x => x.CurrentState);

            Initially(
                When(AllocationCreated)
                    .Schedule(HoldExpiration, context => context.Init<AllocationHoldDurationExpired>(new { context.Message.AllocationId }),
                        context => context.Message.HoldDuration)
                    .TransitionTo(Allocated),
                When(ReleaseRequested)
                    .TransitionTo(Released)
            );

            During(Allocated,
                When(AllocationCreated)
                    .Schedule(HoldExpiration, context => context.Init<AllocationHoldDurationExpired>(new { context.Message.AllocationId }),
                        context => context.Message.HoldDuration)
            );

            During(Released,
                When(AllocationCreated)
                    .Then(context => logger.LogInformation("Allocation already released: {AllocationId}", context.Saga.CorrelationId))
                    .Finalize()
            );

            During(Allocated,
                When(HoldExpiration.Received)
                    .Then(context => logger.LogInformation("Allocation expired {AllocationId}", context.Saga.CorrelationId))
                    .Finalize(),
                When(ReleaseRequested)
                    .Unschedule(HoldExpiration)
                    .Then(context => logger.LogInformation("Allocation Release Granted: {AllocationId}", context.Saga.CorrelationId))
                    .Finalize(),
                When(AllocationConfirmed)
                    .Unschedule(HoldExpiration)
                    .Then(context => logger.LogInformation("Allocation Completd: {AllocationId}", context.Saga.CorrelationId))
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }

        public Schedule<AllocationState, AllocationHoldDurationExpired> HoldExpiration { get; set; }

        public State Allocated { get; set; }
        public State Released { get; set; }

        public Event<AllocationCreated> AllocationCreated { get; set; }
        public Event<AllocationReleaseRequested> ReleaseRequested { get; set; }
        public Event<AllocationConfirmed> AllocationConfirmed { get; set; }

    }
}
