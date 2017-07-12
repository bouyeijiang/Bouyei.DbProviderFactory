/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:31:58
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: 93f135ed-ae22-42a9-aac7-260731b7ce1f
---------------------------------------------------------------*/
using System;
using SysLE = System.Linq.Expressions;

namespace Bouyei.ProviderFactory.DbSqlProvider.Extensions
{
    using Expression;
   public static class OrderByExtension
    {
        #region order by 关键字拼接
        /// <summary>
        /// 默认Asc(升序)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="Names"></param>
        /// <returns></returns>
        public static OrderBy OrderBy(this From from, params string[] Names)
        {
            OrderBy orderby = new OrderBy(Names);
            orderby.SqlString = from.SqlString + orderby.ToString();

            return orderby;
        }

        public static OrderBy OrderBy(this From from, Ordering[] orderings, params string[] Names)
        {
            OrderBy orderby = new OrderBy(Names);
            orderby.SqlString = from.SqlString + orderby.ToString(orderings);

            return orderby;
        }

        public static OrderBy OrderBy<T, TSource>(this From from, SysLE.Expression<Func<T, TSource>> exp)
        {
            OrderBy orderby = new OrderBy(SysExpression_Analyize<T, TSource>(exp));
            orderby.SqlString = from.SqlString + orderby.ToString();

            return orderby;
        }

        private static string SysExpression_Analyize<T, TSource>(SysLE.Expression<Func<T, TSource>> exp)
        {
            if (exp.Body.NodeType == SysLE.ExpressionType.Parameter) return exp.Name;
            else if (exp.Body.NodeType == SysLE.ExpressionType.MemberAccess)
            {
                SysLE.MemberExpression mexp = (SysLE.MemberExpression)exp.Body;
                if (mexp == null) return string.Empty;
                else return mexp.Member.Name;
            }
            else return string.Empty;
        }

        #endregion
    }
}
