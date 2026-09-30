namespace Moba.Shared.MatchmakerLibs;

public enum MatchProperty : byte {Full = 0, Trio = 1, Solo = 2}

public static class MatchPropertyExtension
{
    
    public static int GetMinPlayers(this MatchProperty mode) => mode switch
    {
        MatchProperty.Full => 10,
        MatchProperty.Trio => 6,
        MatchProperty.Solo => 2,
        _ => throw new ArgumentOutOfRangeException(nameof(mode), $"Unknown match property: {mode}")
    };

}