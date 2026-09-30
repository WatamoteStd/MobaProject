using Moba.Matchmaker.Entities;
using Moba.Shared.MatchmakerLibs;

namespace Moba.Matchmaker.Core;

public struct MatchResult
{
    
    public  PoolPlayer[] Players {get; init;}
    public  MatchProperty Mode {get; init;}

}