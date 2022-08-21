// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Helloworld;

Channel channel = new Channel("127.0.0.1:30051", ChannelCredentials.Insecure);

var client = new Greeter.GreeterClient(channel);
String user = "you";

var reply = client.SayHello(new HelloRequest { Name = user });
Console.WriteLine("Greeting: " + reply.Message);

channel.ShutdownAsync().Wait();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();