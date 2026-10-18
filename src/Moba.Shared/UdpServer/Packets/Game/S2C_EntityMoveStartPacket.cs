
using System.Buffers.Binary;

namespace Moba.Shared.UdpServer.Packets.Game;

public struct S2C_EntityMoveStartPacket
{
    
    public ushort EntityId;
    public float X;
    public float Z;

    public int Serialize(Span<byte> buffer)
    {
        if(buffer.Length < 10) return 0;
        int offset = 0;

        BinaryPrimitives.WriteUInt16LittleEndian(buffer, EntityId);
        offset += 2;
        BinaryPrimitives.WriteSingleLittleEndian(buffer[offset..], X);
        offset += 4;
        BinaryPrimitives.WriteSingleLittleEndian(buffer[offset..], Z);
        offset += 4;

        return offset;

    }

    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        
        int offset = 0;

        EntityId = BinaryPrimitives.ReadUInt16LittleEndian(buffer);
        offset +=2;
        X = BinaryPrimitives.ReadSingleLittleEndian(buffer[offset..]);
        offset +=4;
        Z = BinaryPrimitives.ReadSingleLittleEndian(buffer[offset..]);

    }

}