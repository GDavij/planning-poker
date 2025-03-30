using MediatR;

namespace Domain.Commands.Matches.TakePartOfMatch;

public record TakePartOfMatchCommand(long MatchId, byte RoleId, bool ShouldSpectate, Guid AuthGuid) : IRequest<TakePartOfMatchCommandResponse>;