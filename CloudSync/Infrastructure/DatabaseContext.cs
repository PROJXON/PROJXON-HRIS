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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseNpgsql()
    .UseSnakeCaseNamingConvention();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<InvitedUser>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<UserRole>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = 0, Name = "Administrator" },
            new UserRole { Id = 1, Name = "Human Resources" },
            new UserRole { Id = 2, Name = "Employee" },
            new UserRole { Id = 4, Name = "Guest" }
        );
        
        modelBuilder.Entity<Permission>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Employee>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Department>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 0, Name = "Business" },
            new Department { Id = 6, Name = "Executive", ParentDepartmentId = 0},
            new Department { Id = 7, Name = "Consulting", ParentDepartmentId = 0},
            
            new Department { Id = 1, Name = "Human Resources" },
            new Department { Id = 8, Name = "Onboarding", ParentDepartmentId = 1},
            new Department { Id = 9, Name = "Offboarding", ParentDepartmentId = 1 },
            new Department { Id = 10, Name = "Recruiting", ParentDepartmentId = 1 },
            
            new Department { Id = 2, Name = "Information Technology" },
            new Department { Id = 11, Name = "AI", ParentDepartmentId = 2 },
            new Department { Id = 12, Name = "Applications Development", ParentDepartmentId = 2 },
            new Department { Id = 13, Name = "Cybersecurity", ParentDepartmentId = 2 },
            new Department { Id = 14, Name = "SEO", ParentDepartmentId = 2 },
            new Department { Id = 15, Name = "Web Development", ParentDepartmentId = 2 },
            
            new Department { Id = 3, Name = "Marketing" },
            new Department { Id = 16, Name = "Market Research", ParentDepartmentId = 3 },
            new Department { Id = 17, Name = "Social Media", ParentDepartmentId = 3 },
            new Department { Id = 18, Name = "Copywriting", ParentDepartmentId = 3 },
            new Department { Id = 19, Name = "Graphic Design", ParentDepartmentId = 3 },
            
            new Department { Id = 4, Name = "Operations" },
            new Department { Id = 20, Name = "Finance", ParentDepartmentId = 4 },
            new Department { Id = 21, Name = "Legal", ParentDepartmentId = 4 }
        );
        
        modelBuilder.Entity<ProjectTeam>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<TeamMember>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Candidate>()
            .HasKey(u => u.Id);
    }
}