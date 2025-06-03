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
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Address> Addresses { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<ProjectTeam> ProjectTeams { get; set; }
    public virtual DbSet<TeamMember> TeamMembers { get; set; }

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
            new UserRole { Id = 1, Name = "Administrator" },
            new UserRole { Id = 2, Name = "Human Resources" },
            new UserRole { Id = 3, Name = "Employee" },
            new UserRole { Id = 4, Name = "Guest" }
        );
        
        modelBuilder.Entity<Permission>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Employee>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<Employee>().OwnsOne(e => e.BasicInfo);
        modelBuilder.Entity<Employee>().OwnsOne(e => e.ContactInfo);
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.PositionDetails)
            .WithOne()
            .HasForeignKey<Employee>(e => e.EmployeeDetailsId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Documents)
            .WithOne()
            .HasForeignKey<Employee>(e => e.EmployeeDocumentsId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Legal)
            .WithOne()
            .HasForeignKey<Employee>(e => e.EmployeeLegalId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Education)
            .WithOne()
            .HasForeignKey<Employee>(e => e.EmployeeEducationId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Training)
            .WithOne()
            .HasForeignKey<Employee>(e => e.EmployeeTrainingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Address>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<EmployeePosition>().HasKey(u => u.Id);
        
        modelBuilder.Entity<Department>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Business", ParentDepartmentId = null },
            new Department { Id = 6, Name = "Executive", ParentDepartmentId = 1},
            new Department { Id = 7, Name = "Consulting", ParentDepartmentId = 1 },
            
            new Department { Id = 2, Name = "Human Resources", ParentDepartmentId = null },
            new Department { Id = 8, Name = "Onboarding", ParentDepartmentId = 2 },
            new Department { Id = 9, Name = "Offboarding", ParentDepartmentId = 2 },
            new Department { Id = 10, Name = "Recruiting", ParentDepartmentId = 2 },
            
            new Department { Id = 3, Name = "Information Technology", ParentDepartmentId = null },
            new Department { Id = 11, Name = "AI", ParentDepartmentId = 3 },
            new Department { Id = 12, Name = "Applications Development", ParentDepartmentId = 3 },
            new Department { Id = 13, Name = "Cybersecurity", ParentDepartmentId = 3 },
            new Department { Id = 14, Name = "SEO", ParentDepartmentId = 3 },
            new Department { Id = 15, Name = "Web Development", ParentDepartmentId = 3 },
            
            new Department { Id = 4, Name = "Marketing", ParentDepartmentId = null },
            new Department { Id = 16, Name = "Market Research", ParentDepartmentId = 4 },
            new Department { Id = 17, Name = "Social Media", ParentDepartmentId = 4 },
            new Department { Id = 18, Name = "Copywriting", ParentDepartmentId = 4 },
            new Department { Id = 19, Name = "Graphic Design", ParentDepartmentId = 4 },
            
            new Department { Id = 5, Name = "Operations", ParentDepartmentId = null },
            new Department { Id = 20, Name = "Finance", ParentDepartmentId = 5 },
            new Department { Id = 21, Name = "Legal", ParentDepartmentId = 5 }
        );
        
        modelBuilder.Entity<ProjectTeam>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<TeamMember>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<Candidate>()
            .HasKey(u => u.Id);
    }
}