using System;

namespace Bouyei.DbProviderFactory
{
    using DbEntityProvider;

    public class OrmProvider : EntityProvider,IOrmProvider
    {
        public static OrmProvider CreateOrmProvider(string DbConnection = null)
        {
            return new OrmProvider(DbConnection);
        }

        public OrmProvider(string DbConnection = null)
            : base(DbConnection)
        { }
    }
}
