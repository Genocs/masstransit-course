using System;

namespace Genocs.MassTransit.Contracts
{
    public interface PaymentCaptured
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
    }
}
