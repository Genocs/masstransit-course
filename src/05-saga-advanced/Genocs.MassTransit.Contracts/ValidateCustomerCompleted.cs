using System;

namespace Genocs.MassTransit.Contracts
{
    public interface ValidateCustomerCompleted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
    }
}
