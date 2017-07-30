using System;
using System.Text;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Expression
{
    public class In : ExpTree
    {
        string[] ins = null;
        public In(params string[] ins)
        { this.ins = ins; }

        public override string ToString()
        {
            return "In (" + string.Join(",", ins) + ") ";
        }
    }
}
