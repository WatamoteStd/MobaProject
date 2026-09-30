using Moba.Matchmaker.Entities;
using Moba.Shared.MatchmakerLibs;

namespace Moba.Matchmaker.Core;

public class MatchmakingEngine
{
    
    public int BaseMmrRange {get; private set;}
    public int MmrExpansionPerSecound {get; private set;}
    public int MaxMmrRange {get; set;} = 0; // defaulth == no limit

    public MatchmakingEngine(int baseRange, int mmrExpansion, int maxMmrRange = 0)
    {
        
        BaseMmrRange = baseRange;
        MmrExpansionPerSecound = mmrExpansion;
        MaxMmrRange = maxMmrRange;

    }

    public List<MatchResult> FindMatch(MatchProperty mode, List<PoolPlayer> snapshot)
    {
        
        if (snapshot.Count < mode.GetMinPlayers())
        {
            return new List<MatchResult>();
        }

        return mode switch
        {
            MatchProperty.Solo => FindSoloMatch(snapshot),
            MatchProperty.Trio => FindTrioMatch(snapshot),
            MatchProperty.Full => FindFullMatch(snapshot),
            _ => []
        };

    }

    // No sort bcs they defaulth added by "JoinedAt"
    private List<MatchResult> FindSoloMatch(List<PoolPlayer> snapshot)
    {

        var matches = new List<MatchResult>();
        
        var used = new HashSet<long>();

        for (int i = 0; i < snapshot.Count; i++) // search the 'Captain1'
        {
            
            var cur = snapshot[i];

            if (used.Contains(cur.PlayerId)) continue;

            double secound = (DateTime.UtcNow - cur.JoinedQueue).TotalSeconds;

            int delta = BaseMmrRange + (int)(secound * MmrExpansionPerSecound);

            if (MaxMmrRange > 0)
            {
                delta = Math.Min(delta, MaxMmrRange);
            }

            for (int j = i + 1; j < snapshot.Count; j++)
            {
                
                var candidat = snapshot[j];

                if (used.Contains(candidat.PlayerId)) continue;

                int diff = Math.Abs(cur.MMR - candidat.MMR);

                if (diff <= delta)
                {
                    
                    used.Add(cur.PlayerId);
                    used.Add(candidat.PlayerId);

                    matches.Add(new MatchResult
                    {
                        Players = [cur, candidat],
                        Mode = MatchProperty.Solo
                    });

                    break;

                }

            }

        }

        return matches;

    }

    private List<MatchResult> FindTrioMatch(List<PoolPlayer> snapshot)
    {
        
        var used = new HashSet<long>();
        var matches = new List<MatchResult>();

        for (int i = 0; i < snapshot.Count; i++)
        {
            
            var cur = snapshot[i];

            if (used.Contains(cur.PlayerId)) continue;

            var party = new List<PoolPlayer> {cur};

            double secound = (DateTime.UtcNow - cur.JoinedQueue).TotalSeconds;
            int delta = BaseMmrRange + (int)(secound * MmrExpansionPerSecound);

            if (MaxMmrRange > 0)
            {
                delta = Math.Min(delta, MaxMmrRange);
            }

            for (int j = i + 1; j < snapshot.Count; j++)
            {
                
                var another = snapshot[j];

                if (used.Contains(another.PlayerId)) continue;

                int diffrense = Math.Abs(another.MMR - cur.MMR);

                if (diffrense <= delta)
                {
    
                    party.Add(another);

                    if (party.Count == MatchProperty.Trio.GetMinPlayers())
                    {
                        matches.Add(new MatchResult
                        {
                            Players = party.ToArray(),
                            Mode = MatchProperty.Trio
                        });

                        foreach(var a in party)
                        {
                            used.Add(a.PlayerId);
                        }

                        break;
                    }

                }

            }

        }

        return matches;
        
    }

    private List<MatchResult> FindFullMatch(List<PoolPlayer> snapshot)
    {
        
        var used = new HashSet<long>();
        var matches = new List<MatchResult>();

        for (int i = 0; i < snapshot.Count; i++)
        {
            
            var cur = snapshot[i];

            if (used.Contains(cur.PlayerId)) continue;

            var party = new List<PoolPlayer> {cur};

            double secound = (DateTime.UtcNow - cur.JoinedQueue).TotalSeconds;
            int delta = BaseMmrRange + (int)(secound * MmrExpansionPerSecound);

            if (MaxMmrRange > 0)
            {
                delta = Math.Min(delta, MaxMmrRange);
            }

            for (int j = i + 1; j < snapshot.Count; j++)
            {
                
                var another = snapshot[j];

                if (used.Contains(another.PlayerId)) continue;

                int diffrense = Math.Abs(another.MMR - cur.MMR);

                if (diffrense <= delta)
                {
    
                    party.Add(another);

                    if (party.Count == MatchProperty.Full.GetMinPlayers())
                    {
                        matches.Add(new MatchResult
                        {
                            Players = party.ToArray(),
                            Mode = MatchProperty.Full
                        });

                        foreach(var a in party)
                        {
                            used.Add(a.PlayerId);
                        }

                        break;
                    }

                }

            }

        }

        return matches;

    }

}