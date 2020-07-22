using System;
using System.Collections.Generic;

namespace Models.ForTableBased
{
    public class BlogTable
    {
        private string _tenantId;

        public int Id { get; set; }
        public string Url { get; set; }
        
        public virtual List<PostTable> Posts { get; set; }

        public void SetTenant(string tenant)
        {
            if (string.IsNullOrWhiteSpace(tenant))
                throw new ArgumentException("Tenant id");
            _tenantId = tenant;
        }
    }
}
