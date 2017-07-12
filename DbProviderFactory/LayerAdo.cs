using System;
using System.Data;

namespace Bouyei.ProviderFactory
{
    using DbAdoProvider;

    public class LayerAdo : DbProvider,IDbProvider
    {
        public LayerAdo(string ConnectionString,
            ProviderType ProviderType = ProviderType.SqlServer,
            bool IsSingleton = false)
            : base(ConnectionString, ProviderType, IsSingleton)
        {
        }

        public static LayerAdo CreateLayerAdo(string ConnectionString,
            ProviderType providerType=ProviderType.SqlServer, 
            bool IsSingleton = false)
        {
            return new LayerAdo(ConnectionString, providerType, IsSingleton);
        }
    }
}
