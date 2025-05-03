using Domain.Abstractions.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Stories.ListStories;

public record ListStoriesQueryResponse
{
    public long StoryId { get; init; }
    public long MatchId { get; init; }
    public string Name { get; init; }
    public string? StoryNumber { get; init; }
    public short Order { get; init; }
    public List<StoryPointResponse> StoryPoints { get; init; }
};

public record StoryPointResponse(short Points, string ParticipantName);

public class ListStoriesQueryHandler
{
    private readonly IApplicationDbContext _dbContext;
    
    public ListStoriesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ListStoriesQueryResponse>> Handle(long matchId, CancellationToken cancellationToken)
    {
        return await _dbContext.Stories.Include(s => s.StoryPoints)
                                            .ThenInclude(sp => sp.Participant)
                                                .ThenInclude(p => p.Account)
                                       .Where(s => s.MatchId == matchId)
                                       .OrderBy(s => s.Order)
                                       .Select(s => new ListStoriesQueryResponse
                                       {
                                           Name = s.Name,
                                           MatchId = s.MatchId,
                                           Order = s.Order,
                                           StoryNumber = s.StoryNumber,
                                           StoryId = s.StoryId,
                                           StoryPoints = s.StoryPoints.Select(sp => new StoryPointResponse(sp.Points, sp.Participant.Account.Name))
                                                                      .ToList()
                                       })
                                       .ToListAsync(cancellationToken);
    }
}
