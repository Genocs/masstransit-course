using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;

namespace Genocs.MassTransit.Components.StateMachines
{
    public class PaymentStateStateMachine : MassTransitStateMachine<PaymentState>
    {

        readonly ILogger<PaymentStateStateMachine> _logger;

        public PaymentStateStateMachine(ILogger<PaymentStateStateMachine> logger)
        {
            _logger = logger;

            // *****************************
            // Event Section
            Event(() => PaymentRequested, x => x.CorrelateById(m => m.Message.PaymentOrderId));

            Event(() => PaymentInProgress, x => x.CorrelateById(m => m.Message.PaymentOrderId));

            Event(() => PaymentCaptured, x => x.CorrelateById(m => m.Message.PaymentOrderId));
            Event(() => PaymentAuthorized, x => x.CorrelateById(m => m.Message.PaymentOrderId));

            Event(() => PaymentNotCaptured, x => x.CorrelateById(m => m.Message.PaymentOrderId));
            Event(() => PaymentNotAuthorized, x => x.CorrelateById(m => m.Message.PaymentOrderId));

            Event(() => PaymentCompleted, x => x.CorrelateById(m => m.Message.PaymentOrderId));

            Event(() => PaymentStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.PaymentOrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<PaymentNotFound>(new { context.Message.PaymentOrderId });
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
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.PaymentCardNumber = context.Message.PaymentCardNumber;
                        context.Saga.LastUpdate = DateTime.UtcNow;
                    })
                    .TransitionTo(Accepted)
                );


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
                     }));

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
                        PaymentOrderId = x.Saga.CorrelationId,
                        OrderId = x.Saga.CustomerNumber,
                        Status = x.Saga.CurrentState,
                        ReadyStatus = x.Saga.ReadyEventStatus
                    }))
                );

            SetCompletedWhenFinalized();

        }

        public State Accepted { get; private set; }

        public State InProgress { get; private set; }
        public State Captured { get; private set; }
        public State Authorized { get; private set; }

        public State Completed { get; private set; }
        public State Faulted { get; private set; }

        public Event<PaymentRequested> PaymentRequested { get; private set; }
        public Event<PaymentProgress> PaymentInProgress { get; private set; }
        public Event<PaymentCaptured> PaymentCaptured{ get; private set; }
        public Event<PaymentAuthorized> PaymentAuthorized { get; private set; }

        public Event<PaymentNotCaptured> PaymentNotCaptured { get; private set; }
        public Event<PaymentNotAuthorized> PaymentNotAuthorized { get; private set; }

        public Event<PaymentStatus> PaymentStatusRequested { get; private set; }
        public Event<PaymentCompleted> PaymentCompleted { get; private set; }


        //public Event PaymentReady { get; private set; }
    }
}
