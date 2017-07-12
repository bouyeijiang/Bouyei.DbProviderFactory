using System;
using System.Text;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
{
    public class GroupBy:ExpTree
    {
        string[] groupNames = null;

        public GroupBy(params string[] groupNames)
        { this.groupNames = groupNames; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Group By ");
            for (int i = 0; i < groupNames.Length; ++i)
            {
                builder.AppendFormat("{0}{1}", groupNames[i], (i < groupNames.Length - 1 ? "," : " "));
            }
           return builder.ToString();
        }
    }
}
