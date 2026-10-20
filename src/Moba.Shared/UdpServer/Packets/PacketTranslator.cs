using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Moba.Shared.UdpServer.Packets.Game;
using Moba.Shared.UdpServer.Packets.Lobby;
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

    public static int Write(PacketTypes type, Span<byte> buffer, ref C2S_HandshakePacket pck)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort) type);

        int payloadSize = pck.Serialize(buffer[2..]);

        return 2 + payloadSize;
    }
    public static void Read(ReadOnlySpan<byte> buffer, ref C2S_HandshakePacket pck)
    {
        pck.Deserialize(buffer);
    }

    public static int Write(PacketTypes type, Span<byte> buffer, ref S2C_HandshakeResponse pck)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort) type);

        int payloadSize = pck.Serialize(buffer[2..]);
        return 2 + payloadSize;
    }

    public static void Read(ReadOnlySpan<byte> buffer, ref S2C_HandshakeResponse pck)
    {
        pck.Deserialize(buffer);
    }
    public static int Write(PacketTypes type, Span<byte> buffer, ref S2C_MatchReadyUpdate pck)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort) type);

        int payloadSize = pck.Serialize(buffer[2..]);

        return 2 + payloadSize;
    }
    public static void Read(ReadOnlySpan<byte> buffer, ref S2C_MatchReadyUpdate pck)
    {
        pck.Deserialize(buffer);
    }

    public static int Write(PacketTypes type, Span<byte> buffer, ref S2C_LobbyJoinRequestPacket pck)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort) type);

        int payloadSize = pck.Serialize(buffer[2..]);

        return 2 + payloadSize;
    }
    public static void Read(ReadOnlySpan<byte> buffer, ref S2C_LobbyJoinRequestPacket pck)
    {
        pck.Deserialize(buffer);
    }

    public static int Write(PacketTypes type, Span<byte> buffer, ref C2S_LobbyChangeHeroRequestPacket pck)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort) type);

        int payloadSize = pck.Serialize(buffer[2..]);

        return 2 + payloadSize;
    }
    public static void Read(ReadOnlySpan<byte> buffer, ref C2S_LobbyChangeHeroRequestPacket pck)
    {
        pck.Deserialize(buffer);
    }

    public static int Write(PacketTypes type, Span<byte> buffer, ref S2C_LobbyPlayerHeroChangedPacket pck)
    {
        
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort) type);

        int payloadSize = pck.Serialize(buffer[2..]);

        return 2 + payloadSize;

    }
    public static void Read(ReadOnlySpan<byte> buffer, ref S2C_LobbyPlayerHeroChangedPacket pck)
    {
        pck.Deserialize(buffer);
    }
    public static int Write(PacketTypes type, Span<byte> buffer, ref S2C_PlayersSpawnPacket pck)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort) type);
        int paylaodSize = pck.Serialize(buffer[2..]);

        return 2 + paylaodSize;
    }
    public static void Read(ReadOnlySpan<byte> buffer, ref S2C_PlayersSpawnPacket pck)
    {
        pck.Deserialize(buffer);
    }

    public static int Write(PacketTypes type, Span<byte> buffer, ref C2S_MoveRequestPacket pck)
    {
        
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort) type);
        int payloadSize = pck.Serialize(buffer[2..]);

        return 2 + payloadSize;

    }
    public static void Read(ReadOnlySpan<byte> buffer, ref C2S_MoveRequestPacket pck)
    {
        pck.Deserialize(buffer);
    }

}