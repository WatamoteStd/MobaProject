
using System.Buffers.Binary;

namespace Moba.Shared.UdpServer.Packets.Structs;

public struct S2C_HandshakeResponse
{
    
    public enum StatusCode : byte { Ok, NotOk};
    public StatusCode Status;
    public ushort LocalPlayerId;
    public byte TotalPlayersCount;
    public byte ActivePlayersCount;

    public int Serialize(Span<byte> buffer)
    {
        
        if(buffer.Length < 5) return 0;

        int offset = 0;
        buffer[0] = (byte)Status;
        offset++;
        BinaryPrimitives.WriteUInt16LittleEndian(buffer[offset..], LocalPlayerId);
        offset += 2;

        buffer[offset] = TotalPlayersCount;
        offset ++;
        buffer[offset] = ActivePlayersCount;
        offset++;

        return offset;

    }

    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        
        if(buffer.Length < 5) return;

        int offset = 0;

        Status = (StatusCode)buffer[0];
        offset++;
        LocalPlayerId = BinaryPrimitives.ReadUInt16LittleEndian(buffer[offset..]);
        offset += 2;

        TotalPlayersCount = buffer[offset];
        offset++;
        ActivePlayersCount = buffer[offset];

    }

}