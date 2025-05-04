using System.ComponentModel.DataAnnotations;
using System.Net;
using Domain.Abstractions;
using Domain.Abstractions.Auth;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Management.Accounts.CreateAccount;

public record CreateAccountCommand
{
    [EmailAddress(ErrorMessage = "Email must be valid")]
    [MaxLength(60, ErrorMessage = "Email must have a max of 60 characters")]
    [Required(ErrorMessage = "Email is Required")]
    public string Email { get; init; }
    
    [MaxLength(80, ErrorMessage = "Name must have a max of 80 characters")]
    [Required(ErrorMessage = "Name is Required")]
    public string Name { get; init; }
    
    public string? Password { get; init; }
    public string? FirebaseUserId { get; init; }
    public string? PhotoUrl { get; init; }
    
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

    public async Task<CreateAccountCommandResponse?> Handle(CreateAccountCommand request)
    {
        var existentAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email && !a.Deleted);
        if (existentAccount is not null && string.IsNullOrEmpty(request.FirebaseUserId))
        {
            _notificationService.AddNotification("Account with this email already exists", "account.exists", HttpStatusCode.NotFound);
            return null;
        }
        else if (existentAccount is not null)
        {
            existentAccount.UseFirebaseIdentity(request.FirebaseUserId!);

            await _dbContext.SaveChangesAsync();
            return new CreateAccountCommandResponse(existentAccount.AccountId);
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

        await _dbContext.SaveChangesAsync();

        return new CreateAccountCommandResponse(account.AccountId);
    }

}