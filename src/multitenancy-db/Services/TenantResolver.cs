using Microsoft.AspNetCore.Http;
using Models;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace multitenancy_db.Services
{
    public class TenantResolver : ITenantResolver<Tenant>
    {
        public static readonly List<Tenant> Tenants = new List<Tenant>();

        public Task<TenantContext<Tenant>> ResolveAsync(HttpContext context)
        {
            string key = context.Request.Query["tenant"];
            var tenant = Tenants.FirstOrDefault(t => t.Key == key);
            if (tenant == null)
            {
                tenant = new Tenant
                {
                    Name = "noname"
                };
            }
            return Task.FromResult(new TenantContext<Tenant>(tenant));
        }
    }
}
