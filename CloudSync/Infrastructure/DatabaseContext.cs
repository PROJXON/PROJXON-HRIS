using Microsoft.EntityFrameworkCore;
using CloudSync.Modules.UserManagement.Models;

namespace CloudSync.Infrastructure;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
}