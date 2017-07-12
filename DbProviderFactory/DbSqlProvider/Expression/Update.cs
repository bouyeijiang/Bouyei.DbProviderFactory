using System;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
{
    public class Update : ExpTree
    {
        string tableName = string.Empty;
        public Update(string tableName)
        { this.tableName = tableName; }

        public override string ToString()
        {
            return "Update " + tableName ;
        }
    }

    public class Update<T>:ExpTree
    {
        string tableName = string.Empty;

        public Update()
        {
            tableName = typeof(T).Name;
        }

        public override string ToString()
        {
            return "Update " + tableName;
        }
    }
}
