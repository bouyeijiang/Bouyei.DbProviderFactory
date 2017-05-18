using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Bouyei.ProviderFactory
{
    interface IDbLayer
    {
        ResultInfo<bool,string> Connect(string ConnectionString);

        ResultInfo<int, string> Execute(string command);

        ResultInfo<DataSet, string> Query(string command);

        ResultInfo<int, string> ExecuteTransaction(string command);

        //ReturnInfo<int> ExecuteTransaction(string command, DbParameter[] commandParams);

        //ReturnInfo<int> ExecuteTransaction(List<commandParamsInfo> paramsInfo);

        //ReturnInfo<int> ExecuteStoredProcedure(string procedureName, DbParameter[] dbParam);

        //ReturnInfo<int> ExecuteStoredProcedure(string procedureName, DbParameter[] dbParam, DbParameter outParam);

        //ReturnInfo<DataSet> ExecuteStoredProcedureDataSet(string procedureName, DbParameter[] dbParam);

    }
}
