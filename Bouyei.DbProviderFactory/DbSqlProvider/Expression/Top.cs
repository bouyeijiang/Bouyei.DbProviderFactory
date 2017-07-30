using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Expression
{
   public class Top:ExpTree
    {
       int count = 0;
       public Top(int count=30)
       { this.count = count; }

       public override string ToString()
       {
           return "Top " + count.ToString() + " ";
       }
    }
}
