namespace Genocs.MassTransit.Integrations.Service;

public class ConsoleHostedService
    : IHostedService
{

    public ConsoleHostedService(ILogger<ConsoleHostedService> logger)
    {
        logger.LogError("simple Error");
        // _bus = bus;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        //  await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
        //  return _bus.StopAsync(cancellationToken);
    }
}
