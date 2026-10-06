
namespace Moba.Shared.MatchmakerLibs.MatchQueue;

public readonly record struct QueuePlayerStatusResponse(QueuePlayerStatus Status, string Ip, int Port, Guid MatchId);
