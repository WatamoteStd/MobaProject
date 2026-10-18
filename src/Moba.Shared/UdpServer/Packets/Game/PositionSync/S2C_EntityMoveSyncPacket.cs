
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Moba.Shared.UdpServer.Packets.Game.PositionSync;

public struct S2C_EntityMoveSyncPacket
{

    public byte Count;

    public EntityPositionSync[] Entities;

    public int Serialize(Span<byte> buffer)
    {
        buffer[0] = Count;

        var entitySpan = MemoryMarshal.AsBytes(Entities.AsSpan());

        if(buffer.Length < entitySpan.Length) return 0;
        entitySpan.CopyTo(buffer[1..]);

        return entitySpan.Length + 1;

    }

    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        
        Count = buffer[0];
        int expectedSize = 1 + (Count * Unsafe.SizeOf<EntityPositionSync>());

        Entities = new EntityPositionSync[Count];

        if (buffer.Length < expectedSize) return;

        Entities = MemoryMarshal.Cast<byte, EntityPositionSync>(buffer[1..expectedSize]).ToArray();



    }

}