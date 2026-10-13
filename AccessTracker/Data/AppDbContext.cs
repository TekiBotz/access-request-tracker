using AccessTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessTracker.Data;

// Bridge between the models and the database.
// Each DbSet turns into a table.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Resource> Resources { get; set; }
    public DbSet<AccessRequest> AccessRequests { get; set; }
    public DbSet<AuditEntry> AuditEntries { get; set; }


    // Add starter rows to the Resources table.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Resource>().HasData(
            new Resource { Id = 1, Name = "Finance Shared Drive", Description = "Accounting files" },
            new Resource { Id = 2, Name = "HR Portal", Description = "Employee records app" }
        );
    }
}