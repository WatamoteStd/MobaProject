
using System.Net;
using Moba.Shared.UdpServer.Heroes;
using Moba.Shared.UdpServer.Teams;

namespace Players;

public struct PlayerConnection
{
    
    public ushort Id;
    public bool IsConnected;
    public EndPoint EndPoint;
    public string Nickname;
    public long PlayerId;
    public MatchTeams Team;
    public MatchHeroes SelectedHero;

}