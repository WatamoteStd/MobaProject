
namespace Moba.Shared.UdpServer.Packets;

public enum PacketTypes : ushort
{
    
    Pint = 0,
    Pong = 1,
    C2S_Handshake = 2,
    S2C_HandshakeResponse = 3

}