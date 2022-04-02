using MassTransit.Definition;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public class IssueCardActivityDefinition :
        ActivityDefinition<IssueCardActivity, IssueCardArguments, IssueCardLog>
    {
        public IssueCardActivityDefinition()
        {
            ConcurrentMessageLimit = 20;
        }
    }
}
