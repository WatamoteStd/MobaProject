
using System.Buffers.Binary;

namespace Moba.Shared.UdpServer.Packets.Game;

public struct C2S_MoveRequestPacket
{
    
    public float X;
    public float Z;

    public int Serialize(Span<byte> buffer)
    {

        if(buffer.Length < 8) return 0;
        
        int offset = 0;

        BinaryPrimitives.WriteSingleLittleEndian(buffer, X);
        offset +=4;
        BinaryPrimitives.WriteSingleLittleEndian(buffer[offset..], Z);
        offset += 4;

        return offset;
        

    }

    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        if(buffer.Length < 8) return;
        
        int offset = 0;

        X = BinaryPrimitives.ReadSingleLittleEndian(buffer);
        offset += 4;
        Z = BinaryPrimitives.ReadSingleLittleEndian(buffer[offset..]);

    }

}