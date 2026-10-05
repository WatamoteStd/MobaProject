
using Moba.Matchmaker;
using Moba.Matchmaker.Core;
using Moba.Matchmaker.Entities;
using Moba.Shared.MatchmakerLibs;
using Moba.Shared.MatchmakerLibs.MatchQueue;
using NATS.Client.Core;
using NATS.Client.Serializers.Json;
using NATS.Net;

var opts = NatsOpts.Default with { SerializerRegistry = NatsJsonSerializerRegistry.Default};
await using var nats = new NatsClient(opts);

MatchmakingManager manager = new MatchmakingManager();
MatchmakingEngine engine = new MatchmakingEngine(100, 10);
MatchmakingLoop loop = new MatchmakingLoop(manager, engine, nats);

string ver = "pre-alpha:0.0.1";

loop.Start();

Console.WriteLine("=================== MATCHMAKER WORKS ====================");
Console.WriteLine($"= Version:{ver} =");

await nats.ConnectAsync();
Console.WriteLine("\n[Matchmaker] Connected to NATS!");

Task taskEnqueue = Task.Run(async () =>
{
    await foreach(var msg in nats.SubscribeAsync<EnqueuePlayerMessage>("matchmaking.requests"))
    {
        var packet = msg.Data;
        manager.AddPlayer(new PoolPlayer(packet.PlayerId, packet.MMR), packet.Mode);
    }
});

Task taskStatus = Task.Run(async () =>
{
    await foreach(var msg in nats.SubscribeAsync<long>("matchmaker.player.status"))
    {

        long userId = msg.Data;

        bool isFound = manager.GetPlayer(userId, out var player);
        Console.WriteLine($"[NATS: Task Status] new request from Id:{userId}. IsFound:{isFound}");

        QueuePlayerStatusResponse response;

        if (isFound)
        {
            response = new QueuePlayerStatusResponse(QueuePlayerStatus.Search, string.Empty, 0);
        }
        else
        {
            response = new QueuePlayerStatusResponse(QueuePlayerStatus.NotFound, string.Empty, 0);
        }

        await msg.ReplyAsync(response);

        Console.WriteLine($"[NATS: Task Stratus] Reply successfully send to MasterServer!");

    }
});

await Task.WhenAll(taskEnqueue, taskStatus);