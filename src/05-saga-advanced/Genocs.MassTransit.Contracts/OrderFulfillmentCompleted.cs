using System;

namespace Genocs.MassTransit.Contracts
{
    public interface OrderFulfillmentCompleted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
    }
}
