using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Mappings;

internal class StoryMapping : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.ToTable("Stories");
        
        builder.HasKey(st => st.StoryId)
            .HasName("PK_Stories_StoryId");
        
        builder.HasOne<Match>(st => st.Match)
            .WithMany(m => m.Stories)
            .HasForeignKey(st => st.MatchId)
            .HasConstraintName("FK_Stories_Matches_MatchId");
        
        builder.Property(st => st.Name)
            .HasMaxLength(120)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(st => st.StoryNumber)
            .HasMaxLength(20)
            .HasColumnName("StoryNumber");

        builder.Property(st => st.Order)
            .HasColumnName("Order")
            .IsRequired();
    }
}