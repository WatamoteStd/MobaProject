
using System.Net;
using System.Net.Sockets;

namespace NetworkLayer;

public class NetworkManager
{
    
    private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    const int SIO_UDP_CONNRESET = -1744830452;

    // KIDS

    PacketCatcher? _packetCatcher;
    
    public NetworkManager(int port)
    {
        
        if (OperatingSystem.IsWindows())
        {
            _socket.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0 }, null);
        }

        _socket.ReceiveBufferSize = 1024 * 64;
        _socket.SendBufferSize = 1024 * 64;

        EndPoint end = new IPEndPoint(IPAddress.Any, port);
        _socket.Bind(end);

    }

    public void Start()
    {
        
        _packetCatcher = new PacketCatcher(_socket);
        _ = _packetCatcher.ListenAsync();

    }

}