/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:30:33
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: d7dab3ea-6b82-45fe-81f2-76c6b8879a92
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.ProviderFactory.DbSqlProvider.Extensions
{
    using Expression;
   public static class InExtension
    {
        #region  In
        public static In In(this Where where, params string[] values)
        {
            In _in = new In(values);
            _in.SqlString = where.SqlString + _in.ToString();

            return _in;
        }

        #endregion
    }
}
