using Genocs.MassTransit.Components.Consumers;
using Genocs.MassTransit.Contracts;
using MassTransit.Testing;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Genocs.MassTransit.UnitTests;

public class SubmitOrderUnitTest
{
    [Fact]
    public async Task CallSubmitOrderTest()
    {
        var harness = new InMemoryTestHarness();

        var consumer = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();

        try
        {
            await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
            {
                OrderId = System.Guid.NewGuid(),
                CustomerNumber = "1234"
            });

            Assert.True(consumer.Consumed.Select<SubmitOrder>().Any());
        }
        finally
        {
            await harness.Stop();
        }
    }
}
