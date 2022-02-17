using System;

namespace Genocs.MassTransit.Contracts
{
    public interface OrderSubmissionAccepted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
        string Notes { get; }
    }
}
