/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/7/12 9:53:15
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: Microsoft
 *   guid: 3cd366b1-356b-4a36-bf5c-aa0decc4bdec
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using System.Reflection;

namespace Bouyei.DbProviderFactory.DbAdoProvider
{
     public class DbConn
    {
        protected System.Data.Common.DbProviderFactory dbFactory = null;

        protected DbConnection dbConn = null;
        protected DbDataAdapter dbDataAdapter = null;
        protected DbCommand dbCommand = null;
        protected DbTransaction dbTransaction = null;
        protected DbBulkCopy dbBulkCopy = null;
        protected DbCommandBuilder dbCommandBuilder = null;
        protected ProviderType DbProviderType { get; private set; }

        protected bool IsSingleton { get; private set; }

        protected string ConnectionString { get; private set; }

        //默认预设的工厂提供动态实例，可以直接在app.config配置
        private static Dictionary<ProviderType, AssemblyFactoryInfo> AssemblyCache
            = new Dictionary<ProviderType, AssemblyFactoryInfo>();
        //{
        //{ProviderType.DB2,"IBM.Data.DB2.DB2Factory,IBM.Data.DB2, Culture=neutral,Version=9.7.7.4,PublicKeyToken=7c307b91aa13d208"},
        //{ProviderType.SQLite,"Devart.Data.SQLite.SQLiteProviderFactory, Devart.Data.SQLite, Culture=neutral, Version=5.2.244.0, PublicKeyToken=09af7300eec23701"},
        //{ProviderType.Oracle,"Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess, Culture=neutral, Version=4.121.2.0,PublicKeyToken=89b483f429c47342"},//Oracle.ManagedDataAccess, Version=4.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
        //{ProviderType.MySql,"MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data.MySqlClient, Culture=neutral, Version=5.1.7.0, PublicKeyToken=c5687fc88969c44d"},
        //{ProviderType.MsOracle,"System.Data.OracleClient.OracleClientFactory,System.Data.OracleClient, Culture=neutral,Version=4.0.0.0, PublicKeyToken=b77a5c561934e089"},
        //{ProviderType.OleDb,"System.Data.OleDb.OleDbFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c"},
        //{ProviderType.SqlServer,"System.Data.SqlClient.SqlClientFactory, System.Data, Culture=neutral, Version=4.0.0.0, PublicKeyToken=b77a5c561934e089"},
        //{ProviderType.Odbc,"System.Data.Odbc.OdbcFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}
        //};

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbProviderType"></param>
        /// <param name="IsSingleton"></param>
        protected DbConn(ProviderType dbProviderType,
             bool IsSingleton)
        {
            this.IsSingleton = IsSingleton;
            this.DbProviderType = dbProviderType;

            string invariantName = GetInvariantName(dbProviderType);

            dbFactory = GetDbProviderFactory(invariantName);

            if (dbFactory == null)
                throw new Exception("不提供支持该" + dbProviderType.ToString() + "类型的实例");
        }

        protected DbConnection CreateConnection(string ConnectionString)
        {
            if (IsSingleton)
            {
                if (dbConn == null)
                    dbConn = dbFactory.CreateConnection();
            }
            else
            {
                if (dbConn != null) dbConn.Dispose();
                dbConn = dbFactory.CreateConnection();
            }
            if (dbConn.ConnectionString != ConnectionString)
            {
                if (dbConn.State != ConnectionState.Closed) dbConn.Close();
                dbConn.ConnectionString = (this.ConnectionString = ConnectionString);
            }

            return dbConn;
        }

        protected DbDataAdapter CreateAdapter()
        {
            if (IsSingleton)
            {
                if (dbDataAdapter == null)
                    dbDataAdapter = dbFactory.CreateDataAdapter();
            }
            else
            {
                if (dbDataAdapter != null) dbDataAdapter.Dispose();
                dbDataAdapter = dbFactory.CreateDataAdapter();
            }

            return dbDataAdapter;
        }

        protected DbCommandBuilder CreateCommandBuilder()
        {
            if (IsSingleton)
            {
                if (dbCommandBuilder == null)
                    dbCommandBuilder = dbFactory.CreateCommandBuilder();
            }
            else
            {
                if (dbCommandBuilder != null) dbCommandBuilder.Dispose();
                dbCommandBuilder = dbFactory.CreateCommandBuilder();
            }
            return dbCommandBuilder;
        }

