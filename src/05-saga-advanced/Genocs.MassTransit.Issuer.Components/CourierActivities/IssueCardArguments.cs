using System;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public interface IssueCardArguments
    {
        Guid OrderId { get; }
        string Currency { get; }
    }
}
