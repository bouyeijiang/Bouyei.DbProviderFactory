using System;

namespace Bouyei.ProviderFactory
{
    using DbEntityProvider;

    public class DbEFLayer:IDisposable
    {
        private static readonly Lazy<DbEFLayer> dbEFLayer = new Lazy<DbEFLayer>();
        EntityContext eContext = null;
        IEntityProvider ieProvider = null;

        public static DbEFLayer Singleton
        {
            get { return dbEFLayer.Value; }
        }

        public static DbEFLayer CreateEFLayer(string DbConnection=null)
        {
            return new DbEFLayer(DbConnection);
        }

        public DbEFLayer()
        {
            eContext = new EntityContext();
          
            ieProvider = new EntityProvider(eContext);
        }

        public DbEFLayer(string DbConnection=null)
        {
            eContext = new EntityContext(DbConnection);
            ieProvider = new EntityProvider(eContext);
        }

        public IEntityProvider Provider
        {
            get { return ieProvider; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
         
        ~DbEFLayer()
        {
            Dispose(false);
        }

        protected void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (eContext != null)
                {
                    eContext.Dispose();
                }
                if (ieProvider != null)
                {
                    ieProvider.Dispose();
                }
            }
        }

        public int SaveChanges()
        {
           return eContext.SaveChanges();
        }
    }
}
