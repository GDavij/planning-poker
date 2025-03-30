using Domain.Entities;

namespace Domain.Abstractions.Auth;

public interface IAuthenticationService
{
    Task<bool> StoreAccount(Account account);

    Task<bool> HaveAccountWithEmail(string email);

    Task<Guid> GenerateAuthCode(TimeSpan expiresIn, string customLabel);

    Task<bool> HasValidAuthCode(Guid code, string customLabel);
}