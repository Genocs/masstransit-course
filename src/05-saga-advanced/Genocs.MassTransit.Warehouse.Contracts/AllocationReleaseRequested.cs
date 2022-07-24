using System;

namespace Genocs.MassTransit.Warehouse.Contracts
{
    public interface AllocationReleaseRequested
    {
        Guid AllocationId { get; }
        string Reason { get; }
    }
}
