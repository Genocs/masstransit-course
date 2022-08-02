using System;

namespace Genocs.MassTransit.Contracts
{
    public interface FulfillOrder
    {
        Guid OrderId { get; }

        string CustomerNumber { get; }
        string ShippingAddress { get; }
        string PaymentCardNumber { get; }
        string Currency { get; }
    }
}
