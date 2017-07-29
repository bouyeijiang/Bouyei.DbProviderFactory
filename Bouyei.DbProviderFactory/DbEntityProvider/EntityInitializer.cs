using System;
using System.Data;
using System.Data.Entity;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

namespace Bouyei.ProviderFactory.DbEntityProvider
{
    /// <summary>
    /// 初始化实体数据库
    /// </summary>
    public class EntityInitializer : IDatabaseInitializer<EntityContext>
    {
        /// <summary>
        /// 如果不存在数据库或表则创建
        /// </summary>
        /// <param name="eContext"></param>
        public void InitializeDatabase(EntityContext eContext)
        {
            eContext.Database.CreateIfNotExists();
        }
    }
}
