using System;
using System.Data;

namespace Bouyei.DbProviderFactory
{
    using DbAdoProvider;

    public class AdoProvider : DbProvider,IAdoProvider
    {
        public AdoProvider(string ConnectionString,
            ProviderType ProviderType = ProviderType.SqlServer,
            bool IsSingleton = false)
            : base(ConnectionString, ProviderType, IsSingleton)
        {
        }

        public static AdoProvider CreateProvider(string ConnectionString,
            ProviderType providerType=ProviderType.SqlServer, 
            bool IsSingleton = false)
        {
            return new AdoProvider(ConnectionString, providerType, IsSingleton);
        }
    }
}
