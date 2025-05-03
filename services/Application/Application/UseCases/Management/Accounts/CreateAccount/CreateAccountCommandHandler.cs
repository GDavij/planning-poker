using Domain.Abstractions;
using Domain.Abstractions.Auth;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Management.Accounts.CreateAccount;

public record CreateAccountCommand
{
    public string Email { get; init; }
    public string Name { get; init; }
    public string? Password { get; init; }
    public string? FirebaseUserId { get; init; }
    public string? PhotoUrl { get; init; }
    
    private CreateAccountCommand() 
    { }

    public CreateAccountCommand(string email, string name, string? password)
    {
        Email = email;
        Name = name;
        Password = password;
    }
}

public record CreateAccountCommandResponse(long AccountId);

public class CreateAccountCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IAuthenticationService _authService;
    private readonly INotificationService _notificationService;

    public CreateAccountCommandHandler(
        IApplicationDbContext dbContext,
        IAuthenticationService authService,
        INotificationService notificationService)
    {
        _dbContext = dbContext;
        _authService = authService;
        _notificationService = notificationService;
    }

    public async Task<CreateAccountCommandResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var existsAnyAccountWithSameEmail = await _dbContext.Accounts.AnyAsync(a => a.Email == request.Email && !a.Deleted, cancellationToken);
        if (existsAnyAccountWithSameEmail && string.IsNullOrEmpty(request.FirebaseUserId))
        {
            _notificationService.AddNotification("Account with this email already exists", "account.exists");
            return null;
        }
        else if (existsAnyAccountWithSameEmail)
        {
            var currentAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email && !a.Deleted, cancellationToken);
            currentAccount.UseFirebaseIdentity(request.FirebaseUserId);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return new CreateAccountCommandResponse(currentAccount.AccountId);
        }

        var account = new Account(request.Email, request.Name);

        if (!string.IsNullOrEmpty(request.PhotoUrl))
        {
            account.TakePhoto(request.PhotoUrl);
        }
        
        _dbContext.Accounts.Add(account);
        
        if (!string.IsNullOrEmpty(request.FirebaseUserId))
        {
            account.UseFirebaseIdentity(request.FirebaseUserId);
        }
        
        
        if (request.Password is not null)
        {
            account.SecureWithPassword(request.Password);
        
            await _authService.StoreAccount(account);
        }

        await _dbContext.SaveChangesAsync(CancellationToken.None);

        return new CreateAccountCommandResponse(account.AccountId);
    }

}