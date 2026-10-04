
namespace Moba.Shared.UdpServer.Packets.Structs;

public struct PingPacket
{
    
    public byte Ping;

    public int Serialize(Span<byte> buffer)
    {
        int offset = 0;
        buffer[offset] = Ping;
        offset += 1;

        return offset;
    }

    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        int offset = 0;
        Ping = buffer[offset];
    }

}