using Microsoft.EntityFrameworkCore;
using Models;

namespace DbBasedStrategy
{
    public sealed class DbBasedContext : DbContext
    {
        public DbBasedContext(DbContextOptions<DbBasedContext> options)
        : base(options)
        { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
