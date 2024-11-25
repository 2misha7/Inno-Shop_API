using Microsoft.EntityFrameworkCore;
using UserManager.Entities;

namespace UserManager.Context;

public class UserManagerContext : DbContext
{
    public UserManagerContext()
    {
    }
    
    public UserManagerContext(DbContextOptions<UserManagerContext> options)
        : base(options)
    {
    } 
    public DbSet<AppUser> Users { get; set; }
}