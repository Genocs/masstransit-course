using System;

namespace Genocs.MassTransit.Contracts
{
    public interface OrderRequested
    {
        Guid OrderId { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
        DateTime Timestamp { get; }
    }
}
