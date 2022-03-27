using System;

namespace Genocs.MassTransit.Warehouse.Contracts
{
    public interface AllocationHoldDurationExpired
    {
        Guid AllocationId { get; }
    }
}
