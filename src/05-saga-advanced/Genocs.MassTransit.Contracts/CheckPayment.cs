using System;

namespace Genocs.MassTransit.Contracts
{
    public interface CheckPayment
    {
        Guid OrderId { get; }
    }

    public interface PaymentStatus
    {
        Guid OrderId { get; }
        string CustomerNumber { get; }
        string Status { get; }
    }

    public interface PaymentNotFound
    {
        Guid OrderId { get; }
    }
}
