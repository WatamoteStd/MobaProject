
using Moba.Shared.UdpServer.Packets;
using Moba.Shared.UdpServer.Packets.Game;
using Moba.Shared.UdpServer.Packets.Lobby;

namespace Phases;

public partial class PhaseManager
{
    
    public void Broadcast(ref S2C_LobbyPlayerHeroChangedPacket pak)
    {

        Span<byte> buffer = stackalloc byte[64];

        int writeBytes = PacketTranslator.Write(PacketTypes.S2C_LobbyPlayerHeroChanged, buffer, ref pak);
        
        for(int i = 0; i < _playersConnections.Length; i++)
        {
            _socket.SendTo(buffer[..writeBytes], _playersConnections[i].EndPoint);
        }

    }

    public void Broadcast(ref S2C_PlayersSpawnPacket pak)
    {
        
        Span<byte> buffer = stackalloc byte[2048];

        int writen = PacketTranslator.Write(PacketTypes.S2C_PlayersSpawn, buffer, ref pak);

        for(int i = 0; i < _playersConnections.Length; i++)
        {
            _socket.SendTo(buffer[..writen], _playersConnections[i].EndPoint);
        }

    }

}