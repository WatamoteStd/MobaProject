
namespace Moba.Shared.UdpServer.Packets;

public enum PacketTypes : ushort
{
    
    Pint = 0,
    Pong = 1,
    C2S_Handshake = 2,
    S2C_HandshakeResponse = 3,
    S2C_MatchReadyUpdate = 4,
    S2C_LobbyJoinRequest = 5,
    C2S_LobbyChangeHeroRequest = 6,
    S2C_LobbyPlayerHeroChanged = 7,
    S2C_PlayersSpawn = 8,
    C2S_MoveRequest = 9,
    S2C_EntityMoveStart = 10,
    S2C_EntityMoveSync = 11

}