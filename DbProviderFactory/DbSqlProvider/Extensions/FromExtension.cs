/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:29:08
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: 680712c3-085b-40bd-a505-6ec5cc4e9e1b
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.ProviderFactory.DbSqlProvider.Extensions
{
    using Expression;

   public static class FromExtension
    {
        #region From关键字拼接
        public static From From(this Select select, params string[] tableName)
        {
            From from = new From(tableName);
            from.SqlString = select.SqlString + from.ToString();

            return from;
        }

        public static From<T> From<T>(this Select<T> select) where T : class
        {
            From<T> from = new From<T>();
            from.SqlString = select.SqlString + from.ToString();

            return from;
        }

        public static From From(this Delete delete, string tableName)
        {
            From from = new Expression.From(tableName);
            from.SqlString = delete.ToString() + from.ToString();

            return from;
        }

        public static From<T> From<T>(this Delete<T> delete) where T : class
        {
            From<T> from = new Expression.From<T>();
            from.SqlString = delete.ToString() + from.ToString();

            return from;
        }
        #endregion
    }
}
