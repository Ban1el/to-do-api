using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using API.Models;


namespace API.Data;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("Users");
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Id).ValueGeneratedOnAdd();
        entity.Property(u => u.Username)
        .IsRequired()
        .HasMaxLength(50);
        entity.Property(u => u.Email)
        .IsRequired()
        .HasMaxLength(100);
        entity.Property(u => u.Password)
        .IsRequired();
        entity.Property(u => u.PasswordSalt)
        .IsRequired();
        entity.Property(u => u.DateCreated)
        .IsRequired();
        entity.Property(u => u.DateModified)
        .IsRequired(false);
        entity.Property(u => u.CreatedBy)
        .IsRequired();
        entity.Property(u => u.ModifiedBy)
        .IsRequired();
        entity.Property(u => u.IsActive)
        .HasDefaultValue(true);
    }
}