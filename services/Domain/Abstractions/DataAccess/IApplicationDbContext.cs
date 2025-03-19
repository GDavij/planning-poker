using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Match = System.Text.RegularExpressions.Match;

namespace Domain.Abstractions.DataAccess;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; init; }
    DbSet<Match> Matches { get; init; }
    DbSet<Participant> Participants { get; init; }
    DbSet<Role> Roles { get; init; }
    DbSet<Story> Stories { get; init; }
    DbSet<StoryPoint> StoryPoints { get; init; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}