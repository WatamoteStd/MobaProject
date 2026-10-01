using Moba.Shared.MatchmakerLibs;

namespace Moba.Shared.MatchmakerLibs;

public readonly record struct EnqueuePlayerMessage(long PlayerId, int MMR, MatchProperty Mode);