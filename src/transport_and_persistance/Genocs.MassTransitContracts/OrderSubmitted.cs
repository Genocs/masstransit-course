using System;

namespace Genocs.MassTransitContracts
{
    public interface OrderSubmitted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
        string Notes { get; }
    }
}
