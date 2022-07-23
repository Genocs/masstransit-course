using System;

namespace Genocs.MassTransit.Contracts
{
    public interface CheckOrder
    {
        Guid OrderId { get; }
    }

    public interface OrderStatus
    {
        Guid OrderId { get; }
        string CustomerNumber { get; }
        string Status { get; }
    }

    public interface OrderNotFound
    {
        Guid OrderId { get; }
    }
}
