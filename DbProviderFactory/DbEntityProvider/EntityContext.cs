using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Configuration;
using System.Data.Common;

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Bouyei.ProviderFactory.DbEntityProvider
{
    public class EntityContext : DbContext, IDisposable
    {
        // private static List<dynamic> EntitiesMapping = new List<dynamic>();

        public EntityContext(string DbConnection = null)
            : base(string.Format("Name={0}", string.IsNullOrEmpty(DbConnection) ? "DbConnection" : DbConnection))
        {
            this.Database.Initialize(true);
        }

        public void Reload<TEntity>(TEntity entity) where TEntity : class
        {
            this.Entry<TEntity>(entity).Reload();
        }

        #region public
        public DbSet<TEntity> DSet<TEntity>() where TEntity : class
        {
            return this.Set<TEntity>();
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return DSet<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return DSet<TEntity>().Where(predicate);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            if (EnsureChange<TEntity>(entity) > 0)
            {
                DSet<TEntity>().Attach(entity);
                this.Entry<TEntity>(entity).State = EntityState.Modified;
            }
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            this.Set<TEntity>().Attach(entity);
            DSet<TEntity>().Remove(entity);
            this.Entry<TEntity>(entity).State = EntityState.Deleted;
        }

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class
        {
            TEntity rentity = this.Set<TEntity>().Add(entity);
            this.Entry<TEntity>(entity).State = EntityState.Added;
            return rentity;
        }

        public IEnumerable<TEntity> InsertRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            return DSet<TEntity>().AddRange(entities);
        }

        public int ExecuteCommand(string command, params object[] parameters)
        {
            return this.Database.ExecuteSqlCommand(command, parameters);
        }

        public int ExecuteCommandTransaction(string command,
            System.Data.IsolationLevel IsolationLevel, params object[] parameters)
        {
            if (this.Database.Connection.State != System.Data.ConnectionState.Open)
                this.Database.Connection.Open();

            using (DbTransaction dbTrans = this.Database.Connection.BeginTransaction(IsolationLevel))
            {
                this.Database.UseTransaction(dbTrans);
                try
                {
                    int rt = this.Database.ExecuteSqlCommand(command, parameters);
                    if (rt > 0) dbTrans.Commit();
                    else dbTrans.Rollback();

                    return rt;
                }
                catch (Exception ex)
                {
                    dbTrans.Rollback();
                    throw ex;
                }
            }
        }

        public List<T> ExecuteQuery<T>(string command, params object[] parameters)
        {
            return this.Database.SqlQuery<T>(command, parameters).ToList();
        }
        #endregion

        #region private
        private int EnsureChange<TEntity>(TEntity entity) where TEntity : class
        {
            var dbEntityEntry = Entry(entity);
            int changedCnt = 0;

            foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
            {
                var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                if (original != null)
                {
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (!original.Equals(current))
                    {
                        changedCnt += 1;
                        dbEntityEntry.Property(property).IsModified = true;
                    }
                }
            }
            return changedCnt;
        }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //自定义映射程序集所在位置
            string mappingDLL = ConfigurationManager.AppSettings.Get("mappingDLL");
            if (string.IsNullOrEmpty(mappingDLL))
                throw new Exception("找不到配置mapping.dll映射文件,如<add key=\"AssemblyPath\" value=\"DbMapping.dll\"/>");

            string path = AppDomain.CurrentDomain.BaseDirectory + mappingDLL;

            if (string.IsNullOrEmpty(path)
                || System.IO.File.Exists(path) == false)
                throw new Exception("找不到mappingDLL配置路径:" + path);

            Assembly assem = Assembly.LoadFile(path);

            modelBuilder.Configurations.AddFromAssembly(assem);
            base.OnModelCreating(modelBuilder);
        }
    }
}
