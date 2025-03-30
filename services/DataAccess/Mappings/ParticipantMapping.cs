using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Mappings;

internal class ParticipantMapping : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("Participants");

        builder.HasKey(p => new { p.AccountId, p.MatchId })
            .HasName("PK_Participants_AccountId_MatchId");

        builder.Property(p => p.IsSpectating)
            .HasColumnName("IsSpectating")
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.HasOne<Account>(p => p.Account)
            .WithMany(a => a.Participants)
            .HasForeignKey(p => p.AccountId)
            .HasConstraintName("FK_Participants_Accounts_AccountId");
        
        builder.HasOne<Role>(p => p.Role)
            .WithMany(r => r.Participants)
            .HasForeignKey(p => p.RoleId)
            .HasConstraintName("FK_Participants_Roles_RoleId");
        
        builder.HasOne<Match>(p => p.Match)
            .WithMany(m => m.Participants)
            .HasForeignKey(p => p.MatchId)
            .HasConstraintName("FK_Participants_Matches_MatchId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany<StoryPoint>(p => p.StoryPoints)
            .WithOne(sp => sp.Participant)
            .HasForeignKey(sp => new { sp.AccountId, sp.MatchId })
            .HasConstraintName("FK_Participants_StoryPoints_AccountId_MatchId");
    }
}