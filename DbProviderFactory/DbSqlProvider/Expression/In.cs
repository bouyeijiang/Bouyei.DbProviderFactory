using System;
using System.Text;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
{
    public class In:ExpTree
    {
        string[] ins = null;
        public In(params string[] ins)
        { this.ins = ins; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("In (");
            for (int i = 0; i < ins.Length; ++i)
            {
                builder.AppendFormat("'{0}'{1}", ins[i], (i < ins.Length - 1 ? "," : ") "));
            }
            return builder.ToString();
        }
    }
}
