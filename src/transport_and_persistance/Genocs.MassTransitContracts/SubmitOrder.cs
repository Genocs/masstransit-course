using System;

namespace Genocs.MassTransitContracts
{
    public interface SubmitOrder
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
        string Notes { get; }
    }
}
