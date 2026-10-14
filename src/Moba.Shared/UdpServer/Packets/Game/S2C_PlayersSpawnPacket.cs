
using System.Buffers.Binary;
using Moba.Shared.UdpServer.Heroes;

namespace Moba.Shared.UdpServer.Packets.Game;

public struct S2C_PlayersSpawnPacket
{
    
    public ushort[] PlayersIds;
    public ushort[] EntityIds;
    public float[] PosX;
    public float[] PosZ;
    public MatchHeroes[] Heroes;

    public int Serialize(Span<byte> buffer)
    {
        
        if(buffer.Length < 24) return 0;
        int offset = 0;

        buffer[offset] = (byte)PlayersIds.Length;
        offset++;

        for(int i = 0; i < PlayersIds.Length; i++)
        {
            
            BinaryPrimitives.WriteUInt16LittleEndian(buffer[offset..], PlayersIds[i]);
            offset += 2;

            BinaryPrimitives.WriteSingleLittleEndian(buffer[offset..], PosX[i]);
            offset += 4;

            BinaryPrimitives.WriteSingleLittleEndian(buffer[offset..], PosZ[i]);
            offset += 4;

            BinaryPrimitives.WriteUInt16LittleEndian(buffer[offset..], (ushort)Heroes[i]);
            offset += 2;

            BinaryPrimitives.WriteUInt16LittleEndian(buffer[offset..], EntityIds[i]);
            offset += 2;
        }

        return offset;

    }
    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        
        int offset = 0;

        byte count = buffer[0];
        offset++;

        PlayersIds = new ushort[count];
        EntityIds = new ushort[count];
        PosX = new float[count];
        PosZ = new float[count];
        Heroes = new MatchHeroes[count];

        for(int i = 0; i < count; i++)
        {
            
            PlayersIds[i] = BinaryPrimitives.ReadUInt16LittleEndian(buffer[offset..]);
            offset += 2;

            PosX[i] = BinaryPrimitives.ReadSingleLittleEndian(buffer[offset..]);
            offset += 4;

            PosZ[i] = BinaryPrimitives.ReadSingleLittleEndian(buffer[offset..]);
            offset += 4;

            Heroes[i] = (MatchHeroes)BinaryPrimitives.ReadUInt16LittleEndian(buffer[offset..]);
            offset += 2;

            EntityIds[i] = BinaryPrimitives.ReadUInt16LittleEndian(buffer[offset..]);
            offset += 2;

        }

    }

}