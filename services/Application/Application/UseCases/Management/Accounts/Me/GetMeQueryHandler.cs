using Domain.Abstractions.Auth.Models;

namespace Application.UseCases.Management.Accounts.Me;

public record GetMeQueryResponse(long AccountId);

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