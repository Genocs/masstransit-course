using Grpc.Core;
using Helloworld;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Genocs.MassTransit.GRPC.Client.Service;

public class ConsoleHostedService
    : IHostedService
{
    private readonly ILogger _logger;
    private const string _serverUrl = "localhost";
    private const int _port = 30051;

    public ConsoleHostedService(ILogger<ConsoleHostedService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConsoleHostedService: StartAsync called");

        Channel channel = new Channel($"{_serverUrl}:{_port}", ChannelCredentials.Insecure);

        var client = new Greeter.GreeterClient(channel);
        string user = "you";
        _logger.LogInformation("Start processing 1000 messages!!!");

        for (int i = 0; i < 1000; i++)
        {
            var reply = client.SayHello(new HelloRequest { Name = user });
        }
        _logger.LogInformation("1000 messages processed !!!");

        await channel.ShutdownAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConsoleHostedService: StopAsync called");
        return Task.CompletedTask;
    }
}
