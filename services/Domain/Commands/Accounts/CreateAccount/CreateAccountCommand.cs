using MediatR;

namespace Domain.Commands.Accounts.CreateAccount;

public record CreateAccountCommand(string Email, string Name, string? Password) 
    : IRequest<CreateAccountCommandResponse>
{
    public string? FirebaseUserId { get; init; }
    public string? PhotoUrl { get; init; }
};