using Godot;
using Moba.Shared.UdpServer.Heroes;
using Moba.Shared.UdpServer.Teams;
using System;

public partial class LobbyCachedPlayer : Node
{
    
    public ushort PlayerId;
    public MatchHeroes SelectedHero;
    public MatchTeams PlayerTeam;
    public string Nickname;

}
