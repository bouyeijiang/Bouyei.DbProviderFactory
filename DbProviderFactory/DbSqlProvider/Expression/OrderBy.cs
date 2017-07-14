using System;
using System.Text;

namespace Bouyei.ProviderFactory.DbSqlProvider.Expression
{
  

    public class OrderBy:ExpTree
    {
        string[] Names =null;

        public OrderBy(params string[] Names)
        { this.Names = Names; }

        /// <summary>
        /// 默认Asc(升序)
        /// </summary>
        /// <param name="ordering"></param>
        /// <returns></returns>
        public override string ToString()
        {
            return "Group By " + string.Join(",", Names) + " Asc ";
        }

        /// <summary>
        /// 参数必需匹配
        /// </summary>
        /// <param name="Names"></param>
        /// <param name="orderings"></param>
        /// <returns></returns>
        public string ToString(Ordering[] orderings)
        {
            StringBuilder builder = new StringBuilder("Group By ");
            for (int i = 0; i < Names.Length; ++i)
            {
                builder.AppendFormat("[{0}] {1} {2}", Names[i], orderings[i].ToString(), (i < Names.Length - 1 ? "," : " "));
            }
            return builder.ToString();
        }
    }
}
