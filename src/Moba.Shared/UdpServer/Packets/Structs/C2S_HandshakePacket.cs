
using System.Buffers.Binary;
using System.Text;

namespace Moba.Shared.UdpServer.Packets.Structs;

public struct C2S_HandshakePacket
{
    
    public Guid SessionId;
    public long UserId;
    public ushort NicknameLength;
    public string Nickname;

    public int Serialize(Span<byte> buffer)
    {
        if (buffer.Length < 26) return 0;

        int offset = 0;

        SessionId.TryWriteBytes(buffer);
        offset += 16;
        BinaryPrimitives.WriteInt64LittleEndian(buffer[offset..], UserId);
        offset += 8;
        NicknameLength = (ushort)Encoding.UTF8.GetByteCount(Nickname);
        BinaryPrimitives.WriteUInt16LittleEndian(buffer[offset..], NicknameLength);
        offset += 2;

        int bytesWriten = Encoding.UTF8.GetBytes(Nickname, buffer[offset..]);
        offset += bytesWriten;

        return offset;
    }

    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        
        if (buffer.Length < 26) return;

        int offset = 0;

        SessionId = new Guid(buffer[..16]);
        offset += 16;

        UserId = BinaryPrimitives.ReadInt64LittleEndian(buffer[offset..]);
        offset += 8;

        NicknameLength = BinaryPrimitives.ReadUInt16LittleEndian(buffer[offset..]);
        offset += 2;

        Nickname = Encoding.UTF8.GetString(buffer.Slice(offset, NicknameLength));

    }

}