using MediatR;

namespace Domain.Commands.Matches.CreateMatch;

public record CreateMatchCommand(string Description) : IRequest<CreateMatchCommandResponse>;