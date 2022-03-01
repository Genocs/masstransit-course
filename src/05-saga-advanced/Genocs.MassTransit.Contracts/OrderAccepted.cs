using System;

namespace Genocs.MassTransit.Contracts
{
    public interface OrderAccepted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
    }
}
