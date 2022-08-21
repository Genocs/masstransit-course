// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Helloworld;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

const int Port = 30051;


Server server = new Server
{
    Services = { Greeter.BindService(new GreeterImpl()) },
    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
};
server.Start();

Log.Logger.Information("Greeter server listening on port " + Port);
Log.Logger.Information("Press any key to stop the server...");

Console.ReadKey();

server.ShutdownAsync().Wait();

Log.CloseAndFlush();

