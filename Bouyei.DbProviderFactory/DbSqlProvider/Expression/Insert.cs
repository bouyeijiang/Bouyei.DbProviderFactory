using System;
using System.Text;
using System.Reflection;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
{
  

    public class Insert<T>:ExpTree where T:class
    {
        T value = default(T);
        T[] Values = null;

        public Insert(T value)
        {
            this.value = value;
        }

        public Insert(T[] Values)
        {
            this.Values = Values;
        }

        public override string ToString()
        {
            PropertyInfo[] infos = typeof(T).GetProperties();
            StringBuilder builder = new StringBuilder(string.Format("Insert Into {0} ", typeof(T).Name));
            string columns = string.Empty;
            string values = string.Empty;
            object temp = null;

            if (Values ==null)
            {
                for (int i = 0; i < infos.Length; ++i)
                {
                    temp = infos[i].GetValue(value, null);
                    if (temp == null) continue;

                    columns += string.Format("{0}{1}", infos[i].Name, i < infos.Length - 1 ? "," : "");
                    if (temp is string) values += string.Format("'{0}'{1}", temp, i < infos.Length - 1 ? "," : "");
                    else values += string.Format("{0}{1}", temp, i < infos.Length - 1 ? "," : "");
                }
                builder.Append(string.Format("({0}) Values({1})", columns, values));
            }
            else
            {
                for (int i = 0; i < infos.Length; ++i)
                {
                    columns += string.Format("{0}{1}", infos[i].Name, i < infos.Length - 1 ? "," : "");
                }

                builder.Append(string.Format("({0}) Values", columns));

                for (int i = 0; i < Values.Length; ++i)
                {
                    values = string.Empty;
                    for (int j = 0; j < infos.Length; ++j)
                    {
                        temp = infos[j].GetValue(Values[i], null);
                        if (temp == null) continue;

                        if (temp is string) values += string.Format("'{0}'{1}", temp, j < infos.Length - 1 ? "," : "");
                        else values += string.Format("{0}{1}", temp, j < infos.Length - 1 ? "," : "");
                    }

                    builder.AppendFormat("({0}){1}", values, i < Values.Length - 1 ? "," : "");
                  
                }
            }
           
            return builder.ToString();
        }
    }

    public class Insert:ExpTree
    {
        string tableName = string.Empty;
        string[] Columns = null;
        string[,] Values = null;
        KeyValue[] keyValues = null;

        public Insert(string tableName,params KeyValue[] keyValues)
        {
            this.keyValues = keyValues;
            this.tableName = tableName;
        }

        public Insert(string tableName,string[] Columns,string[,] Values)
        {
            this.tableName = tableName;
            this.Columns = Columns;
            this.Values = Values;
        }

        public override string ToString()
        {
            StringBuilder builder= new StringBuilder(string.Format("Insert Into {0}", tableName));
            string columns = string.Empty;
            string values = string.Empty;

            if (keyValues != null)
            {
                for (int i = 0; i < keyValues.Length; ++i)
                {
                    columns += string.Format("{0}{1}", keyValues[i].Name, i < keyValues.Length - 1 ? "," : "");
                    values += string.Format("'{0}'{1}", keyValues[i].Value, i < keyValues.Length - 1 ? "," : "");
                }
                builder.AppendFormat("({0}) Values({1})", columns, values);
            }
            else
            {
                for(int i = 0; i < Columns.Length; ++i)
                {
                    columns += string.Format("{0}{1}", Columns[i], i < Columns.Length - 1 ? "," : "");
                }

                builder.AppendFormat("({0}) Values", columns);

                int rows = Values.GetLength(0);
                int cols = Values.GetLength(1);

                for (int i=0;i<rows;++i)
                {
                    values = string.Empty;
                    for(int j = 0; j <cols; ++j)
                    {
                        values += string.Format("'{0}'{1}", Values[i,j], j <cols - 1 ? "," : "");
                    }

                    builder.AppendFormat("({0}){1}", values, i < rows - 1 ? "," : "");
                }
            }
            return builder.ToString();
        }
    }
}
