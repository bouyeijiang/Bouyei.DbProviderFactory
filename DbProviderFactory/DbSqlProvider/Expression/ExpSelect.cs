using System;
using System.Text;
using System.Reflection;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
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
            if (Columns == null) return string.Empty;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < Columns.Length; ++i)
            {
                builder.AppendFormat("{0}{1}", Columns[i], (i < Columns.Length - 1 ? "," : " "));
            }
            return builder.ToString();
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
            if (Columns == null) return string.Empty;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < Columns.Length; ++i)
            {
                builder.AppendFormat("{0},{1}", Columns[i], (i < Columns.Length - 1 ? "," : " "));
            }
            return builder.ToString();
        }
    }
}
