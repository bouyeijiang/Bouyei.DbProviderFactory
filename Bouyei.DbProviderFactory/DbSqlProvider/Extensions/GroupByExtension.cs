/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:31:41
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: 6bcc0e56-42f7-45e2-911c-9a7ab95949ad
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Extensions
{
    using Expression;
   public static class GroupByExtension
    {
        #region group by 关键字拼接
        public static GroupBy GroupBy(this From from, params string[] groupNames)
        {
            GroupBy groupby = new GroupBy(groupNames);
            groupby.SqlString = from.SqlString + groupby.ToString();

            return groupby;
        }
        #endregion
    }
}
