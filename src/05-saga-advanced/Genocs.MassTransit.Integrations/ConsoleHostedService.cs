using Genocs.MassTransit.Integrations.Contracts;
using MassTransit;

namespace Genocs.MassTransit.Integrations.Service;

public class ConsoleHostedService
    : IHostedService
{
    private readonly ILogger _logger;
    private readonly IBus _bus;

    public ConsoleHostedService(ILogger<ConsoleHostedService> logger, IBus bus)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Command sent to the bus (in case on Azure Service Bus)
        _logger.LogInformation("ConsoleHostedService: StartAsync called");
        await _bus.Publish<SettlementSubmitted>(new {
            Id = Guid.NewGuid().ToString(),
            Code = $"Tag_{Guid.NewGuid()}",
            ProcessedTimestamp = DateTime.UtcNow
        });
        //  await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConsoleHostedService: StopAsync called");
        return Task.CompletedTask;
        //  return _bus.StopAsync(cancellationToken);
    }
}
