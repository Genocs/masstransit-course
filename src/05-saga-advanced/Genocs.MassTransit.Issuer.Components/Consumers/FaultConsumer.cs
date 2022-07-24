using Genocs.MassTransit.Contracts;
using MassTransit;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.Consumers
{
    public class FaultConsumer :
            IConsumer<Fault<FulfillOrder>>
    {
        public Task Consume(ConsumeContext<Fault<FulfillOrder>> context)
        {
            return Task.CompletedTask;
        }
    }
}
