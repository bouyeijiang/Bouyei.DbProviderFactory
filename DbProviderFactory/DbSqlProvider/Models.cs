/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:26:54
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: 6b4dd4de-0617-45f5-8972-80289aa7adf3
---------------------------------------------------------------*/
using System;
using System.Runtime.InteropServices;

namespace Bouyei.ProviderFactory.DbSqlProvider
{
    /// <summary>
    /// 排序方式
    /// </summary>
    [Flags]
    public enum Ordering
    {
        /// <summary>
        /// 降序
        /// </summary>
        Desc = 0,
        /// <summary>
        /// 升序
        /// </summary>
        Asc = 2
    }

    [Flags]
    public enum AndOr
    {
        /// <summary>
        /// 与
        /// </summary>
        And = 0,
        /// <summary>
        /// 或
        /// </summary>
        Or
    }

    [StructLayout(LayoutKind.Sequential)]
    public class KeyValue
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
