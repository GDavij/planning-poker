using Domain.Abstractions.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace WebApi.UseCases.Queries.Matches.ListParticipants;

public class ListParticipantsQueryHandler
{
    private readonly IApplicationDbContext _dbContext;

    public ListParticipantsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<ListParticipantsQueryResponse>> Handle(long matchId, CancellationToken cancellationToken)
    {
        return _dbContext.Participants.Include(p => p.Role)
                                      .Include(p => p.Account)
                                      .Include(p => p.Match)
                                        .ThenInclude(m => m.Stories)
                                            .ThenInclude(s => s.StoryPoints)
                                      .Where(p => p.MatchId == matchId)
                                      .Select(p => new ListParticipantsQueryResponse
                                      {
                                          IsSpectating = p.IsSpectating,
                                          AccountId = p.AccountId,
                                          ParticipantName = p.Account.Name,
                                          RoleName = p.Role.Name,
                                          Votes = p.Match.Stories.Select(s => new VoteWithoutPoints(s.StoryId, s.StoryPoints.Any(sp => sp.AccountId == p.AccountId)))
                                                                 .ToList()
                                      }).ToListAsync(cancellationToken);
    }
}