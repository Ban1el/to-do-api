using System;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data;

public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    public void Configure(EntityTypeBuilder<AuditTrail> entity)
    {
        entity.ToTable("AuditTrails");
        entity.HasKey(a => a.Id);
        entity.Property(a => a.Id).ValueGeneratedOnAdd();

        entity.Property(a => a.UserId).HasDefaultValue(0);

        entity.Property(a => a.Module)
            .HasMaxLength(100);

        entity.Property(a => a.Action)
            .HasMaxLength(100);

        entity.Property(a => a.Method)
            .HasMaxLength(100);


        entity.Property(a => a.Description)
            .HasMaxLength(500);

        entity.Property(a => a.Data)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        entity.Property(a => a.ClientIpAddress)
            .IsRequired()
            .HasMaxLength(45); // supports IPv6

        entity.Property(a => a.IsRequest)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(a => a.Path)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(a => a.RefId);

        entity.Property(a => a.DateCreated)
            .IsRequired();
    }
}
