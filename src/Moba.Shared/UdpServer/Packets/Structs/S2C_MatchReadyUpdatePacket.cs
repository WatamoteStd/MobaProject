
namespace Moba.Shared.UdpServer.Packets.Structs;

public struct S2C_MatchReadyUpdate
{
    
    public byte TotalPlayersCount;
    public byte ActivePlayersCount;

    public int Serialize(Span<byte> buffer)
    {
        
        buffer[0] = TotalPlayersCount;
        buffer[1] = ActivePlayersCount;

        return 2;

    }
    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        TotalPlayersCount = buffer[0];
        ActivePlayersCount = buffer[1];
    }
}