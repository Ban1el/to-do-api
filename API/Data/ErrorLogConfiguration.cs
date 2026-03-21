using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using API.Models;

namespace API.Data.Configurations;

public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> entity)
    {
        entity.ToTable("ErrorLogs");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.UserId).HasDefaultValue(0);
        entity.Property(e => e.Description);
        entity.Property(e => e.ClientIpAddress).HasMaxLength(45);
        entity.Property(u => u.DateCreated).IsRequired();
    }
}