using Moba.Shared.MatchmakerLibs;

namespace Moba.Shared.OrchestratorData;

public record struct MatchCreateInfo(
    Guid MatchId,
    MatchProperty GameMode,
    long[] PlayerIds,
    int Port,
    string ServerIp
);