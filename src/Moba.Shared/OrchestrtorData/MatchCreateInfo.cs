using Moba.Shared.MatchmakerLibs;

namespace Moba.Shared.OrchestratorData;

public readonly record struct MatchCreateInfo(
    uint MatchId,
    MatchProperty GameMode,
    long[] PlayerIds
);