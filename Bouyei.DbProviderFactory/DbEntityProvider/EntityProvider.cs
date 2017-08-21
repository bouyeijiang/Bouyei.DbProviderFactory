using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

//using System.Collections.Generic;
//using System.Text;

namespace Bouyei.DbProviderFactory.DbEntityProvider
{
    public class EntityProvider:IDisposable,IEntityProvider
    {
        private EntityContext eContext=null;

        public static EntityProvider CreateProvider(string DbConnection = "")
        {
            return new EntityProvider(DbConnection);
        }

        public EntityProvider(string DbConnection = "")
        {
            eContext = new EntityContext(DbConnection);
        }

        public void DatabaseCreateOrMigrate()
        {
            eContext.CreateOrMigrateDb();
        }

        public DbSet<TEntity> DbSet<TEntity>() where TEntity : class
        {
            return this.eContext.DSet<TEntity>();
        }

		public void Refresh<TEntity>(TEntity entity) where TEntity : class
		{
            this.eContext.Reload(entity);
		}

        public int Count<TEntity>(Expression<Func<TEntity, bool>> predicate)where TEntity:class
        {
           return this.eContext.Count(predicate);
        }

        public bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity:class
        {
            return this.eContext.Any(predicate);
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return this.eContext.Query<TEntity>();
        }
        public IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity,bool>> predicate) where TEntity : class
        {
            return this.eContext.Query(predicate);
        }

        public IQueryable<TEntity> NoTrackQuery<TEntity>() where TEntity:class
        {
            return this.DbSet<TEntity>().AsNoTracking<TEntity>();
        }

        public IQueryable<TEntity> NoTrackQuery<TEntity>(Expression<Func<TEntity,bool>>predicate) where TEntity : class
        {
            return this.NoTrackQuery<TEntity>().Where(predicate);
        }

        public TEntity GetById<TEntity>(object id) where TEntity : class
        {
            return  this.DbSet<TEntity>().Find(id);
        }

        public TEntity Insert<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
			return this.eContext.Insert<TEntity>(entity, isSaveChange);
		}

        public IEnumerable<TEntity> InsertRange<TEntity>(TEntity[] entities, bool isSaveChange = false) where TEntity:class
        {
           return this.eContext.InsertRange<TEntity>(entities, isSaveChange);
        }

        public long BulkCopyWrite<TEntity>(IList<TEntity> buffer,int batchSize=10240) where TEntity : class
        {
            return this.eContext.BulkCopyWrite<TEntity>(buffer,batchSize);
        }

        public void Update<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            this.eContext.Update(entity, isSaveChange);
        }

        public void Delete<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            this.eContext.Delete(entity, isSaveChange);
        }

        public IEnumerable<TEntity> Delete<TEntity>(Func<TEntity, bool> predicate, bool isSaveChange = false) where TEntity : class
        {
            var items = this.eContext.DSet<TEntity>().Where(predicate);
            foreach (var item in items)
            {
                this.eContext.Delete(item,isSaveChange);
            }

            return items;
        }

        public int ExecuteCommand(string command, params object[] parameters)
        {
           return this.eContext.ExecuteCommand(command, parameters);
        }

        public int ExecuteCommandTransaction(string command,System.Data.IsolationLevel IsolationLevel, params object[] parameters)
        {
            return this.eContext.ExecuteCommandTransaction(command, IsolationLevel,parameters);
        }

        public  List<T> ExecuteQuery<T>(string command, params object[] parameters)
        {
            return this.eContext.ExecuteQuery<T>(command, parameters);
        }

        public int SaveChanges()
        {
            return eContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EntityProvider()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.eContext != null)
                {
                    this.eContext.Dispose();
                    this.eContext = null;
                }
            }
        }
    }
}
