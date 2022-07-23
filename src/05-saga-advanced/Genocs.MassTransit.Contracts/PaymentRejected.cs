using System;

namespace Genocs.MassTransit.Contracts
{
    public interface PaymentRejected
    {
        Guid PaymentOrderId { get; }
        string PaymentCardNumber { get; }
        string Reason { get; }
        DateTime Timestamp { get; }
    }
}