        protected DbCommand CreateCommand(DbExecuteParameter dbParameter,
            DbConnection dbConn, DbTransaction dbTrans = null)
        {
            if (IsSingleton)
            {
                if (dbCommand == null)
                    dbCommand = dbFactory.CreateCommand();
            }
            else
            {
                if (dbCommand != null) dbCommand.Dispose();
                dbCommand = dbFactory.CreateCommand();
            }
            if (dbTrans != null) dbCommand.Transaction = dbTrans;
            if (dbParameter.IsStoredProcedure) dbCommand.CommandType = CommandType.StoredProcedure; 
            
            dbCommand.Connection = dbConn;
            dbCommand.CommandText = dbParameter.CommandText;
            dbCommand.CommandTimeout = dbParameter.ExectueTimeout;

            if (dbParameter.dbProviderParameters != null)
            {
                foreach (DbProviderParameter param in dbParameter.dbProviderParameters)
                {
                    dbCommand.Parameters.Add(param);
                }
            }

            return dbCommand;
        }

        protected DbTransaction BeginTransaction(DbConnection dbConn, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            dbTransaction = dbConn.BeginTransaction(isolationLevel);
            return dbTransaction;
        }

        protected DbBulkCopy CreateBulkCopy(string ConnectionString, bool isTransaction = false)
        {
            if (dbBulkCopy != null) dbBulkCopy.Dispose();

            if (isTransaction)
                dbBulkCopy = new DbBulkCopy(DbProviderType, ConnectionString, CreateConnection(ConnectionString));
            else
                dbBulkCopy = new DbBulkCopy(DbProviderType, ConnectionString);

            return dbBulkCopy;
        }

        private System.Data.Common.DbProviderFactory GetDbProviderFactory(string invariantName)
        {
            if (ExistsDbProviderFactories(invariantName))
                return DbProviderFactories.GetFactory(invariantName);
            else
            {
                AssemblyFactoryInfo assemInfo = null;

                AssemblyCache.TryGetValue(DbProviderType, out assemInfo);
                if (assemInfo == null)
                {
                    assemInfo = GetDynamicDllProviderInfo(invariantName);
                    assemInfo.FactoryName = GetFactoryName(DbProviderType);

                    AssemblyCache.Add(DbProviderType, assemInfo);
                }

                Type type = Type.GetType(assemInfo.ToString());
                if (type == null) return null;

                FieldInfo field = type.GetField("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                if (field == null || !field.FieldType.IsSubclassOf(typeof(System.Data.Common.DbProviderFactory))) return null;

                object obj = field.GetValue(null);
                if (obj == null) return null;

                return (System.Data.Common.DbProviderFactory)obj;
            }
        }

        private string GetInvariantName(ProviderType providerType)
        {
            string invariantName = "System.Data.SqlClient";
            switch (providerType)
            {
                case ProviderType.DB2: invariantName = "IBM.Data.DB2"; break;
                case ProviderType.MsOracle: invariantName = "System.Data.OracleClient"; break;
                case ProviderType.Oracle: invariantName = "Oracle.DataAccess"; break;
                case ProviderType.MySql: invariantName = "MySql.Data.MySqlClient"; break;
                case ProviderType.SQLite: invariantName = "System.Data.SQLite"; break;
                case ProviderType.OleDb: invariantName = "System.Data.OleDb"; break;
                case ProviderType.Odbc: invariantName = "System.Data.Odbc"; break;
                case ProviderType.SqlServer:
                default: break;
            }
            return invariantName;
        }

        private string GetFactoryName(ProviderType providerType)
        {
            string factoryName = "System.Data.SqlClient.SqlClientFactory";

            switch (providerType)
            {
                case ProviderType.DB2:
                    factoryName = "IBM.Data.DB2.DB2Factory";
                    break;
                case ProviderType.Oracle:
                    factoryName = "Oracle.DataAccess.Client.OracleClientFactory";
                    break;
                case ProviderType.MySql:
                    factoryName = "MySql.Data.MySqlClient.MySqlClientFactory";
                    break;
                case ProviderType.SQLite:
                    factoryName = "Devart.Data.SQLite.SQLiteProviderFactory";
                    break;
                case ProviderType.OleDb:
                    factoryName = "System.Data.OleDb.OleDbFactory";
                    break;
                case ProviderType.Odbc:
                    factoryName = "System.Data.Odbc.OdbcFactory";
                    break;
                case ProviderType.SqlServer:
                    factoryName = "System.Data.SqlClient.SqlClientFactory";
                    break;
                case ProviderType.MsOracle:
                    factoryName = "System.Data.OracleClient.OracleClientFactory";
                    break;
                default: break;
            }
            return factoryName;
        }

        private bool ExistsDbProviderFactories(string invariantName)
        {
            return DbProviderFactories.GetFactoryClasses().Rows.Contains(invariantName);
        }

        private AssemblyFactoryInfo GetDynamicDllProviderInfo(string invariantName)
        {
            string path = string.Empty;
            try
            {
                path = AppDomain.CurrentDomain.BaseDirectory + invariantName + ".dll";
                Assembly assem = Assembly.LoadFile(path);

                return new AssemblyFactoryInfo(assem.FullName);
            }
            catch (Exception ex)
            {
                throw new Exception("path:" + path + ";" + ex.ToString());
            }
        }
    }

  
}
