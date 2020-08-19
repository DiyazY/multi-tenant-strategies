using DbBasedStrategy;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TableBasedStrategy;
using SchemaBasedStrategy;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace multitenancy_db.Services
{
    public static class DbStrategysResolver
    {

        public static IServiceCollection AddDbStrategy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMultitenancy<Tenant, TenantResolver>();
            var connectionString = configuration.GetConnectionString("db");
            services
                .AddDbContext<DbBasedContext>((provider, options) =>
                {
                    var tenant = provider.GetService<Tenant>();
                    options
                        .UseLazyLoadingProxies()
                        .UseNpgsql(tenant?.ConStr ?? $"{connectionString}_tenant_0");
                });

            return services;
        }

        public static IServiceCollection AddTableStrategy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMultitenancy<Tenant, TenantResolver>();
            var connectionString = configuration.GetConnectionString("table");
            services
                .AddDbContext<TableBasedContext>((provider, options) =>
                {
                    options
                        .UseLazyLoadingProxies()
                        .UseNpgsql(
                            connectionString,
                            x => x.MigrationsAssembly("TableBasedStrategy")
                         );
                });
            return services;
        }

        public static IServiceCollection AddSchemaStrategy(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConnectionString = configuration.GetConnectionString("schema");
            services.AddMultitenancy<Tenant, TenantResolver>();
            services
                .AddDbContext<SchemaBasedContext>(options =>
                    options
                        .UseLazyLoadingProxies()
                        .UseNpgsql(
                            sqlConnectionString,
                            b => b.MigrationsAssembly("SchemaBasedStrategy")
                        )
                        .ReplaceService<IModelCacheKeyFactory, ServiceModelCacheKeyFactory>()
                    );
            return services;
        }
    }
}
