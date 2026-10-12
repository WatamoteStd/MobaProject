using Godot;
using Moba.Shared.UdpServer.Heroes;
using Moba.Shared.UdpServer.Packets.Lobby;
using Moba.Shared.UdpServer.Teams;
using System;
using System.Collections.Generic;

public partial class LobbyCache : Node
{
    
    public Dictionary<ushort, LobbyCachedPlayer> IdToPlayerCache = new();

    public void AddPlayer(ushort id, string nickname, MatchTeams team, MatchHeroes hero)
    {
        
        IdToPlayerCache[id] = new LobbyCachedPlayer { PlayerId = id, Nickname = nickname, PlayerTeam = team, SelectedHero = hero};

    }
    public void UpdatePlayerHero(ushort id, MatchHeroes hero)
    {
        
        if(IdToPlayerCache.TryGetValue(id, out var cache))
        {
            cache.SelectedHero = hero;
        }

    }

}
