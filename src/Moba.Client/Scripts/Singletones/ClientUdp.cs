using Godot;
using Moba.Shared.UdpServer.Packets;
using Moba.Shared.UdpServer.Packets.Structs;
using System;
using System.Net;
using System.Net.Sockets;

public partial class ClientUdp : Node
{
	
	public static ClientUdp Instance {get; private set;}
	private Socket _socket;

	public override void _EnterTree()
	{
		Instance = this;
	}


	public override void _Ready()
	{
	

	}

	public void ConnectAndSendHandshake()
	{

		_socket?.Close();
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		_socket.Connect(IPAddress.Parse(GameSession.Instance.MatchIp), GameSession.Instance.MatchPort);


		
		var packet = new C2S_HandshakePacket
		{
			SessionId = GameSession.Instance.MatchId,
			UserId = GameSession.Instance.UserId,
			Nickname = GameSession.Instance.Nickname
		};

		Span<byte> buffer = stackalloc byte[256];

		int bytesWriten = PacketTranslator.Write(PacketTypes.C2S_Handshake, buffer, ref packet);
		_socket.Send(buffer[..bytesWriten]);

		
		GD.Print($"[ClientUdp] Handshake sent! Size: {bytesWriten} bytes.");

	}

	public override void _ExitTree()
	{
		_socket?.Close();
		_socket?.Dispose();
	}


}
