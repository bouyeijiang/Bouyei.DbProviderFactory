/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:31:20
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: df4672a5-fca8-47c0-b626-6dc48f42e9d0
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.ProviderFactory.DbSqlProvider.Extensions
{
    using Expression;
   public static class NotExtension
    {
        #region notexists

        public static NotExists NotExists(this Where where, string values)
        {
            NotExists _notEists = new NotExists(values);
            _notEists.SqlString = where.SqlString + _notEists.ToString();

            return _notEists;
        }

        #endregion
    }
}
