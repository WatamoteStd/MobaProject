using Moba.Shared.UdpServer.Packets;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading.Channels;
using NetworkLayer.DataStructs;
using Players;
using Moba.Shared.UdpServer.Packets.Structs;

namespace Phases;

public class PhaseManager
{

    public event Action<Phases>? OnPhaseChanged;
    public enum Phases { WaitingForPlayers, Picking, InGame}
    public Phases CurrentPhase = Phases.WaitingForPlayers;

    private readonly Guid _matchGuid;
    private readonly ChannelReader<NetworkCommand> _channel;
    private PlayerConnection[] _playersConnections;
    private int _curPlayerId = 0;
    public PhaseManager(int playersCount, ChannelReader<NetworkCommand> channel, Guid matchGuid) 
    {
        
        _playersConnections = new PlayerConnection[playersCount];
        _channel = channel;

        _matchGuid = matchGuid;

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

                    if (packet.SessionId == _matchGuid)
                    {

                        for(int i = 0; i < _curPlayerId; i++)
                        {
                            if (_playersConnections[i].EndPoint.Equals(cmd.PlayerEndPoint))
                            {
                                _playersConnections[i].IsConnected = true;
                                return;
                            }
                        }
                    
                        if (_curPlayerId >= _playersConnections.Length)
                            return;
                        
                        PlayerConnection player = new PlayerConnection
                        {
                            Id = _curPlayerId,
                            IsConnected = true,
                            EndPoint = cmd.PlayerEndPoint
                        };
                        _playersConnections[_curPlayerId] = player;

                        Console.WriteLine($"[PhaseManager] New player connected to the game. In matchId:{_curPlayerId}.");
                        Console.WriteLine($"[PhaseManager] Players:{_curPlayerId + 1} / {_playersConnections.Length}");

                        _curPlayerId++;

                    }

                }
            break;

        }

    }


}