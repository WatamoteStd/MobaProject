using System.Runtime.InteropServices;
using Moba.Shared.UdpServer.Rei.Property;

namespace Moba.Shared.UdpServer.Rei.Header;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct ReiHeader
{
    
    private readonly byte _property;
    private readonly ushort _sequenceId;

    public DeliveryMethod Method => (DeliveryMethod)(_property & 3);
    public bool IsAck => (_property & 4) != 0;
    public ushort SequenceId => _sequenceId;

    public ReiHeader(DeliveryMethod method, bool isAck, ushort sequenceId)
    {
        
        byte prop = (byte)((byte)method & 3);

        if(isAck) prop |= 4;

        _property = prop;
        _sequenceId = sequenceId;

    }

}