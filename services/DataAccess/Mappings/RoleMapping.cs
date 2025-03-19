using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Mappings;

internal class RoleMapping : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        
        builder.HasKey(r => r.RoleId)
            .HasName("PK_Roles_RoleId");

        builder.Property(r => r.Name)
            .HasMaxLength(40)
            .HasColumnName("Name");
        
        builder.Property(r => r.Abbreviation)
            .HasMaxLength(10)
            .HasColumnName("Abbreviation");
        
        builder.HasMany<Participant>(r => r.Participants)
            .WithOne(p => p.Role)
            .HasForeignKey(p => p.RoleId)
            .HasConstraintName("FK_Roles_Participants_RoleId");
    }
}