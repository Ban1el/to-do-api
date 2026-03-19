using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using API.Models;

namespace API.Data.Configurations;

public class ToDoConfiguration : IEntityTypeConfiguration<ToDo>
{
    public void Configure(EntityTypeBuilder<ToDo> entity)
    {
        entity.ToTable("ToDos");
        entity.HasKey(t => t.Id);
        entity.Property(t => t.Id).ValueGeneratedOnAdd();
        entity.Property(t => t.Title).IsRequired().HasMaxLength(50);
        entity.Property(t => t.Description);
        entity.Property(t => t.DateCreated).IsRequired();
        entity.Property(t => t.DateModified);
        entity.Property(t => t.IsActive).HasDefaultValue(true);
    }
}