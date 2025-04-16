namespace WebApi.UseCases.Commands.Accounts.CreateAccount;

public record CreateAccountCommand(string Email, string Name, string? Password) 
{
    public string? FirebaseUserId { get; init; }
    public string? PhotoUrl { get; init; }
};