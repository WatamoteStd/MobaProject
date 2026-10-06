
using System.Net;
using Moba.Shared.UdpServer.Packets;

namespace NetworkLayer.DataStructs;

public readonly struct NetworkCommand
{
    
    public PacketTypes Type {get; init;}
    public byte[] Payload {get; init;}
    public int PayloadLength {get; init;}
    public EndPoint PlayerEndPoint {get; init;}

}