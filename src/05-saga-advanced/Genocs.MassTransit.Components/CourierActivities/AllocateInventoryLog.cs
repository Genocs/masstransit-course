using System;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public interface AllocateInventoryLog
    {
        Guid AllocationId { get; }
    }
}
