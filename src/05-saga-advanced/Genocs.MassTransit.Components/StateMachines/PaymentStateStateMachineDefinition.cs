using MassTransit;

namespace Genocs.MassTransit.Components.StateMachines
{
    public class PaymentStateStateMachineDefinition : SagaDefinition<PaymentState>
    {
        public PaymentStateStateMachineDefinition()
        {
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<PaymentState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000, 10000));
            base.ConfigureSaga(endpointConfigurator, sagaConfigurator);
        }
    }
}
