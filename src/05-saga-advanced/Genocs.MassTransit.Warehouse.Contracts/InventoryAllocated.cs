using System;

namespace Genocs.MassTransit.Warehouse.Contracts
{
    public interface InventoryAllocated
    {
        Guid AllocationId { get; }
        string ItemNumber { get; }
        decimal Quantity { get; }
    }
}
