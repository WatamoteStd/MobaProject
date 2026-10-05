using Moba.Shared.MatchmakerLibs;

namespace Moba.Shared.OrchestratorData;

public readonly record struct MatchCreateInfo(
    Guid MatchId,
    MatchProperty GameMode,
    long[] PlayerIds
);