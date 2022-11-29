using Authentication.Common;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data;

public class AuthenticationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Role> Roles { get; set; }

    public AuthenticationDbContext() : base() { }

    public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasMany(u => u.Roles)
                .WithMany(r => r.Users);

            user.Property(u => u.Username).IsRequired();
            user.Property(u => u.PasswordHash).IsRequired();
            user.Property(u => u.IsActive).HasDefaultValue(true);
            user.Property(u => u.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Role>(role =>
        {
            role.Property(r => r.Name).IsRequired();
        });
    }
}