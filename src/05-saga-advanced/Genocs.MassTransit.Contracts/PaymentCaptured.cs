using System;

namespace Genocs.MassTransit.Contracts
{
    public interface PaymentCaptured
    {
        Guid PaymentOrderId { get; }
        string PaymentCardNumber { get; }
        DateTime Timestamp { get; }
    }
}
