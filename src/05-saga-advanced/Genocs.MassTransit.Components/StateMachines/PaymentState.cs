using MassTransit;
using System;

namespace Genocs.MassTransit.Components.StateMachines
{
    public class PaymentState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }

        public string CurrentState { get; set; }
        public int ReadyEventStatus { get; set; }

        /// <summary>
        /// This is the Original Order Id that start the payment
        /// </summary>
        public Guid OrderId { get; set; }
        public string CustomerNumber { get; set; }
        public string PaymentCardNumber { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
