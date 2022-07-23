using System;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public interface PaymentLog
    {
        string AuthorizationCode { get; }
    }
}
