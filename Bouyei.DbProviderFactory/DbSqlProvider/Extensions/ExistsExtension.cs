/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:30:53
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: eff63b12-34cf-47ce-b7ae-e5288fa6aefa
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Extensions
{
    using Expression;
   public static class ExistsExtension
    {
        #region exists

        public static Exists Exists(this Where where, string values)
        {
            Exists _exists = new Exists(values);
            _exists.SqlString = where.SqlString + _exists.ToString();

            return _exists;
        }

        #endregion
    }
}
