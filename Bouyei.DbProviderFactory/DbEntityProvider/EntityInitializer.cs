using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Bouyei.DbProviderFactory.DbEntityProvider
{
    /// <summary>
    /// 初始化实体数据库
    /// </summary>
    internal class CreateOrMigrateDatabase<TContext,TConfig>
         : CreateDatabaseIfNotExists<TContext>, IDatabaseInitializer<TContext>
         where TContext : DbContext
         where TConfig : DbMigrationsConfiguration<TContext>, new()
    {
        private readonly DbMigrationsConfiguration _configuration;

        public CreateOrMigrateDatabase(string dbConnectionString = null)
        {
            if (string.IsNullOrEmpty(dbConnectionString) == false)
                _configuration = new TConfig()
                {
                    TargetDatabase = new DbConnectionInfo(dbConnectionString)
                };
            else
                _configuration = new TConfig();
        }

        /// <summary>
        /// 如果不存在数据库或表则创建
        /// </summary>
        /// <param name="eContext"></param>
        void IDatabaseInitializer<TContext>.InitializeDatabase(TContext dbContext)
        {
            var exist = dbContext.Database.Exists();

            if (exist)
            {
                var migrator = new DbMigrator(_configuration);
                if (dbContext.Database.CompatibleWithModel(false)==false ||
                     migrator.GetPendingMigrations().Any())
                {
                    migrator.Update();
                }
            }
            else
            {
                Seed(dbContext);
                dbContext.SaveChanges();
            }
            base.InitializeDatabase(dbContext);
        }

        protected override void Seed(TContext context)
        {
        }
    }

    internal class DbConfig:DbMigrationsConfiguration<EntityContext>
    {
        public DbConfig()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(EntityContext context)
        {
        }
    }

    internal class DbInitialize:CreateOrMigrateDatabase<EntityContext,DbConfig>
    {
        public DbInitialize(string dbConnectionString=null)
            : base(dbConnectionString)
        {

        }

        protected override void Seed(EntityContext context)
        {
            base.Seed(context);
        }
    }
}
