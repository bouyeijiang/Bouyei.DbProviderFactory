using System;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
{
  

    public class Where<T> : ExpWhere
    {
        T value;
        public Where(T value)
            :base()
        {
            this.value = value;
        }

        public override string ToString()
        {
            if (this.value==null) return "Where ";

            return "Where " + base.ToExpression(value);
        }        
    }

    public class Where : ExpWhere
    {
        KeyValue[] values = null;
        string whereStr = string.Empty;
        public Where(KeyValue[] values)
            : base()
        {
            this.values = values;
        }

        public Where(string whereStr)
        {
            this.whereStr = whereStr;
        }

        /// <summary>
        /// 默认“与”条件
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(whereStr))
                return "Where " + base.ToExpression(values);
            else return whereStr;
        }

        /// <summary>
        /// 定义“与或”条件
        /// </summary>
        /// <param name="andOrs"></param>
        /// <returns></returns>
        public string ToString(AndOr[] andOrs)
        {
            return "Where " + base.ToExpression(values,andOrs);
        }
    }
}
