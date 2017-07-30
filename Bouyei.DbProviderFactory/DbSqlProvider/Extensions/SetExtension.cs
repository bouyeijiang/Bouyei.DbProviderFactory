/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:32:17
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: ace252e0-9c16-4f24-8b01-a36aedf06b62
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Extensions
{
    using Expression;
   public static class SetExtension
    {
        public static Set Set(this Update update, string tableName, params KeyValue[] kvalue)
        {
            Set set = new Set(kvalue);
            set.SqlString = update.SqlString + set.ToString();

            return set;
        }

        public static Set<T> Set<T>(this Update<T> update, T Entity)
        {
            Set<T> set = new Expression.Set<T>(Entity);
            set.SqlString = update.SqlString + set.ToString();

            return set;
        }
    }
}
