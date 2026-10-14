using Godot;
using Moba.Shared.UdpServer.Packets;
using Moba.Shared.UdpServer.Packets.Game;
using Moba.Shared.UdpServer.Packets.Lobby;
using Moba.Shared.UdpServer.Packets.Structs;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

public partial class ClientUdp : Node
{
	public event Action<int, int> OnGameReadyPlayersUpdate; // total and ready
	public event Action<S2C_LobbyJoinRequestPacket> OnSwithToLobby;
	public event Action<S2C_LobbyPlayerHeroChangedPacket> OnLobbyPlayerChanged;


	public static ClientUdp Instance {get; private set;}
	private Socket _socket;
	private Channel<NetworkPacket> _channel = Channel.CreateUnbounded<NetworkPacket>();
	private PacketListner _listener;

	private ConcurrentQueue<NetworkPacket> _gamePacketQueue = new();

	public override void _EnterTree()
	{
		Instance = this;
	}

	public void ConnectAndSendHandshake()
	{

		_socket?.Close();
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		_socket.Connect(IPAddress.Parse(GameSession.Instance.MatchIp), GameSession.Instance.MatchPort);

		_listener = new PacketListner(_channel, _socket);
		_listener.Start();
		
		var packet = new C2S_HandshakePacket
		{
			SessionId = GameSession.Instance.MatchId,
			UserId = GameSession.Instance.UserId,
			Nickname = GameSession.Instance.Nickname
		};

		Span<byte> buffer = stackalloc byte[256];

		int bytesWriten = PacketTranslator.Write(PacketTypes.C2S_Handshake, buffer, ref packet);
		_socket.Send(buffer[..bytesWriten]);


	}

	public override void _Process(double delta)
	{
		
		while(_channel.Reader.TryRead(out NetworkPacket packet)) 
		{

			bool isReturned = false;
			try
			{
				
				switch(packet.Type)
				{
				
					case PacketTypes.S2C_HandshakeResponse:
						{
							var pak = new S2C_HandshakeResponse();
							PacketTranslator.Read(packet.Buffer[..packet.PayloadLength], ref pak);

							GameSession.Instance.LocalPlayerId = pak.LocalPlayerId;
							OnGameReadyPlayersUpdate?.Invoke(pak.TotalPlayersCount, pak.ActivePlayersCount);


							ArrayPool<byte>.Shared.Return(packet.Buffer);
							isReturned = true;
						}
					break;

					case PacketTypes.S2C_MatchReadyUpdate:
						{
							var pak = new S2C_MatchReadyUpdate();
							PacketTranslator.Read(packet.Buffer[..packet.PayloadLength], ref pak);

							OnGameReadyPlayersUpdate?.Invoke(pak.TotalPlayersCount, pak.ActivePlayersCount);

							ArrayPool<byte>.Shared.Return(packet.Buffer);
							isReturned = true;
						}
					break;

					case PacketTypes.S2C_LobbyJoinRequest:
						{
							S2C_LobbyJoinRequestPacket pak = default;
							PacketTranslator.Read(packet.Buffer[..packet.PayloadLength], ref pak);

							OnSwithToLobby?.Invoke(pak);

							ArrayPool<byte>.Shared.Return(packet.Buffer);
							isReturned = true;
						}
					break;

					case PacketTypes.S2C_LobbyPlayerHeroChanged:
						{
							
							S2C_LobbyPlayerHeroChangedPacket pak = default;
							PacketTranslator.Read(packet.Buffer[..packet.PayloadLength], ref pak);

							OnLobbyPlayerChanged?.Invoke(pak);

							ArrayPool<byte>.Shared.Return(packet.Buffer);
							isReturned = true;

						}
					break;

					case PacketTypes.S2C_PlayersSpawn: 
						_gamePacketQueue.Enqueue(packet); 
						break;

			}

			}
			finally
			{
				if(!isReturned)
				{
					ArrayPool<byte>.Shared.Return(packet.Buffer);
				}
			}

			

		}

	}

	public void Lobby_SendHeroChange(C2S_LobbyChangeHeroRequestPacket packet)
	{
		
		Span<byte> buffer = stackalloc byte[16];
		int writenBytes = PacketTranslator.Write(PacketTypes.C2S_LobbyChangeHeroRequest, buffer, ref packet);

		_socket.Send(buffer[..writenBytes]);

	}



	public bool TryGetGamePacket(out NetworkPacket packet)
	{
		return _gamePacketQueue.TryDequeue(out packet);
	}


	public override void _ExitTree()
	{
		_socket?.Close();
		_socket?.Dispose();
	}


}
