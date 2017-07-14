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
            return "Group By " + string.Join(",", groupNames) + " ";
        }
    }
}
