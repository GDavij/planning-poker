using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace WebApi.UseCases.Queries.Matches.ListMatches;

public class ListMatchesQueryHandler 
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentAccount _currentAccount;

    public ListMatchesQueryHandler(IApplicationDbContext dbContext, ICurrentAccount currentAccount)
    {
        _dbContext = dbContext;
        _currentAccount = currentAccount;
    }
    
    public Task<List<ListMatchesQueryResponse>> Handle(ListMatchesQuery request, CancellationToken cancellationToken)
    {
        return _dbContext.Matches.Include(m => m.Participants)
                                 .Where(m => m.Participants.Any(p => p.AccountId == _currentAccount.AccountId) && !m.HasClosed)
                                 .Select(m => new ListMatchesQueryResponse
                                                    {
                                                        MatchId = m.MatchId,
                                                        Description = m.Description,
                                                        HasStarted = m.HasStarted,
                                                        HasClosed = m.HasClosed
                                                    })
                                 .Skip((request.Page - 1) * request.Limit)
                                 .Take(request.Limit)
                                 .ToListAsync(cancellationToken);
    }
}