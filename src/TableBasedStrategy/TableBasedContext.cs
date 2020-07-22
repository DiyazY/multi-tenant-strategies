using Microsoft.EntityFrameworkCore;
using Models.ForTableBased;

namespace TableBasedStrategy
{
    public sealed class TableBasedContext : DbContext
    {
        private readonly Models.Tenant _tenant;

        public TableBasedContext(DbContextOptions<TableBasedContext> options, Models.Tenant tenant)
            : base(options)
        {
            _tenant = tenant;
        }
        public TableBasedContext(DbContextOptions<TableBasedContext> options)
        : base(options)
        { }

        public DbSet<BlogTable> Blogs { get; set; }
        public DbSet<PostTable> Posts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<BlogTable>()
                .Property<string>("_tenantId")
                .HasColumnName("TenantId");

            // Configure entity filters
            modelBuilder
                .Entity<BlogTable>()
                .HasQueryFilter(b => EF.Property<string>(b, "_tenantId") == _tenant.Key);
            base.OnModelCreating(modelBuilder);
        }
    }
}
