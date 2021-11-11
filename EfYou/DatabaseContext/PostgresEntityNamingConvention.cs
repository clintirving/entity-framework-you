using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using EfYou.EntityServices;

namespace EfYou.DatabaseContext
{
    public class PostgresEntityNamingConvention : IStoreModelConvention<EntitySet>
    {
        public void Apply(EntitySet item, DbModel model)
        {
            item.Table = item.Table.ToLower();
        }
    }
}