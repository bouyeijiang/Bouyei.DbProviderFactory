using System;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Expression
{
    public class Exists:ExpTree
    {
        string value = string.Empty;
        public Exists(string value)
        { this.value = value; }

        public override string ToString()
        {
            return "Exists (" + this.value + ") ";
        }
    }
}
