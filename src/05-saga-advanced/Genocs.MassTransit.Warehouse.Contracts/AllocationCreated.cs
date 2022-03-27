using System;

namespace Genocs.MassTransit.Warehouse.Contracts
{
    public interface AllocationCreated
    {
        Guid AllocationId { get; }
        TimeSpan HoldDuration { get; }
    }
}
