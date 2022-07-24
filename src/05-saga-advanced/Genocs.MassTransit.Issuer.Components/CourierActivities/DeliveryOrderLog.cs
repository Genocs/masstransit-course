using System;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public interface DeliveryOrderLog
    {
        string AuthorizationCode { get; }
    }
}
