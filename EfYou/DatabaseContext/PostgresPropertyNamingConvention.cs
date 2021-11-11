using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EfYou.DatabaseContext
{
    public class PostgresPropertyNamingConvention : IStoreModelConvention<EdmProperty>
    {
        public void Apply(EdmProperty item, DbModel model)
        {
            item.Name = item.Name.ToLower();
        }
    }
}