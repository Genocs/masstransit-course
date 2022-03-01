using Genocs.MassTransit.Components.Consumers;
using Genocs.MassTransit.Contracts;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;



namespace Genocs.MassTransit.UnitTests;

public class SubmitOrderUnitTest
{    
    public SubmitOrderUnitTest(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
        // add the xunit test output sink to the serilog logger
        // https://github.com/trbenning/serilog-sinks-xunit#serilog-sinks-xunit
        .WriteTo.TestOutput(output)
        .CreateLogger();

    }

    [Fact]
    public async Task CallSubmitOrderTest()
    {

        //var harness = new InMemoryTestHarness();

        //var consumer = harness.Consumer<SubmitOrderConsumer>();

        //await harness.Start();

        //try
        //{
        //    await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
        //    {
        //        OrderId = System.Guid.NewGuid(),
        //        CustomerNumber = "1234"
        //    });

        //    Assert.True(consumer.Consumed.Select<SubmitOrder>().Any());
        //}
        //finally
        //{
        //    await harness.Stop();
        //}
    }

    [Fact]
    public async Task CallSubmitOrderWithDependencyInjectionTest()
    {
        var provider = new ServiceCollection()
                                        .AddSingleton<ILoggerFactory, LoggerFactory>()
                                        .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                                        .AddMassTransitInMemoryTestHarness(cfg =>
                                        {
                                            cfg.AddConsumer<SubmitOrderWithResponseConsumer>();
                                            cfg.AddConsumerTestHarness<SubmitOrderWithResponseConsumer>(); 
                                        })
                                        .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<InMemoryTestHarness>();

        await harness.Start();


        try
        {
            var bus = provider.GetRequiredService<IBus>();

            IRequestClient<SubmitOrderWithResponse> client = bus.CreateRequestClient<SubmitOrderWithResponse>();

            await client.GetResponse<OrderSubmissionAccepted>(new
            {
                OrderId = System.Guid.NewGuid(),
                CustomerNumber = "1234"
            });

            Xunit.Assert.True(await harness.Consumed.Any<SubmitOrderWithResponse>());
            var consumerHarness = provider.GetRequiredService<IConsumerTestHarness<SubmitOrderWithResponseConsumer>>();

            Xunit.Assert.True(await consumerHarness.Consumed.Any<SubmitOrderWithResponse>());


            await harness.OutputTimeline(TestContext.Out, options => options.Now().IncludeAddress());
        }
        catch   
        (System.Exception ex)
        {

        }
        finally
        {
            await harness.Stop();

            await provider.DisposeAsync();
        }
    }
}
