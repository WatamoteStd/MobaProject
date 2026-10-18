
using System.Runtime.InteropServices;

namespace Moba.Shared.UdpServer.Packets.Game.PositionSync;


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EntityPositionSync
{
    
    public ushort EntityId;
    public float PosX;
    public float PosZ;

}