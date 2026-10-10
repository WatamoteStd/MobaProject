using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Net.Sockets;
using System.Threading.Channels;
using System.Threading.Tasks;
using Moba.Shared.UdpServer.Packets;

public partial class PacketListner
{
	
	private readonly Socket _socket;
	private readonly ChannelWriter<NetworkPacket> _channel;
	private byte[] _buffer = new byte[2048];

	public PacketListner(ChannelWriter<NetworkPacket> channel, Socket socket)
	{
		_channel = channel;
		_socket = socket;
	}

	public void Start()
	{
		_ = ListenAsync();
	}

	private async Task ListenAsync()
	{
		
		while(true)
		{

			int bytesWriten = await _socket.ReceiveAsync(_buffer);
			if(bytesWriten < 2) break;

			PacketTypes type = (PacketTypes)BinaryPrimitives.ReadUInt16LittleEndian(_buffer);

			byte[] packetData = ArrayPool<byte>.Shared.Rent(bytesWriten - 2);

			Array.Copy(_buffer, 2, packetData, 0, bytesWriten - 2);

			var netPacket = new NetworkPacket(packetData, bytesWriten - 2, type);

			_channel.TryWrite(netPacket);

		}


	}

}
