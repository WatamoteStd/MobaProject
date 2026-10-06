
namespace Moba.Shared.UdpServer.Packets.Structs;

public struct C2S_HandshakePacket
{
    
    public Guid SessionId;

    public int Serialize(Span<byte> buffer)
    {
        if (buffer.Length < 16) return 0;

        SessionId.TryWriteBytes(buffer);

        return 16;
    }

    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        
        if (buffer.Length < 16) return;

        SessionId = new Guid(buffer[..16]);

    }

}