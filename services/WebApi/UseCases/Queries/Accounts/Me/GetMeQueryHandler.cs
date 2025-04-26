using Domain.Abstractions.Auth.Models;

namespace WebApi.UseCases.Queries.Accounts.Me;

public class GetMeQueryHandler
{
    private readonly ICurrentAccount _currentAccount;

    public GetMeQueryHandler(ICurrentAccount currentAccount)
    {
        _currentAccount = currentAccount;
    }

    public Task<GetMeQueryResponse> Handle()
    {
        return Task.FromResult(new GetMeQueryResponse(_currentAccount.AccountId));
    }
}