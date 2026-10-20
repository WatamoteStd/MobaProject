using System.Buffers;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Moba.Shared.UdpServer.Rei.Header;
using Moba.Shared.UdpServer.Rei.Interfaces;
using Moba.Shared.UdpServer.Rei.Property;

namespace Moba.Shared.UdpServer.Rei;

public class ReiClone
{
    private  EndPoint _userEndpoint;
    private readonly ISocketOwner _socketOwner;

    private const int BUFFER_SIZE = 1024;
    private readonly PendingPacket[] _sendBuffer = new PendingPacket[BUFFER_SIZE];

    private ushort _outSequence = 0;

    public ReiClone(ISocketOwner socketOwner, EndPoint userEndPoint)
    {
        
        _socketOwner = socketOwner;
        _userEndpoint = userEndPoint;

    }

    public void SendPacket(ReadOnlySpan<byte> payload, DeliveryMethod deliveryMethod)
    {
        
        switch (deliveryMethod)
        {
            
            case DeliveryMethod.Unreliable:
                {

                    ReiHeader header = new ReiHeader(DeliveryMethod.Unreliable, false, _outSequence);

                    int headerSize = Unsafe.SizeOf<ReiHeader>();
                    Span<byte> buffer = stackalloc byte[headerSize + payload.Length];

                    MemoryMarshal.Write(buffer, in header);

                    payload.CopyTo(buffer.Slice(headerSize));

                    _socketOwner.SendTo(buffer, buffer.Length, _userEndpoint);

                    _outSequence++;

                }
            break;

            case DeliveryMethod.ReliableOrdered:
            case DeliveryMethod.ReliableUnordered:
                {

                    ReiHeader header = new ReiHeader(deliveryMethod, false, _outSequence);

                    int headerSize = Unsafe.SizeOf<ReiHeader>();
                    var array = ArrayPool<byte>.Shared.Rent(headerSize + payload.Length);

                    MemoryMarshal.Write(array, in header);
                    payload.CopyTo(array[headerSize..]);


                    PendingPacket pending = new PendingPacket
                    {
                        Data = array,
                        Length = headerSize + payload.Length,
                        SetTimestamp = Stopwatch.GetTimestamp(),
                        RetryCount = 0
                    };

                    int index = _outSequence & 0b_11_1111_1111;
                    _outSequence++;
                    _sendBuffer[index] = pending;

                    _socketOwner.SendTo(array, pending.Length, _userEndpoint);

                }
            break;

            case DeliveryMethod.UnreliableSequenced:
                {
                    
                    ReiHeader header = new ReiHeader(DeliveryMethod.UnreliableSequenced, false ,_outSequence);
                    int headerSize = Unsafe.SizeOf<ReiHeader>();

                    Span<byte> buffer = stackalloc byte[headerSize + payload.Length];

                    MemoryMarshal.Write(buffer, in header);
                    payload.CopyTo(buffer[headerSize..]);

                    _socketOwner.SendTo(buffer, buffer.Length, _userEndpoint);

                    _outSequence++;

                }
            break;

        }

    }

    public void ReceivedBytes(ReadOnlySpan<byte> rawData, EndPoint sender)
    {
        
        

    }
    

}