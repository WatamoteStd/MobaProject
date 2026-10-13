using Moba.Shared.UdpServer.Packets;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading.Channels;
using NetworkLayer.DataStructs;
using Players;
using Moba.Shared.UdpServer.Packets.Structs;
using System.Net.Sockets;
using Moba.Shared.UdpServer.Packets.Lobby;
using Moba.Shared.UdpServer.Teams;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Moba.Shared.UdpServer.Heroes;
using Match;

namespace Phases;

public partial class PhaseManager
{

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern uint timeBeginPeriod(uint uPeriod);
    
    [DllImport("winmm.dll", SetLastError = true)]
    private static extern uint timeEndPeriod(uint uPeriod);

    private const float FixedDeltaTime = 1f / 60f;


    public event Action<Phases>? OnPhaseChanged;
    public enum Phases { WaitingForPlayers, Picking, InGame}



    public Phases CurrentPhase = Phases.WaitingForPlayers;

    private Thread? _gameThread;

    private readonly Guid _matchGuid;
    private readonly ChannelReader<NetworkCommand> _channel;
    private readonly Socket _socket;
    private readonly HashSet<long> _allowedIds;
    private PlayerConnection[] _playersConnections;
    private ushort _curPlayerId = 0;

    private MatchHolder? _matchHolder;

    // tools
    private float _timerBeforeMatchStart = 15.0f;

    public PhaseManager(int playersCount, ChannelReader<NetworkCommand> channel, Guid matchGuid, HashSet<long> playersIds, Socket socket) 
    {
        
        _playersConnections = new PlayerConnection[playersCount];
        _channel = channel;

        _matchGuid = matchGuid;
        _allowedIds = playersIds;

        _socket = socket;

        ChangePhase(Phases.WaitingForPlayers);
        timeBeginPeriod(1);

    }

    public void StartMatch()
    {
        
        _gameThread = new Thread(Update);
        _gameThread.IsBackground = false;
        _gameThread.Priority = ThreadPriority.Highest;
        _gameThread.Start();

    }

    public void Update()
    {

        long targetTickTicks = Stopwatch.Frequency / 60;
        
        
        while(true)
        {

            var startTime = Stopwatch.GetTimestamp();


            while(_channel.TryRead(out var cmd))
            {
                
                try
                {
                    
                    switch(CurrentPhase)
                    {
                        case Phases.WaitingForPlayers: ProcessWaitingPhase(cmd); break;
                        case Phases.Picking:
                            {
                                ProcessPickPhase(cmd);
                            }
                        break;
                        case Phases.InGame: ProcessGame(cmd); break;
                    }
                }
                finally
                {
                    if (cmd.PayloadLength > 0 && cmd.Payload != Array.Empty<byte>())
                    {
                        ArrayPool<byte>.Shared.Return(cmd.Payload);
                    }
                }

            }
            
            var elapsedTicks = Stopwatch.GetTimestamp() - startTime;

            if (CurrentPhase == Phases.Picking)
            {
                _timerBeforeMatchStart -= FixedDeltaTime;
    
                if (_timerBeforeMatchStart <= 0.0f)
                {
                    ChangePhase(Phases.InGame);
                }
            }
            else if(CurrentPhase == Phases.InGame)
            {
                
                

            }


            if(elapsedTicks < targetTickTicks)
            {
                int sleepMs = (int)((targetTickTicks - elapsedTicks) * 1000 / Stopwatch.Frequency);
                if(sleepMs > 1)
                {
                    Thread.Sleep(sleepMs - 1);
                }

                while((Stopwatch.GetTimestamp() - startTime) < targetTickTicks)
                {
                    Thread.SpinWait(10);
                }
            }

        }


    }

    public void ChangePhase(Phases nextPhase)
    {
        
        CurrentPhase = nextPhase;
        Console.WriteLine($"[Phase Manager] Phase Changed. Current is:{nextPhase.ToString()}");

        OnPhaseChanged?.Invoke(nextPhase);

        if(nextPhase == Phases.Picking)
        {
            byte total = (byte)_playersConnections.Length;

            ushort[] ids = new ushort[total];
            MatchTeams[] teams = new MatchTeams[total];
            string[] nicknames = new string[total];

            for(int i = 0; i < total; i++)
            {
                var pl = _playersConnections[i];
                ids[i] = pl.Id;
                teams[i] = pl.Team;
                nicknames[i] = pl.Nickname;
            }

            var packet = new S2C_LobbyJoinRequestPacket
            {
                PlayersCount = total,
                Ids = ids,
                Teams = teams,
                Nicknames = nicknames
            };

            Span<byte> buffer = stackalloc byte[1024];
            int writenBytes = PacketTranslator.Write(PacketTypes.S2C_LobbyJoinRequest, buffer, ref packet);

            for(int i = 0; i < total; i++)
            {
                _socket.SendTo(buffer[..writenBytes], _playersConnections[i].EndPoint);
            }
        }
        else if(nextPhase == Phases.InGame)
        {
            _matchHolder = new MatchHolder(_playersConnections);
        }

    }

