using Microsoft.EntityFrameworkCore;
using CloudSync.Modules.EmployeeManagement.Models;
using CloudSync.Modules.UserManagement.Models;

namespace CloudSync.Infrastructure;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<UserRole>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Permission>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Employee>()
            .HasKey(u => u.Id);
    }
}