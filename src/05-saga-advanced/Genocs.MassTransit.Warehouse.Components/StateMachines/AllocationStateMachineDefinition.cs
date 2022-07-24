using MassTransit;

namespace Genocs.MassTransit.Warehouse.Components.StateMachines
{
    public class AllocationStateMachineDefinition :
        SagaDefinition<AllocationState>
    {
        public AllocationStateMachineDefinition()
        {
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<AllocationState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
