using System.Net.Sockets;
using Microsoft.Extensions.Caching.Memory;
using Moba.Shared.MatchmakerLibs.MatchQueue;
using Moba.Shared.OrchestratorData;
using NATS.Client.Core;
using NATS.Net;

namespace Services.Matchmake;

public class MatchReadyNatsListener : BackgroundService
{
    
    private readonly INatsConnection _nats;
    private readonly IMemoryCache _cache;
    private readonly string _natsAdress = "orchestrator.match.ready";

    public MatchReadyNatsListener(INatsConnection nats, IMemoryCache cache)
    {
        _nats = nats;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            
            try
            {

                await foreach (var msg in _nats.SubscribeAsync<MatchCreateInfo>(_natsAdress, cancellationToken: stoppingToken))
                {

                    Console.WriteLine($"[NATS] Received match ready for players: {string.Join(",", msg.Data.PlayerIds ?? Array.Empty<long>())}");
                    if (msg.Data.PlayerIds == null) continue;

                    for (int i = 0; i < msg.Data.PlayerIds.Length; i++)
                    {
                        var curP = msg.Data.PlayerIds[i];
                        QueuePlayerStatusResponse info = new QueuePlayerStatusResponse
                        {
                            Ip = msg.Data.ServerIp,
                            Port = msg.Data.Port,
                            Status = QueuePlayerStatus.Find
                        };

                        _cache.Set($"match:{curP}", info, TimeSpan.FromSeconds(120));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {    
            
                try
                {
                    await Task.Delay(5000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

        }

    }

}