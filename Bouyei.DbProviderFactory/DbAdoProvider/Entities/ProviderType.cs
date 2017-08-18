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

namespace Bouyei.DbProviderFactory.DbAdoProvider
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
        None = -1,
        // 摘要: 
        //     对所有选项使用默认值。
        Default = 0,
        //
        // 摘要: 
        //     保留源标识值。 如果未指定，则由目标分配标识值。
        KeepIdentity = 1,
        //
        // 摘要: 
        //     请在插入数据的同时检查约束。 默认情况下，不检查约束。
        CheckConstraints = 2,
        //
        // 摘要: 
        //     在批量复制操作期间获取批量更新锁。 如果未指定，则使用行锁。
        TableLock = 4,
        //
        // 摘要: 
        //     保留目标表中的空值，而不管默认值的设置如何。 如果未指定，则空值将由默认值替换（如果适用）。
        KeepNulls = 8,
        //
        // 摘要: 
        //     指定后，会导致服务器为插入到数据库中的行激发插入触发器。
        FireTriggers = 16,
        //
        // 摘要: 
        //     如果已指定，则每一批批量复制操作将在事务中发生。 如果指示了此选项，并且为构造函数提供了 System.Data.SqlClient.SqlTransaction
        //     对象，则发生 System.ArgumentException。
        UseInternalTransaction = 32,
    }
}
