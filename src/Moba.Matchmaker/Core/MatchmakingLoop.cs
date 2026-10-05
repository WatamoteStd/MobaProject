using Moba.Shared.MatchmakerLibs;
using Moba.Shared.OrchestratorData;
using NATS.Client.Serializers.Json;
using NATS.Net;

namespace Moba.Matchmaker.Core;

public class MatchmakingLoop
{
    
    private readonly MatchmakingManager _manager;
    private readonly MatchmakingEngine _engine;
    private readonly NatsClient _nats;
    private readonly Thread _thread;
    public bool IsWork {get; private set;}= false;
    public MatchmakingLoop(MatchmakingManager manager, MatchmakingEngine engine, NatsClient nats)
    {
        _manager = manager;
        _engine = engine;
        _nats = nats;

        _thread = new Thread(ProcessQueue);
        _thread.IsBackground = true;
    }

    public void Start()
    {
        
        IsWork = true;
        _thread.Start();

    }

    public void Stop()
    {
        
        IsWork = false;

    }

    private void ProcessQueue()
    {
        
        while(IsWork)
        {
            
            var soloPool = _manager.GetPool(MatchProperty.Solo);
            var soloSnap = soloPool.GetActivePlayers();

            if (soloSnap.Count > 0)
            {
                
                var matches = _engine.FindMatch(MatchProperty.Solo, soloSnap);  

                foreach(var m in matches)
                {
                    foreach(var p in m.Players)
                    {
                        _manager.RemovePlayer(p.PlayerId, MatchProperty.Solo);
                    }
                    long[] pIds = new long[m.Players.Length];
                    for (int i = 0; i < m.Players.Length; i++)
                    {
                        pIds[i] = m.Players[i].PlayerId;
                    }


                    var matchDto = new MatchCreateInfo(
                        MatchId: Guid.NewGuid(),
                        GameMode: MatchProperty.Solo,
                        PlayerIds: pIds
                    );

                    _nats.PublishAsync(
                        subject: "matchmaking.created",
                        data: matchDto,
                        serializer: NatsJsonSerializer<MatchCreateInfo>.Default
                    );
                    Console.WriteLine($"[MATCHMAKER] Собрана катка Solo! Игроков: {m.Players.Length}");
                }
                

            }

            var trioPool = _manager.GetPool(MatchProperty.Trio);
            var trioSnap = trioPool.GetActivePlayers();

            if (trioSnap.Count > 0)
            {
                
                var matches = _engine.FindMatch(MatchProperty.Trio, trioSnap);

                foreach(var m in matches)
                {
                    foreach(var p in m.Players)
                    {
                        _manager.RemovePlayer(p.PlayerId, MatchProperty.Trio);
                    }

                    long[] pIds = new long[m.Players.Length];
                    for (int i = 0; i < m.Players.Length; i++)
                    {
                        pIds[i] = m.Players[i].PlayerId;
                    }


                    var matchDto = new MatchCreateInfo(
                        MatchId: Guid.NewGuid(),
                        GameMode: MatchProperty.Trio,
                        PlayerIds: pIds
                    );

                    _nats.PublishAsync(
                        subject: "matchmaking.created",
                        data: matchDto,
                        serializer: NatsJsonSerializer<MatchCreateInfo>.Default
                    );

                    Console.WriteLine($"[MATCHMAKER] Собрана катка Trio! Игроков: {m.Players.Length}");
                }

            }

            var fullPool = _manager.GetPool(MatchProperty.Full);
            var fullSpan = fullPool.GetActivePlayers();

            if (fullSpan.Count > 0)
            {
                
                var matches = _engine.FindMatch(MatchProperty.Full, fullSpan);

                foreach(var m in matches)
                {
                    foreach(var p in m.Players)
                    {
                        _manager.RemovePlayer(p.PlayerId, MatchProperty.Full);
                    }

                    long[] pIds = new long[m.Players.Length];
                    for (int i = 0; i < m.Players.Length; i++)
                    {
                        pIds[i] = m.Players[i].PlayerId;
                    }


                    var matchDto = new MatchCreateInfo(
                        MatchId: Guid.NewGuid(),
                        GameMode: MatchProperty.Full,
                        PlayerIds: pIds
                    );

                    _nats.PublishAsync(
                        subject: "matchmaking.created",
                        data: matchDto,
                        serializer: NatsJsonSerializer<MatchCreateInfo>.Default
                    );
                    Console.WriteLine($"[MATCHMAKER] Собрана катка Full! Игроков: {m.Players.Length}");
                }

            }


            Thread.Sleep(2000);



        }

    }

}