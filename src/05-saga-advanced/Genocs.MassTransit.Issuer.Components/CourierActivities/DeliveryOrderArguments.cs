using System;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public interface DeliveryOrderArguments
    {
        Guid OrderId { get; }
        string ShippingAddress { get; }
    }
}
