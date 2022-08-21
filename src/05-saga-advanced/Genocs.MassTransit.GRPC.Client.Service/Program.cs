// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Helloworld;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();


Channel channel = new Channel("127.0.0.1:30051", ChannelCredentials.Insecure);

var client = new Greeter.GreeterClient(channel);
string user = "you";

var reply = client.SayHello(new HelloRequest { Name = user });

Log.Logger.Information("Greeting: " + reply.Message);

channel.ShutdownAsync().Wait();
Log.Logger.Information("Press any key to exit...");

Console.ReadKey();

Log.CloseAndFlush();