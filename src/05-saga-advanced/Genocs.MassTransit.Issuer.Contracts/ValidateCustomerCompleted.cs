using System;

namespace Genocs.MassTransit.Contracts
{
    public interface ValidateCustomerCompleted
    {
        Guid OrderId { get; }
        string CustomerNumber { get; }
        DateTime Timestamp { get; }
    }
}
