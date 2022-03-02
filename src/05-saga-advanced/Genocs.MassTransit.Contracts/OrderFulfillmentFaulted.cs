using System;

namespace Genocs.MassTransit.Contracts
{
    public interface OrderFulfillmentFaulted
    {
        Guid OrderId { get; }
    }
}
