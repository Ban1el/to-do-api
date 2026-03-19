using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<ToDo> ToDos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDo>(entity =>
        {
            entity.ToTable("ToDos");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).ValueGeneratedOnAdd();
            entity.Property(t => t.Title).IsRequired().HasMaxLength(50);
            entity.Property(t => t.Description);
            entity.Property(t => t.DateCreated).IsRequired();
            entity.Property(t => t.DateModified);
            entity.Property(t => t.IsActive).HasDefaultValue(true);
        });
    }
}
