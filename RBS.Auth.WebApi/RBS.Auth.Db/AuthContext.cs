using Microsoft.EntityFrameworkCore;
using RBS.Auth.Db.Domain;

namespace RBS.Auth.Db;

public class AuthContext : DbContext
{
    public AuthContext(DbContextOptions<AuthContext> options)
        : base(options)
    {
    }

    public DbSet<UserCredential> UserCredentials { get; set; }

    public DbSet<UserDetails> Details { get; set; }

    public DbSet<UserClaim> Claims { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthContext).Assembly);
    }
}