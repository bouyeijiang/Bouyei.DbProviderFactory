using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Expression
{
    public class NotExists:ExpTree
    {
        string value = string.Empty;
        public NotExists(string value)
        { this.value = value; }

        public override string ToString()
        {
            return "Not Exists (" + this.value + ") ";
        }
    }
}
