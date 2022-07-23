using System;

namespace Genocs.MassTransit.Contracts
{
    public interface PaymentAuthorized
    {
        Guid PaymentOrderId { get; }
        string PaymentCardNumber { get; }
        DateTime Timestamp { get; }
    }
}
