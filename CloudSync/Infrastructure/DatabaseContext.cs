using Microsoft.EntityFrameworkCore;
using CloudSync.Modules.CandidateManagement.Models;
using CloudSync.Modules.EmployeeManagement.Models;
using CloudSync.Modules.UserManagement.Models;

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
    public DbSet<EmployeePosition> EmployeePositions { get; set; }
    public DbSet<EmployeeDocuments> EmployeeDocuments { get; set; }
    public DbSet<EmployeeLegal> EmployeeLegals { get; set; }
    public DbSet<EmployeeEducation> EmployeeEducations { get; set; }
    public DbSet<EmployeeTraining> EmployeeTrainings { get; set; }
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
        
        // Convert Enums to Strings for EmployeePosition
        modelBuilder.Entity<EmployeePosition>()
            .Property(e => e.EmploymentStatus)
            .HasConversion<string>();

        modelBuilder.Entity<EmployeePosition>()
            .Property(e => e.EmploymentType)
            .HasConversion<string>();

        // Convert Enums to Strings for EmployeeBasic (Owned Type)
        modelBuilder.Entity<Employee>().OwnsOne(e => e.BasicInfo, basicInfo =>
        {
            basicInfo.Property(e => e.Gender)
                .HasConversion<string>();
            
            basicInfo.Property(e => e.MaritalStatus)
                .HasConversion<string>();
        });

        // Convert Enums to Strings for EmployeeEducation
        modelBuilder.Entity<EmployeeEducation>()
            .Property(e => e.EducationLevel)
            .HasConversion<string>();
        
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
        
        modelBuilder.Entity<Employee>().OwnsOne(e => e.ContactInfo, contactInfo =>
        {
            contactInfo.OwnsOne(ci => ci.PermanentAddress);
            contactInfo.OwnsOne(ci => ci.MailingAddress);
        });
        
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.PositionDetails)
            .WithOne(d => d.Employee)
            .HasForeignKey<EmployeePosition>(e => e.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Documents)
            .WithOne(d => d.Employee)
            .HasForeignKey<EmployeeDocuments>(e => e.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Legal)
            .WithOne(d => d.Employee)
            .HasForeignKey<EmployeeLegal>(e => e.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Education)
            .WithOne(d => d.Employee)
            .HasForeignKey<EmployeeEducation>(e => e.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Training)
            .WithOne(d => d.Employee)
            .HasForeignKey<EmployeeTraining>(e => e.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmployeeDocuments>().HasKey(u => u.Id);
        modelBuilder.Entity<EmployeeEducation>().HasKey(u => u.Id);
        modelBuilder.Entity<EmployeeLegal>().HasKey(u => u.Id);
        modelBuilder.Entity<EmployeePosition>().HasKey(u => u.Id);
        modelBuilder.Entity<EmployeeTraining>().HasKey(u => u.Id);
        
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