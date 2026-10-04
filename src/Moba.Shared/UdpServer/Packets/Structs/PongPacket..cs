
namespace Moba.Shared.UdpServer.Packets.Structs;

public struct PongPacket
{
    
    public byte Pong;

    public int Serialize(Span<byte> buffer)
    {
        
        buffer[0] = Pong;
        return 1;

    }
    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        
        Pong = buffer[0];

    }

}