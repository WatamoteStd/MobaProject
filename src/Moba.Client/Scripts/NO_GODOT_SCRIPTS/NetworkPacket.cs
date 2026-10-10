using System;
using Moba.Shared.UdpServer.Packets;

public readonly struct NetworkPacket
{
	
	public readonly byte[] Buffer;
	public readonly int PayloadLength;
	public readonly PacketTypes Type;

	public NetworkPacket(byte[] buffer, int payloadLength, PacketTypes type)
	{
		Buffer = buffer;
		PayloadLength = payloadLength;
		Type = type;
	}

}
