using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Microsoft.AspNetCore.Http;

namespace Firebase.Auth;

public class FirebaseUser : ICurrentAccount
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FirebaseUser(IApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    private  long _accountId = 0;

    public long AccountId
    {
        get
        {
            if (_accountId == 0)
            {
                var accountId = _httpContextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == "user_id")!.Value;

                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return 0;
                }
                
                var validAccountId = _dbContext.Accounts.First(a => a.FirebaseUserId == accountId).AccountId;

                _accountId = validAccountId;
            }

            return _accountId;
        }
    }
}