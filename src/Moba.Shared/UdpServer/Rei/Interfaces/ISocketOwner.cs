
using System.Net;

namespace Moba.Shared.UdpServer.Rei.Interfaces;

public interface ISocketOwner
{
    
    void SendTo(Span<byte> buffer, int length, EndPoint endPoint);

}