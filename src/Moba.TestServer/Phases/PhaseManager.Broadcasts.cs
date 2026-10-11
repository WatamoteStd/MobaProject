
using Moba.Shared.UdpServer.Packets;
using Moba.Shared.UdpServer.Packets.Lobby;

namespace Phases;

public partial class PhaseManager
{
    
    private void Broadcast(ref S2C_LobbyPlayerHeroChangedPacket pak)
    {

        Span<byte> buffer = stackalloc byte[64];

        int writeBytes = PacketTranslator.Write(PacketTypes.S2C_LobbyPlayerHeroChanged, buffer, ref pak);
        
        for(int i = 0; i < _playersConnections.Length; i++)
        {
            _socket.SendTo(buffer[..writeBytes], _playersConnections[i].EndPoint);
        }

    }

}