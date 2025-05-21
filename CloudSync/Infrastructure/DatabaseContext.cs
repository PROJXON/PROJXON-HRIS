using Microsoft.EntityFrameworkCore;
using CloudSync.Modules.EmployeeManagement.Models;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.CandidateManagement.Models;

namespace CloudSync.Infrastructure;

public class DatabaseContext : DbContext
{
    public DatabaseContext() : base()
    {}
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<InvitedUser> InvitedUsers { get; set; }
    public virtual DbSet<UserRole> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<UserRole>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<InvitedUser>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Permission>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Employee>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Department>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<ProjectTeam>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<TeamMember>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Candidate>()
            .HasKey(u => u.Id);
    }
}