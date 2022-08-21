using Grpc.Core;
using Helloworld;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Genocs.MassTransit.GRPC.Server.Service;

public class ConsoleHostedService
    : IHostedService
{
    private readonly ILogger _logger;

    private const string _serverUrl = "localhost";
    private const int _port = 30051;

    private Grpc.Core.Server? _server;

    public ConsoleHostedService(ILogger<ConsoleHostedService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _server = new Grpc.Core.Server
        {
            Services = { Greeter.BindService(new GreeterImpl()) },
            Ports = { new ServerPort(_serverUrl, _port, ServerCredentials.Insecure) }
        };

        _server.Start();
        _logger.LogInformation("Greeter server listening on port " + _port);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConsoleHostedService: StopAsync called");

        if (_server != null)
        {
            await _server.ShutdownAsync();
        }
    }
}
