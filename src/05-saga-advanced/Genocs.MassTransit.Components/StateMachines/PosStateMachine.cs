using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;

namespace Genocs.MassTransit.Components.StateMachines
{
    public class PosStateMachine : MassTransitStateMachine<PosState>
    {

        readonly ILogger<PosStateMachine> _logger;

        public PosStateMachine(ILogger<PosStateMachine> logger)
        {
            _logger = logger;

            // *****************************
            // Event Section
            Event(() => PaymentRequested, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => PaymentAccepted, x => x.CorrelateById(m => m.Message.OrderId));

            Event(() => PaymentInProgress, x => x.CorrelateById(m => m.Message.OrderId));

            Event(() => PaymentCaptured, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => PaymentAuthorized, x => x.CorrelateById(m => m.Message.OrderId));

            Event(() => PaymentNotCaptured, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => PaymentNotAuthorized, x => x.CorrelateById(m => m.Message.OrderId));

            Event(() => PaymentStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<PaymentNotFound>(new { context.Message.OrderId });
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
                When(PaymentRequested)
                    .Then(context =>
                    {
                        _logger.Log(LogLevel.Debug, "PaymentRequested: {CustomerNumber}", context.Message.CustomerNumber);
                        context.Saga.CustomerNumber = context.Message.CustomerNumber;
                        context.Saga.PaymentCardNumber = context.Message.PaymentCardNumber;
                        context.Saga.LastUpdate = DateTime.UtcNow;
                    })
                    .TransitionTo(Submitted)
                );

            During(Submitted,
                Ignore(PaymentRequested),
                When(PaymentAccepted)
                    //.Activity(x => x.OfType<AcceptOrderActivity>())
                    .TransitionTo(Accepted));

            During(Accepted,
                When(PaymentInProgress)
                    //.Activity(x => x.OfType<AcceptOrderActivity>())
                    .TransitionTo(InProgress));

            During(InProgress,
                 Ignore(PaymentInProgress));

            During(Captured,
                 Ignore(PaymentInProgress),
                 When(PaymentAuthorized)
                     .Then(context =>
                     {
                         context.Publish<PaymentCompleted>(new
                         {
                             context.Saga.PaymentCardNumber
                         });
                     })
                     .TransitionTo(Completed));

            During(Authorized,
                 Ignore(PaymentInProgress),
                 When(PaymentCaptured)
                     .Then(context =>
                     {
                         context.Publish<PaymentCompleted>(new
                         {
                             context.Saga.PaymentCardNumber
                         });
                     })
                     .TransitionTo(Completed));

            During(InProgress,
                When(PaymentCaptured)
                    .TransitionTo(Captured),
                When(PaymentAuthorized)
                    .TransitionTo(Authorized),
                When(PaymentNotCaptured)
                    .TransitionTo(Faulted),
                When(PaymentNotAuthorized)
                    .TransitionTo(Faulted));

            //CompositeEvent(() => PaymentReady, x => x.ReadyEventStatus, PaymentCaptured, PaymentAuthorized);

            //DuringAny(
            //    When(PaymentReady)
            //        .Then(context =>
            //        {
            //            context.Publish<PaymentCompleted>(new
            //            {
            //                context.Saga.PaymentCardNumber
            //            });
            //            _logger.LogInformation("Order Ready: {0}", context.Saga.CorrelationId);
            //        }));

            DuringAny(
                When(PaymentStatusRequested)
                    .RespondAsync(x => x.Init<PaymentStatus>(new
                    {
                        OrderId = x.Saga.CorrelationId,
                        CustomerNumber = x.Saga.CustomerNumber,
                        Status = x.Saga.CurrentState,
                        ReadyStatus = x.Saga.ReadyEventStatus
                    }))
                );

            SetCompletedWhenFinalized();

        }

        public State Submitted { get; private set; }
        public State Accepted { get; private set; }

        public State InProgress { get; private set; }
        public State Captured { get; private set; }
        public State Authorized { get; private set; }

        public State Completed { get; private set; }
        public State Faulted { get; private set; }

        public Event<PaymentRequested> PaymentRequested { get; private set; }
        public Event<PaymentAccepted> PaymentAccepted { get; private set; }

        public Event<PaymentProgress> PaymentInProgress { get; private set; }
        public Event<PaymentCaptured> PaymentCaptured{ get; private set; }
        public Event<PaymentAuthorized> PaymentAuthorized { get; private set; }

        public Event<PaymentNotCaptured> PaymentNotCaptured { get; private set; }
        public Event<PaymentNotAuthorized> PaymentNotAuthorized { get; private set; }

        public Event<PaymentStatus> PaymentStatusRequested { get; private set; }

        //public Event PaymentReady { get; private set; }
    }
}
