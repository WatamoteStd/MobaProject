
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using Moba.Shared.UdpServer.Packets;
using Moba.Shared.UdpServer.Packets.Structs;
using NetworkLayer.DataStructs;

namespace NetworkLayer;

public class PacketCatcher
{
    
    private readonly Socket _socket;
    public bool IsWork {get; private set; } = false;
    private byte[] buffer = new byte[2048];
    private readonly byte[] _pongBuffer = new byte[32];
    private readonly ChannelWriter<NetworkCommand> _channel;

    public PacketCatcher(Socket socket, ChannelWriter<NetworkCommand> channel)
    {
        
        _socket = socket;
        _channel = channel;
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
            int payloadLen = result.ReceivedBytes - 2;

            // PACKETS LOGIC

            switch (packetType)
            {
                
                case PacketTypes.Pint:
                    {
                        var ping = new PingPacket();
                        PacketTranslator.Read(payload, ref ping);
                        Console.WriteLine($"Received ping packet. Value:{ping.Ping}. Ip:{result.RemoteEndPoint}");

                        var pong = new PongPacket { Pong = ping.Ping};

                        int writenBytes = PacketTranslator.Write(PacketTypes.Pong, _pongBuffer, ref pong);

                        _socket.SendTo(_pongBuffer[..writenBytes], SocketFlags.None, result.RemoteEndPoint);

                        Console.WriteLine($"Send pong packet to IP:{result.RemoteEndPoint}");

                    }
                break;

                default:
                    {
                        bool isOk = false;
                        byte[] rented;
                        if(payloadLen > 0)
                        {
                            rented = ArrayPool<byte>.Shared.Rent(payloadLen);
                            payload.CopyTo(rented);
                        }
                        else rented = Array.Empty<byte>();

                        try
                        {

                            NetworkCommand cmd = new NetworkCommand
                            {
                                Type = packetType,
                                Payload = rented,
                                PayloadLength = payloadLen,
                                PlayerEndPoint = result.RemoteEndPoint
                            };

                            if (_channel.TryWrite(cmd)) isOk = true;

                        }
                        finally
                        {
                            if (!isOk)
                            {
                                ArrayPool<byte>.Shared.Return(rented);
                            }
                        }
                       
                    }
                break;

            }

        }

    }

}