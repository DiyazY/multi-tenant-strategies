using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace SchemaBasedStrategy
{
    public class ServiceModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        => new ServiceModelCacheKey(context);
    }

    class ServiceModelCacheKey : ModelCacheKey
    {
        string _schema;

        public ServiceModelCacheKey(DbContext context)
            : base(context)
        {
            _schema = (context as SchemaBasedContext)?.Schema;
        }

        protected override bool Equals(ModelCacheKey other)
            => base.Equals(other)
                && (other as ServiceModelCacheKey)?._schema == _schema;

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode() * 397;
            if (_schema != null)
            {
                hashCode ^= _schema.GetHashCode();
            }
            return hashCode;
        }
    }
}
