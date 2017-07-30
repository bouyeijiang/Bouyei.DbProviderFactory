/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:30:07
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: a6e640d9-576c-4fa3-9f75-ab936f65a9f8
---------------------------------------------------------------*/
using System;
using System.Text;
using SysLE = System.Linq.Expressions;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Extensions
{
    using Expression;
   public static class WhereExtension
    {
        #region where关键字拼接
        /// <summary>
        /// 与条件
        /// </summary>
        /// <param name="from"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Where Where(this From from, params KeyValue[] values)
        {
            Where where = new Where(values);
            where.SqlString = from.SqlString + where.ToString();

            return where;
        }

        /// <summary>
        /// 自定义条件
        /// </summary>
        /// <param name="from"></param>
        /// <param name="andOrs"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Where Where(this From from, AndOr[] andOrs, params KeyValue[] values)
        {
            Where where = new Where(values);
            where.SqlString = from.SqlString + where.ToString(andOrs);

            return where;
        }

        public static Where Where(this From from, string whereStr)
        {
            Where where = new Where(whereStr);
            return where;
        }

        public static Where<T> Where<T>(this From<T> from, T value) where T : class
        {
            Where<T> where = new Where<T>(value);
            where.SqlString = from.SqlString + where.ToString();

            return where;
        }

        public static Where<T> Where<T>(this From<T> from, SysLE.Expression<Func<T, bool>> expression) where T : class
        {
            Where<T> where = new Where<T>(default(T));
            where.SqlString = from.SqlString + where.ToString() + SysExpression_Analyize(expression);

            return where;
        }

        public static Where Where(this Set set, string wherestr)
        {
            Where where = new Expression.Where(wherestr);
            where.SqlString = set.SqlString + where.ToString();

            return where;
        }

        public static Where<T> Where<T>(this Set<T> set, T value)
        {
            Where<T> where = new Expression.Where<T>(value);
            where.SqlString = set.SqlString + where.ToString();

            return where;
        }

        private static string SysExpression_Analyize<T>(SysLE.Expression<Func<T, bool>> exp)
        {
            StringBuilder builder = new StringBuilder();
            SysExpressionWhere_Analyize(exp.Body, builder);
            return builder.ToString();
        }

        private static SysLE.Expression SysExpressionWhere_Analyize(SysLE.Expression exp, StringBuilder builder)
        {
            if (exp == null) return null;

            SysLE.BinaryExpression binEx = exp as SysLE.BinaryExpression;
            if (binEx != null) SysExpressionWhere_Analyize(binEx.Left, builder);

            switch (exp.NodeType)
            {
                case SysLE.ExpressionType.Parameter:
                    {
                        SysLE.ParameterExpression param = (SysLE.ParameterExpression)exp;
                        builder.Append("(" + param.Name);
                        return null;
                    }
                case SysLE.ExpressionType.MemberAccess:
                    {
                        SysLE.MemberExpression mexp = (SysLE.MemberExpression)exp;
                        builder.Append("(" + mexp.Member.Name);
                        return null;
                    }
                case SysLE.ExpressionType.Constant:
                    {
                        SysLE.ConstantExpression cex = (SysLE.ConstantExpression)exp;
                        if (cex.Value is string) builder.Append("'" + cex.Value.ToString() + "') ");
                        else builder.Append(cex.Value.ToString() + ")");
                        return null;
                    }
                default:
                    {
                        if (exp.NodeType == SysLE.ExpressionType.Equal) builder.Append("=");
                        else if (exp.NodeType == SysLE.ExpressionType.NotEqual) builder.Append("<>");
                        else if (exp.NodeType == SysLE.ExpressionType.LessThan) builder.Append("<");
                        else if (exp.NodeType == SysLE.ExpressionType.LessThanOrEqual) builder.Append("<=");
                        else if (exp.NodeType == SysLE.ExpressionType.GreaterThan) builder.Append(">");
                        else if (exp.NodeType == SysLE.ExpressionType.GreaterThanOrEqual) builder.Append(">=");
                        else if (exp.NodeType == SysLE.ExpressionType.AndAlso || exp.NodeType == SysLE.ExpressionType.And)
                        {
                            builder.Append("and");
                        }
                        else if (exp.NodeType == SysLE.ExpressionType.OrElse || exp.NodeType == SysLE.ExpressionType.Or)
                        {
                            builder.Append("or");
                        }
                    }
                    break;
            }

            if (binEx != null) SysExpressionWhere_Analyize(binEx.Right, builder);

            return binEx;
        }

        #endregion
    }
}
