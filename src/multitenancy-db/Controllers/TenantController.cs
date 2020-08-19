using System.Text.RegularExpressions;
using DbBasedStrategy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using SchemaBasedStrategy;

namespace multitenancy_db.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TenantController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Register new tenant (db strategy)
        /// </summary>
        /// <param name="tenant">tenant name</param>
        /// <returns></returns>
        [HttpGet("register-db")]
        public IActionResult RegisterDb([FromQuery] string tenant)
        // If we talk about db or schema strategies this method's job will be better to delegate to a separate worker or service.
        // And notify your tenant later that he can start work. App has to create db or schema and it can be time consumable.
        {
            
            if (string.IsNullOrWhiteSpace(tenant)) return BadRequest("Tenant name is required!");
            var reg = new Regex("[0-9a-zA-Z$_]+");
            if (!reg.IsMatch(tenant)) return BadRequest("Tenant name must follow this pttern [0-9a-zA-Z$_]+");

            var newTenant = new Models.Tenant
            {
                ConStr = $"{_configuration.GetConnectionString("db")}_{tenant}",
                Key = tenant,
                Name = tenant
            };

            var connectionString = _configuration.GetConnectionString("db");
            var optionsBuilder = new DbContextOptionsBuilder<DbBasedContext>()
                .UseLazyLoadingProxies()
                .UseNpgsql(newTenant.ConStr);
            using (var db = new DbBasedContext(optionsBuilder.Options))
            {
                db.Database.Migrate();
            }
            Services.TenantResolver.Tenants.Add(newTenant);// add new tenant to collection of tenants
            return Ok("Tenant was created");
        }

        /// <summary>
        /// Register new tenant (table strategy)
        /// </summary>
        /// <param name="tenant">tenant name</param>
        /// <returns></returns>
        [HttpGet("register-table")]
        public IActionResult RegisterTable([FromQuery] string tenant)
        {

            if (string.IsNullOrWhiteSpace(tenant)) return BadRequest("Tenant name is required!");
            var reg = new Regex("[0-9a-zA-Z$_]+");
            if (!reg.IsMatch(tenant)) return BadRequest("Tenant name must follow this pttern [0-9a-zA-Z$_]+");

            var newTenant = new Models.Tenant
            {
                Key = tenant,
                Name = tenant
            };

            Services.TenantResolver.Tenants.Add(newTenant);// add new tenant to collection of tenants

            return Ok("Tenant was created");
        }

        /// <summary>
        /// Register new tenant (schema strategy)
        /// </summary>
        /// <param name="tenant">tenant name</param>
        /// <returns></returns>
        [HttpGet("register-schema")]
        public IActionResult RegisterSchema([FromQuery] string tenant)
        // If we talk about db or schema strategies this method's job will be better to delegate to a separate worker or service.
        // And notify your tenant later that he can start work. App has to create db or schema and it can be time consumable.
        {
            if (string.IsNullOrWhiteSpace(tenant)) return BadRequest("Tenant name is required!");
            var reg = new Regex("[0-9a-zA-Z$_]+");
            if (!reg.IsMatch(tenant)) return BadRequest("Tenant name must follow this pttern [0-9a-zA-Z$_]+");

            var newTenant = new Models.Tenant
            {
                Key = tenant,
                Name = tenant
            };

            var connectionString = _configuration.GetConnectionString("schema");
            var optionsBuilder = new DbContextOptionsBuilder<SchemaBasedContext>()
                        .UseNpgsql(
                            connectionString,
                            b => b.MigrationsAssembly("SchemaBasedStrategy")
                        )
                        .ReplaceService<IModelCacheKeyFactory, ServiceModelCacheKeyFactory>();

            new SchemaBasedContext(optionsBuilder.Options, newTenant.Key, true);

            Services.TenantResolver.Tenants.Add(newTenant);// add new tenant to collection of tenants
            return Ok("Tenant was created");
        }
    }
}