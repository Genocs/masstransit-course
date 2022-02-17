using System;

namespace Genocs.MassTransit.Contracts
{
    public interface OrderSubmissionRejected
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
        string Notes { get; }
    }
}
