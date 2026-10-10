using Moba.Shared.UdpServer.Packets;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading.Channels;
using NetworkLayer.DataStructs;
using Players;
using Moba.Shared.UdpServer.Packets.Structs;
using System.Net.Sockets;

namespace Phases;

public class PhaseManager
{

    public event Action<Phases>? OnPhaseChanged;
    public enum Phases { WaitingForPlayers, Picking, InGame}
    public Phases CurrentPhase = Phases.WaitingForPlayers;

    private readonly Guid _matchGuid;
    private readonly ChannelReader<NetworkCommand> _channel;
    private readonly Socket _socket;
    private readonly HashSet<long> _allowedIds;
    private PlayerConnection[] _playersConnections;
    private ushort _curPlayerId = 0;
    public PhaseManager(int playersCount, ChannelReader<NetworkCommand> channel, Guid matchGuid, HashSet<long> playersIds, Socket socket) 
    {
        
        _playersConnections = new PlayerConnection[playersCount];
        _channel = channel;

        _matchGuid = matchGuid;
        _allowedIds = playersIds;

        _socket = socket;

        ChangePhase(Phases.WaitingForPlayers);

    }

    public void StartMatch()
    {
        
        _ = Update();

    }

    public async Task Update()
    {
        
        while(true)
        {
            while(_channel.TryRead(out var cmd))
            {
                
                try
                {
                    
                    switch(CurrentPhase)
                    {
                        
                        case Phases.WaitingForPlayers:
                            {
                                
                                ProcessWaitingPhase(cmd);

                            }
                        break;

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
            

            await Task.Delay(16);

        }

    }

    public void ChangePhase(Phases nextPhase)
    {
        
        CurrentPhase = nextPhase;
        Console.WriteLine($"[Phase Manager] Current phase is:{nextPhase.ToString()}");

        OnPhaseChanged?.Invoke(nextPhase);

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
                        
                        PlayerConnection player = new PlayerConnection
                        {
                            Id = _curPlayerId,
                            IsConnected = true,
                            EndPoint = cmd.PlayerEndPoint,
                            Nickname = packet.Nickname,
                            PlayerId = packet.UserId
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

                    }

                }
            break;

        }

    }


}