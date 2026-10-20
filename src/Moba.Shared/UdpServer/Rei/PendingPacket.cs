
using Moba.Shared.UdpServer.Rei.Header;

namespace Moba.Shared.UdpServer.Rei;

public struct PendingPacket
{
    public byte[] Data;
    public int Length;
    public long SetTimestamp;
    public byte RetryCount;
}