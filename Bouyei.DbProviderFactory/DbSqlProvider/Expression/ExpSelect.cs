using System;
using System.Text;
using System.Reflection;

namespace Bouyei.DbProviderFactory.DbSqlProvider.Expression
{
    public class ExpSelect<T> : ExpTree
    {
        public string[] Columns { get; set; }

        public ExpSelect()
        {
            PropertyInfo[] infos = typeof(T).GetProperties();
            Columns = new string[infos.Length];
            for (int i = 0; i < infos.Length; ++i)
            {
                Columns[i] = infos[i].Name;
            }
        }

        public override string ToString()
        {
            if (Columns == null
                || Columns.Length==0) return string.Empty;

            return string.Join(",", Columns)+" ";
        }
    }

    public class ExpSelect : ExpTree
    {
        public string[] Columns { get; set; }

        public ExpSelect()
        { }

        public ExpSelect(string[] Columns)
        {
            this.Columns = Columns;
        }

        public override string ToString()
        {
            if (Columns == null
                || Columns.Length == 0) return string.Empty;

            return string.Join(",", Columns)+" ";
        }
    }
}
