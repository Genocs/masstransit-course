using System;

namespace Genocs.MassTransit.Warehouse.Contracts
{
    public interface AllocateInventory
    {
        Guid AllocationId { get; }
        string ItemNumber { get; }
        decimal Quantity { get; }
    }
}
