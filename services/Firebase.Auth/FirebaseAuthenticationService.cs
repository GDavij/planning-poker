using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Domain.Abstractions.Auth;
using Domain.Entities;

namespace Firebase.Auth;

public class FirebaseAuthenticationService : IAuthenticationService
{
    private IDictionary<Guid, AuthCodeRegistry> AuthCodes { get; } = new Dictionary<Guid, AuthCodeRegistry>();  
    
    public Task<bool> StoreAccount(Account account)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HaveAccountWithEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> GenerateAuthCode(TimeSpan expiresIn, string customLabel)
    {
        var authCode = Guid.NewGuid();

        var hashedCustomLabel = Encoding.ASCII.GetString(SHA256.HashData(Encoding.ASCII.GetBytes(customLabel)));
        
        AuthCodes.Add(authCode, new AuthCodeRegistry(DateTime.UtcNow, expiresIn, hashedCustomLabel));

        return Task.FromResult(authCode);
    }

    public Task<bool> HasValidAuthCode(Guid code, string customLabel)
    {
        var hashedCustomLabel = Encoding.ASCII.GetString(SHA256.HashData(Encoding.ASCII.GetBytes(customLabel)));
        
        if (AuthCodes.TryGetValue(code, out AuthCodeRegistry? authCodeRegistry))
        {
            return Task.FromResult(authCodeRegistry.IsAlive() &&
                                   authCodeRegistry.CustomLabel == hashedCustomLabel);
        }

        return Task.FromResult(false);
    }
}

public record AuthCodeRegistry(DateTime CreatedAt, TimeSpan ExpiresIn, string CustomLabel)
{
    public bool IsAlive() => CreatedAt.Add(ExpiresIn) >= DateTime.UtcNow;
};