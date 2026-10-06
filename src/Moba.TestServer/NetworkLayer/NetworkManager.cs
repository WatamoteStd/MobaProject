
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using NetworkLayer.DataStructs;

namespace NetworkLayer;

public class NetworkManager
{
    
    private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private readonly ChannelWriter<NetworkCommand> _channel;
    const int SIO_UDP_CONNRESET = -1744830452;

    // KIDS

    PacketCatcher? _packetCatcher;
    
    public NetworkManager(int port, ChannelWriter<NetworkCommand> channel)
    {
        
        if (OperatingSystem.IsWindows())
        {
            _socket.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0 }, null);
        }

        _socket.ReceiveBufferSize = 1024 * 64;
        _socket.SendBufferSize = 1024 * 64;

        EndPoint end = new IPEndPoint(IPAddress.Any, port);
        _socket.Bind(end);

        _channel = channel;

    }

    public void Start()
    {
        
        _packetCatcher = new PacketCatcher(_socket, _channel);
        _ = _packetCatcher.ListenAsync();

    }

}