using Moba.Shared.MatchmakerLibs;

namespace DTOs;

public readonly record struct EnqueuePlayerMessage(long PlayerId, int MMR, MatchProperty Mode);