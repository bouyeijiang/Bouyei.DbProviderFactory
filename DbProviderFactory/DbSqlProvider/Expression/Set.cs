using System;
using System.Text;
using System.Reflection;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
{
  

    public class Set:ExpTree
    {
        KeyValue[] keyvalues = null;

        public Set(params KeyValue[] keyvalues)
        { this.keyvalues = keyvalues; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Set ");
            for (int i = 0; i < keyvalues.Length; ++i)
            {
                builder.AppendFormat("{0}='{1}'{2}", keyvalues[i].Name, keyvalues[i].Value, (i < keyvalues.Length - 1 ? "," : " "));
            }
            return builder.ToString();
        }
    }

    public class Set<T>:ExpTree
    {
        T Entity = default(T);
        public Set(T Entity)
        {
            this.Entity = Entity;
        }

        public override string ToString()
        {
            PropertyInfo[] infos = typeof(T).GetProperties();
            object temp = null;
            StringBuilder builder = new StringBuilder(" Set ");
            for (int i = 0; i < infos.Length; ++i)
            {
                temp = infos[i].GetValue(Entity, null);
                if (temp == null) continue;

                if(temp is string) builder.AppendFormat("{0}='{1}'{2}", infos[i].Name, temp, (i < infos.Length - 1 ? "," : " "));
                else builder.AppendFormat("{0}={1}{2}", infos[i].Name, temp, (i < infos.Length - 1 ? "," : " "));
            }
            return builder.ToString();
        }
    }
}
