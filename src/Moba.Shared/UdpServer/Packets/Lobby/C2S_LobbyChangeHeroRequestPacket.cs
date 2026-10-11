
using Moba.Shared.UdpServer.Heroes;

namespace Moba.Shared.UdpServer.Packets.Lobby;

public struct C2S_LobbyChangeHeroRequestPacket
{
    
    public MatchHeroes Hero;

    public int Serialize(Span<byte> buffer)
    {
        buffer[0] = (byte)Hero;
        return 1;
    }
    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        Hero = (MatchHeroes)buffer[0];
    }

}