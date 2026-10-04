
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Moba.Shared.UdpServer.Packets;
using Moba.Shared.UdpServer.Packets.Structs;

namespace NetworkLayer;

public class PacketCatcher
{
    
    private readonly Socket _socket;
    public bool IsWork {get; private set; } = false;
    private byte[] buffer = new byte[2048];

    public PacketCatcher(Socket socket)
    {
        
        _socket = socket;
        IsWork = true;

    }

    public async Task ListenAsync()
    {
        EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        
        while(IsWork)
        {
            
            SocketReceiveFromResult result = await _socket.ReceiveFromAsync(buffer, endPoint);

            if (result.ReceivedBytes < 2) continue;

            ReadOnlySpan<byte> receivedSpan = buffer.AsSpan(0, result.ReceivedBytes);

            ushort rawType = BinaryPrimitives.ReadUInt16LittleEndian(receivedSpan);
            PacketTypes packetType = (PacketTypes)rawType;

            ReadOnlySpan<byte> payload = receivedSpan[2..];

            switch (packetType)
            {
                
                case PacketTypes.Pint:
                    {
                        var ping = new PingPacket();
                        PacketTranslator.Read(payload, ref ping);
                        Console.WriteLine($"Received ping packet. Value:{ping.Ping}. Ip:{result.RemoteEndPoint}");
                    }
                break;

            }

        }

    }

}