/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/1/19 14:55:53
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: a097c23d-6886-4420-a23f-0480a8141ec3
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bouyei.ProviderFactory.DbAdoProvider
{
    public delegate void BulkCopiedArgs(long rows);

    [Flags]
    public enum ProviderType
    {
        SqlServer = 0x00,
        DB2 = 0x02,
        [Obsolete("请使用Oracle 第三方provider")]
        MsOracle = 0x04,
        Oracle = 0x08,
        MySql = 0x16,
        SQLite = 0x32,
        OleDb = 0x64,
        Odbc = 0x128
    }

    [Flags]
    public enum BulkCopyOptions
    {
        Default = 0,
        InternalTransaction = 1
    }
}
