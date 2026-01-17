using Microsoft.EntityFrameworkCore;
using SoulArenasAPI.Database.Entities;

namespace SoulArenasAPI.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public virtual DbSet<UserEntity> Users { get; set; } = null!;
}
