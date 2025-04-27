using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Mappings;

internal class StoryPointMapping : IEntityTypeConfiguration<StoryPoint>
{
    public void Configure(EntityTypeBuilder<StoryPoint> builder)
    {
        builder.ToTable("StoryPoints");

        builder.HasKey(sp => new { sp.StoryId, sp.MatchId, sp.AccountId })
            .HasName("PK_StoryPoints_StoryId_MatchId_AccountId");

        builder.Property(sp => sp.AccountId)
            .HasColumnName("AccountId");
        
        builder.Property(sp => sp.StoryId)
            .HasColumnName("StoryId");

        builder.Property(sp => sp.MatchId)
            .HasColumnName("MatchId");

        builder.HasOne(sp => sp.Participant)
            .WithMany(p => p.StoryPoints)
            .HasForeignKey(sp => new { sp.AccountId, sp.MatchId })
            .HasConstraintName("FK_Participant_StoryPoint_AccountId_StoryId")
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Property(sp => sp.Points)
            .HasColumnName("Points")
            .IsRequired();
    }
}