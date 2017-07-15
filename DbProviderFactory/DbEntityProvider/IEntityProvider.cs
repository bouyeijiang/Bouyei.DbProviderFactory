using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
//using System.Text;

namespace Bouyei.ProviderFactory.DbEntityProvider
{
    public interface IEntityProvider
    {
        IQueryable<TEntity> Query<TEntity>() where TEntity : class;

        IQueryable<TEntity> NoTrackQuery<TEntity>() where TEntity : class;

        IQueryable<TEntity> NoTrackQuery<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        DbSet<TEntity> DbSet<TEntity>() where TEntity : class;

        int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity:class;

        void Refresh<TEntity>(TEntity entity) where TEntity : class;

        TEntity GetById<TEntity>(object id) where TEntity : class;

        TEntity Insert<TEntity>(TEntity entity) where TEntity : class;

        IEnumerable<TEntity> InsertRange<TEntity>(TEntity[] entities) where TEntity : class;

        void Update<TEntity>(TEntity entity) where TEntity : class;

        void Delete<TEntity>(TEntity entity) where TEntity : class;

        IEnumerable<TEntity> Delete<TEntity>(Func<TEntity, bool> predicate) where TEntity : class;

        int ExecuteCommand(string command, params object[] parameters);

        int ExecuteCommandTransaction(string command,System.Data.IsolationLevel IsolationLevel, params object[] parameters);

        List<T> ExecuteQuery<T>(string command, params object[] parameters);

        int SaveChanges();

        void Dispose();
    }
}
