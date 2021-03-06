﻿/*-------------------------------------------------------------
 *auth:bouyei
 *date:2016/4/26 9:19:56
 *contact:qq453840293
 *machinename:BOUYEI-PC
 *company/organization:Microsoft
 *profile:www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Bouyei.DbProviderFactory.DbAdoProvider
{
    public interface IDbProvider: IDisposable
    {
        string DbConnectionString { get; set; }

        ProviderType DbType { get; set; }
        ResultInfo<bool, string> Connect(string connString);
        ResultInfo<DataTable, string> Query(DbExecuteParameter dbExecuteParameter);
        ResultInfo<List<T>, string> Query<T>(DbExecuteParameter dbExecuteParameter) where T : new();
        ResultInfo<DataSet, string> QueryToSet(DbExecuteParameter dbExecuteParameter);
        ResultInfo<int, string> QueryTo<T>(DbExecuteParameter dbExecuteParameter, Func<T, bool> rowAction) where T : new();
        ResultInfo<int, string> QueryToReader(DbExecuteParameter dbExecuteParameter, Func<IDataReader,bool> rowAction);
        ResultInfo<IDataReader, string> QueryToReader(DbExecuteParameter dbExecuteParameter);
        ResultInfo<int, string> QueryChanged(DbExecuteParameter dbExecuteParameter, Func<DataTable,bool> action);
        ResultInfo<int, string> QueryToTable(DbExecuteParameter dbExecuteParameter, DataTable dstTable);
        ResultInfo<int, string> ExecuteCmd(DbExecuteParameter dbExecuteParameter);

        ResultInfo<int, string> ExecuteTransaction(DbExecuteParameter dbExecuteParameter);

        ResultInfo<int, string> ExecuteTransaction(string[] CommandTexts, int timeout = 1800);

        ResultInfo<T, string> ExecuteScalar<T>(DbExecuteParameter dbExecuteParameter);

        ResultInfo<int, string> BulkCopy(DbExecuteBulkParameter dbExecuteParameter);
    }
}
