using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Models;

namespace SchemaBasedStrategy
{
    public sealed class SchemaBasedContext : DbContext
    {
        public string Schema { get; private set; }
        private bool _createState = false;

        public SchemaBasedContext(DbContextOptions<SchemaBasedContext> options)
        : base(options)
        { }

        public SchemaBasedContext(DbContextOptions<SchemaBasedContext> options, string schema, bool create) 
            : base(options)
        {
            _createState = create;
            Schema = schema;
            RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)Database.GetService<IDatabaseCreator>();
            databaseCreator.CreateTables();
        }

        public SchemaBasedContext(DbContextOptions<SchemaBasedContext> options, Tenant tenant)
        : base(options)
        {
            Schema = tenant?.Key ?? "public";
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder
                .Entity<Blog>()
                .ToTable("Blog", Schema);
            modelBuilder
                .Entity<Post>()
                .ToTable("Post", Schema);

            if (!_createState)
            {
                //Here should be shared entities.
                //Note: the entity's schema is common for all tenants

                //modelBuilder
                //    .Entity<ApplicationUser>(entity =>
                //    {
                //        entity.ToTable("user", "auth");
                //        entity
                //            .Property(e => e.Id)
                //            .HasColumnName("id");
                //    });
            }
            else
            {
                // When the app is creating a new tenant schema shared entities should be ignored.

                //modelBuilder.Ignore<ApplicationUser>();
            }
        }
    }
}
