using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Abstractions.DataAccess;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; init; }
    DbSet<Match> Matches { get; init; }
    DbSet<Participant> Participants { get; init; }
    DbSet<Role> Roles { get; init; }
    DbSet<Story> Stories { get; init; }
    DbSet<StoryPoint> StoryPoints { get; init; }
    
    ChangeTracker ChangeTracker { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}