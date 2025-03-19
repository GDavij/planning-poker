using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Mappings;

internal class AccountMapping : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.AccountId)
            .HasName("PK_Accounts_AccountId");

        builder.Property(a => a.Email)
            .HasMaxLength(60)
            .HasColumnName("Email");

        builder.Property(a => a.Name)
            .HasMaxLength(80)
            .HasColumnName("Name");

        builder.Property(a => a.Deleted)
            .HasColumnName("Deleted")
            .HasDefaultValue(false);

        builder.Property(a => a.IsAdmin)
            .HasColumnName("IsAdmin")
            .HasDefaultValue(false);

        builder.HasMany<Match>(a => a.Matches)
            .WithOne(m => m.Account)
            .HasForeignKey(m => m.AccountId)
            .HasConstraintName("FK_Accounts_Matches_AccountId");
        
        builder.HasMany<Participant>(a => a.Participants)
            .WithOne(p => p.Account)
            .HasForeignKey(p => p.AccountId)
            .HasConstraintName("FK_Accounts_Participants_AccountId");
    }
}