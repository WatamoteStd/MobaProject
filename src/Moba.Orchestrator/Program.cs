using System.Threading.Channels;
using Core;
using Moba.Shared.OrchestratorData;
using NATS.Client.Core;
using NATS.Client.Serializers.Json;
using NATS.Net;
using Process;


using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, eventArgs) =>
{
    Console.WriteLine("\n[Orchestrator] Stopping...");
    eventArgs.Cancel = true; 
    cts.Cancel(); 
};

PortManager portManager = new PortManager(10, 5000);
var channel = Channel.CreateUnbounded<MatchCreateInfo>(new UnboundedChannelOptions
{
    SingleReader = true,
    SingleWriter = true
});

var worker = new ProcessWorker(portManager, channel.Reader);
Task workerTask = Task.Run(() => worker.StartAsync(cts.Token));


// OBV connection with NATS (nuts ;)
var opts = NatsOpts.Default with { SerializerRegistry = NatsJsonSerializerRegistry.Default };
await using var nats = new NatsClient(opts);


// Start of the program

Console.WriteLine($"================= ORCHESTRATOR STARTET =================");
portManager.ShowPortInfo();

while(!cts.Token.IsCancellationRequested)
{
    
    try
    {
        Console.WriteLine("NATS connecting...");
        
        await nats.ConnectAsync();

        Console.WriteLine("[NATS] Connected successfully. Subscribed to 'matchmaking.created'.");

        await foreach(var msg in nats.SubscribeAsync<MatchCreateInfo>("matchmaking.created"))
        {
            var packet = msg.Data;
            await channel.Writer.WriteAsync(packet);
        }
    }
    catch (OperationCanceledException)
    {
        break;
    }
    catch(NatsException)
    {
        Console.WriteLine($"[NATS] Failed to connect. Retrying in 5 sec...");
        await Task.Delay(5000);
    }

}

channel.Writer.Complete();

await workerTask;

Console.WriteLine("[Orchestrator] Stopped successfully.");