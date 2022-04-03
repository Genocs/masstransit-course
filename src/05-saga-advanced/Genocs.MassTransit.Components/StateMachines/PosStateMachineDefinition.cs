using MassTransit;

namespace Genocs.MassTransit.Components.StateMachines
{
    public class PosStateMachineDefinition : SagaDefinition<PosState>
    {
        public PosStateMachineDefinition()
        {
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<PosState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000, 10000));
            base.ConfigureSaga(endpointConfigurator, sagaConfigurator);
        }
    }
}
