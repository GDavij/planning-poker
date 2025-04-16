namespace WebApi.UseCases.Commands.Matches.JoinMatch;

public record JoinMatchCommand(long MatchId, string ConnectionId);