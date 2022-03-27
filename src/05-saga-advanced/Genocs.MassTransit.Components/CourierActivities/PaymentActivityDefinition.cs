using MassTransit.Definition;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public class PaymentActivityDefinition :
        ActivityDefinition<PaymentActivity, PaymentArguments, PaymentLog>
    {
        public PaymentActivityDefinition()
        {
            ConcurrentMessageLimit = 20;
        }
    }
}
