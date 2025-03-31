using MediatR;

namespace Domain.Commands.Matches.JoinMatch;

public record JoinMatchCommand(long matchId, Guid? authGuid) : IRequest<JoinMatchCommandResponse>;