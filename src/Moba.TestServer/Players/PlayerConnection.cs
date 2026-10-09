
using System.Net;

namespace Players;

public struct PlayerConnection
{
    
    public int Id;
    public bool IsConnected;
    public EndPoint EndPoint;
    public string Nickname;
    public long PlayerId;

}