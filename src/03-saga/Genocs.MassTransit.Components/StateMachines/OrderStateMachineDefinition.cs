using GreenPipes;
using MassTransit;
using MassTransit.Definition;

namespace Genocs.MassTransit.Components.StateMachines
{
    public class OrderStateMachineDefinition : SagaDefinition<OrderState>
    {
        public OrderStateMachineDefinition()
        {
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000, 15000));
            base.ConfigureSaga(endpointConfigurator, sagaConfigurator);
        }
    }
}
