using System;

namespace Genocs.MassTransit.Warehouse.Contracts
{
    public interface AllocationConfirmed
    {
        Guid AllocationId { get; }
    }
}
