using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Mappings;

internal class MatchMapping : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("Matches");
        
        builder.HasKey(m => m.MatchId)
            .HasName("PK_Matches_MatchId");

        builder.Property(m => m.Description)
            .HasMaxLength(120)
            .HasColumnName("Description");

        builder.HasOne<Account>(m => m.Account)
            .WithMany(a => a.Matches)
            .HasForeignKey(m => m.AccountId)
            .HasConstraintName("FK_Matches_Accounts_AccountId");
    }
}