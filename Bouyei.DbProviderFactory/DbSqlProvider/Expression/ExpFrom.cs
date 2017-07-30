using System;
using System.Text;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Expression
{
    public class ExpFrom<T>:ExpTree
    {
        public string TableName { get; set; }
        public ExpFrom()
        {
            TableName = typeof(T).Name;
        }
    }

    public class ExpFrom:ExpTree
    {
        public string[] TableNames { get; set; } 
        public ExpFrom(params string[] TableNames)
        {
            this.TableNames = TableNames;
        }
    }
}
