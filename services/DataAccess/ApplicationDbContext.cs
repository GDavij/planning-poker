using DataAccess.Mappings;
using DataAccess.Seeders;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

internal class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Account> Accounts { get; init; }
    public DbSet<Match> Matches { get; init; }
    public DbSet<Participant> Participants { get; init; }
    public DbSet<Role> Roles { get; init; }
    public DbSet<Story> Stories { get; init; }
    public DbSet<StoryPoint> StoryPoints { get; init; }

    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountMapping())
            .ApplyConfiguration(new MatchMapping())
            .ApplyConfiguration(new ParticipantMapping())
            .ApplyConfiguration(new RoleMapping())
            .ApplyConfiguration(new StoryMapping())
            .ApplyConfiguration(new StoryPointMapping());
        
        SeedData(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        new RoleSeeder().SeedForBuilder(modelBuilder);
    }
}