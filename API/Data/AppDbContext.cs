using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<ToDo> ToDos { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public DbSet<AuditTrail> AuditTrails { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}