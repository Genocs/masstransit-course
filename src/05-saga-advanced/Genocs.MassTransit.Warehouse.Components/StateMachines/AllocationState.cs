using MassTransit;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Genocs.MassTransit.Warehouse.Components.StateMachines
{
    public class AllocationState :
        SagaStateMachineInstance,
        ISagaVersion
    {
        [BsonId]
        public Guid CorrelationId { get; set; }

        public string? CurrentState { get; set; }

        public Guid? HoldDurationToken { get; set; }

        public int Version { get; set; }

    }
}
