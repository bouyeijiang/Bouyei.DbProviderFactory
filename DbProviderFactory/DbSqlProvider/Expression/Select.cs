using System;
using System.Text;
using System.Reflection;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
{
    public class Select<T>:ExpSelect<T> where T : class
    {
        public Select()
            : base()
        {}

        public override string ToString()
        {
            return "Select " + base.ToString();
        }
    }

    public class Select : ExpSelect
    {
        public Select(params string[] Columns)
            : base(Columns)
        {
        }

        public override string ToString()
        {
            return "Select " + base.ToString();
        }
    }
}
