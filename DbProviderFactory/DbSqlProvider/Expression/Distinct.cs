using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
{
    public class Distinct:ExpTree
    {
        string column = string.Empty;

        public Distinct(string column)
        { this.column = column; }

        public override string ToString()
        {
            return "Distinct " + column;
        }
    }
}
