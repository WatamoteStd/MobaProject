
using System.Buffers.Binary;
using Moba.Shared.UdpServer.Packets.Structs;

namespace Moba.Shared.UdpServer.Packets;

public static class PacketTranslator
{
    
    public static int Write(PacketTypes type, Span<byte> buffer, ref PingPacket pck)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort)type);

        int payloadSize = pck.Serialize(buffer[2..]);

        return 2 + payloadSize;

    }
    public static void Read(ReadOnlySpan<byte> buffer, ref PingPacket pck)
    {
        
        pck.Deserialize(buffer);

    }

    public static int Write(PacketTypes type, Span<byte> buffer, ref PongPacket pck)
    {
        
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort)type);

        int payloadSize = pck.Serialize(buffer[2..]);

        return 2 + payloadSize;

    }
    public static void Read(ReadOnlySpan<byte> buffer, ref PongPacket pck)
    {
        pck.Deserialize(buffer);
    }

}