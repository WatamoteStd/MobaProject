
using System.Buffers.Binary;
using Moba.Shared.UdpServer.Heroes;

namespace Moba.Shared.UdpServer.Packets.Lobby;

public struct S2C_LobbyPlayerHeroChangedPacket
{
    public ushort PlayerId;
    public MatchHeroes Hero;

    public int Serialize(Span<byte> buffer)
    {
        
        if(buffer.Length < 3) return 0;

        BinaryPrimitives.WriteUInt16LittleEndian(buffer, PlayerId);
        buffer[2] = (byte)Hero;

        return 3;

    }
    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        
        if(buffer.Length < 3) return;

        PlayerId = BinaryPrimitives.ReadUInt16LittleEndian(buffer);
        Hero = (MatchHeroes)buffer[2];

    }
}