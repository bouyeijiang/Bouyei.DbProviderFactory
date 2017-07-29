using System;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
{
    public class Delete:ExpTree
    {
        public Delete()
        { }

        public override string ToString()
        {
            return "Delete ";
        }
    }

    public class Delete<T>:ExpTree
    {
        public Delete()
        { }

        public override string ToString()
        {
            return "Delete ";
        }
    }
}
