using System;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Expression
{
    public class From<T> : ExpFrom<T> where T : class
    {
        public From()
            : base()
        { }

        public override string ToString()
        {
            return string.Format("From {0} ", TableName);
        }
    }

    public class From : ExpFrom
    {
        public From(params string[] TableName)
            :base(TableName)
        { }

        public override string ToString()
        {
            return string.Format("From {0} ", TableNames);
        }
    }
}