    private void ProcessWaitingPhase(NetworkCommand cmd)
    {
        
        switch(cmd.Type)
        {
            
            case PacketTypes.C2S_Handshake:
                {
                    C2S_HandshakePacket packet = default;
                    PacketTranslator.Read(cmd.Payload, ref packet);

                    if (packet.SessionId == _matchGuid && _allowedIds.Contains(packet.UserId))
                    {

                        for(int i = 0; i < _curPlayerId; i++)
                        {
                            if (_playersConnections[i].PlayerId == packet.UserId)
                            {
                                _playersConnections[i].IsConnected = true;
                                _playersConnections[i].EndPoint = cmd.PlayerEndPoint;
                                return;
                            }
                        }
                    
                        if (_curPlayerId >= _playersConnections.Length)
                            return;
                        

                        MatchTeams pTeam = (_curPlayerId % 2 == 0) ? MatchTeams.Dark : MatchTeams.Light;

                        PlayerConnection player = new PlayerConnection
                        {
                            Id = _curPlayerId,
                            IsConnected = true,
                            EndPoint = cmd.PlayerEndPoint,
                            Nickname = packet.Nickname,
                            PlayerId = packet.UserId,
                            Team = pTeam
                        };
                        _playersConnections[_curPlayerId] = player;

                        Console.WriteLine($"[PhaseManager] New player connected to the game. In matchId:{_curPlayerId} GlobalId:{player.PlayerId}, Nickname:{player.Nickname}.");
                        Console.WriteLine($"[PhaseManager] Players:{_curPlayerId + 1} / {_playersConnections.Length}");

                        S2C_HandshakeResponse response = new S2C_HandshakeResponse
                        {
                            Status = S2C_HandshakeResponse.StatusCode.Ok,
                            LocalPlayerId = _curPlayerId,
                            TotalPlayersCount = (byte)_playersConnections.Length,
                            ActivePlayersCount = (byte)(_curPlayerId + 1)
                        };

                        Span<byte> tempBuffer = stackalloc byte[128];
                        int wrLength = PacketTranslator.Write(PacketTypes.S2C_HandshakeResponse, tempBuffer, ref response);

                        _socket.SendTo(tempBuffer[..wrLength], player.EndPoint);

                        S2C_MatchReadyUpdate forOldPck = new S2C_MatchReadyUpdate
                        {
                            TotalPlayersCount = (byte)_playersConnections.Length,
                            ActivePlayersCount = (byte)(_curPlayerId + 1)
                        };
                        int writenLength = PacketTranslator.Write(PacketTypes.S2C_MatchReadyUpdate, tempBuffer, ref forOldPck);

                        for(int i = 0; i < _playersConnections.Length; i++)
                        {
                            var conn = _playersConnections[i];
                            if (conn.IsConnected)
                            {
                                _socket.SendTo(tempBuffer[..writenLength], conn.EndPoint);
                            }
                        }

                        _curPlayerId++;

                        if(_curPlayerId == _playersConnections.Length)
                        {
                            ChangePhase(Phases.Picking);
                        }

                    }

                }
            break;

        }

    }

    private void ProcessPickPhase(NetworkCommand cmd)
    {
        
        switch(cmd.Type)
        {
            
            case PacketTypes.C2S_LobbyChangeHeroRequest:
                {
                    
                    C2S_LobbyChangeHeroRequestPacket packet = default;
                    PacketTranslator.Read(cmd.Payload, ref packet);


                    for(int i = 0; i < _playersConnections.Length; i++)
                    {
                        
                        if(_playersConnections[i].EndPoint.Equals(cmd.PlayerEndPoint))
                        {
                            
                            _playersConnections[i].SelectedHero = packet.Hero;

                            S2C_LobbyPlayerHeroChangedPacket response = new S2C_LobbyPlayerHeroChangedPacket
                            {
                                PlayerId = _playersConnections[i].Id,
                                Hero = packet.Hero
                            };

                            Broadcast(ref response);

                        }

                    }

                }
            break;

        }

    }

    private void ProcessGame(NetworkCommand cmd)
    {
        


    }

}