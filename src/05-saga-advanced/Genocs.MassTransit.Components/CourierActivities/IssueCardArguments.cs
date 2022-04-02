using System;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public interface IssueCardArguments
    {
        Guid OrderId { get; }
        decimal Amount { get; }
        string CardNumber { get; }
    }
}
