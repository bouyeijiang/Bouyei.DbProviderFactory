using System;
using System.Data;

namespace Bouyei.ProviderFactory
{
    using DbAdoProvider;

    public class DbLayer : DbProvider
    {
        public DbLayer(string ConnectionString,
            ProviderType ProviderType = ProviderType.SqlServer,
            bool IsSingleton = true)
            : base(ConnectionString, ProviderType, IsSingleton)
        {
        }

        public static DbLayer CreateDbLayer(string ConnectionString,
            ProviderType providerType=ProviderType.SqlServer, 
            bool IsSingleton = false)
        {
            return new DbLayer(ConnectionString, providerType, IsSingleton);
        }
    }

    [Serializable]
    public class DbLayerR:MarshalByRefObject,IDbLayer
    {
        private DbProvider dbProvider = null;
        private string ConnectionString = string.Empty;

        public DbLayerR(string ConnectionString,ProviderType providerType,bool isSingleton=true)
        {
            this.ConnectionString = ConnectionString;
            dbProvider = new DbProvider(ConnectionString, providerType, isSingleton);
        }

        public ResultInfo<bool, string> Connect(string ConnectionString)
        {

            this.ConnectionString = ConnectionString;
            return dbProvider.Connect(ConnectionString);

        }

        public ResultInfo<int,string> Execute(string command)
        {
           return dbProvider.ExecuteCmd(new DbExecuteParameter() {CommandText= command});
        }

        public ResultInfo<DataSet,string> Query(string command)
        {
          return dbProvider.QueryToSet(new DbExecuteParameter() { CommandText = command });
        }

        public ResultInfo<int,string> ExecuteTransaction(string command)
        {
           return dbProvider.ExecuteTransaction(new DbExecuteParameter() {CommandText= command});

        }
    }
}
