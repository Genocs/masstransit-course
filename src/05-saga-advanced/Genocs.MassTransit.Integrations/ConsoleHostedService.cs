namespace Genocs.MassTransit.Integrations.Service;

public class ConsoleHostedService
    : IHostedService
{
    private readonly ILogger _logger;
    public ConsoleHostedService(ILogger<ConsoleHostedService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("ConsoleHostedService: logger is ok!");

        // _bus = bus;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConsoleHostedService: StartAsync called");
        await Task.CompletedTask;
        //  await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConsoleHostedService: StopAsync called");
        return Task.CompletedTask;
        //  return _bus.StopAsync(cancellationToken);
    }
}
