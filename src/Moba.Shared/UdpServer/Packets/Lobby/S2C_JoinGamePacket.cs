

namespace Moba.Shared.UdpServer.Packets.Lobby;

public struct S2C_JoinGamePacket
{
    
    public byte PlayersCount;
    public ushort[] PlayerIds;
    public float[] PositionsX;
    public float[] PositionsZ;

    /*public int Serialize(Span<byte> buffer)
    {
        
        int offset = 0;

        

    }*/

}