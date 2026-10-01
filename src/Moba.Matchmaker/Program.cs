
using Moba.Matchmaker;
using Moba.Matchmaker.Core;
using Moba.Matchmaker.Entities;
using Moba.Shared.MatchmakerLibs;
using NATS.Client.Core;
using NATS.Client.Serializers.Json;
using NATS.Net;

var opts = NatsOpts.Default with { SerializerRegistry = NatsJsonSerializerRegistry.Default};
await using var nats = new NatsClient(opts);

MatchmakingManager manager = new MatchmakingManager();
MatchmakingEngine engine = new MatchmakingEngine(100, 10);
MatchmakingLoop loop = new MatchmakingLoop(manager, engine);

string ver = "pre-alpha:0.0.1";

loop.Start();

Console.WriteLine("=================== MATCHMAKER WORKS ====================");
Console.WriteLine($"= Version:{ver} =");

await foreach(var msg in nats.SubscribeAsync<EnqueuePlayerMessage>("matchmaking.requests"))
{
    var packet = msg.Data;

    manager.AddPlayer(new PoolPlayer(packet.PlayerId, packet.MMR), packet.Mode);
}