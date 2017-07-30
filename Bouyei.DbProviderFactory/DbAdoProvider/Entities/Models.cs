/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:55:33
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: 6bceaafe-f915-41b8-87c5-f439de40ab16
---------------------------------------------------------------*/
using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbProviderFactory.DbAdoProvider
{
    [Serializable]
    public class ResultInfo<R, I>
    {
        public R Result { get; set; }

        public I Info { get; set; }

        public ResultInfo()
        {
        }

        public ResultInfo(R Result)
        {
            this.Result = Result;
        }

        public ResultInfo(I Info)
        {
            this.Info = Info;
        }

        public ResultInfo(R Result, I Info)
        {
            this.Result = Result;
            this.Info = Info;
        }

        public static ResultInfo<R, I> Create<R, I>(R Result, I Info)
        {
            return new ResultInfo<R, I>(Result, Info);
        }
    }

    internal class AssemblyFactoryInfo
    {
        public string FactoryName { get; set; }

        public string InvariantName { get; set; }

        public string Culture { get; set; }

        public string Version { get; set; }

        public string PublicKeyToken { get; set; }

        public string AssemblyName { get; private set; }

        public AssemblyFactoryInfo()
        { }

        public AssemblyFactoryInfo(string AssemblyFullName)
        {
            this.AssemblyName = AssemblyFullName;
            string[] infos = AssemblyFullName.Split(',');
            this.InvariantName = infos[0];
            this.Version = infos[1].Split('=')[1];
            this.Culture = infos[2].Split('=')[1];
            this.PublicKeyToken = infos[3].Split('=')[1];
        }

        public override string ToString()
        {
            return string.Format("{0},{1},Version={2},Culture={3},PublicKeyToken={4}",
                FactoryName,
                InvariantName,
                Version,
                Culture,
                PublicKeyToken);
        }
    }

    public class DbProviderParameter : DbParameter
    {
        public override DbType DbType { get; set; }

        public override string ParameterName { get; set; }

        public override int Size { get; set; }

        public override object Value { get; set; }

        public override ParameterDirection Direction { get; set; }

        public override string SourceColumn { get; set; }

        public override DataRowVersion SourceVersion { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override bool IsNullable { get; set; }

        public override void ResetDbType()
        {
            throw new NotImplementedException();
        }
    }

    public class DbExecuteParameter
    {
        public DbExecuteParameter(int ExecuteTimeout)
        {
            this.executeTimeout = ExecuteTimeout;
        }

        public DbExecuteParameter(params DbProviderParameter[] dbProviderParameters)
        {
            this.dbProviderParameters = dbProviderParameters;
        }

        public DbExecuteParameter(string CommandText,
            int ExectueTimeout = 1800,
            DbProviderParameter[] dbProviderParameters = null)
        {
            this.CommandText = CommandText;
            this.executeTimeout = ExectueTimeout;
            this.dbProviderParameters = dbProviderParameters;
        }

        public bool IgnoreCase { get; set; }

        public string CommandText { get; set; }

        private int executeTimeout = 1800;
        /// <summary>
        /// 超时默认值,1800s
        /// </summary>
        public int ExectueTimeout { get { return executeTimeout; } set { executeTimeout = value; } }

        public DbProviderParameter[] dbProviderParameters { get; set; }
    }

    public class DbExecuteBulkParameter : DbExecuteParameter
    {

        public DbExecuteBulkParameter()
            : base()
        { }

        public DbExecuteBulkParameter(DataTable dstDataTable,
            int BatchSize = 10240,
            int ExecuteTimeout = 1800,
            bool IsTransaction = false)
            : base(ExecuteTimeout)
        {
            this.DstDataTable = dstDataTable;
            this.DstTableName = dstDataTable.TableName;
            this.batchSize = BatchSize;
            this.IsTransaction = IsTransaction;
        }

        public DbExecuteBulkParameter(string dstTableName, IDataReader iDataReader,
           int BatchSize = 10240,
           int ExecuteTimeout = 1800,
           bool IsTransaction = false)
            : base(ExecuteTimeout)
        {
            this.IDataReader = iDataReader;
            this.DstTableName = dstTableName;
            this.batchSize = BatchSize;
            this.IsTransaction = IsTransaction;
        }

        public string DstTableName { get; private set; }
        /// <summary>
        /// 如果使用DataTable该数据集批量写入，必需设置TableName
        /// </summary>
        public DataTable DstDataTable { get; set; }

        public IDataReader IDataReader { get; set; }

        private int batchSize = 10240;
        /// <summary>
        /// 批量大小,默认10240
        /// </summary>
        public int BatchSize { get { return batchSize; } set { batchSize = value; } }

        /// <summary>
        /// 是否启用事务
        /// </summary>
        public bool IsTransaction { get; set; }

        public bool IsAutoDispose { get; set; }

        public Action<IDbTransaction, int> TransactionCallback { get; set; }

        public BulkCopiedArgs BulkCopiedHandler { get; set; }
    }
    
    [Serializable]
    public class ConnectionConfiguraton
    {
        public ProviderType DbType { get; set; }

        public string DbIp { get; set; }

        public int DbPort { get; set; }

        public string DbName { get; set; }

        public string DbUserName { get; set; }

        public string DbPassword { get; set; }

        public string ConnectionString { get; private set; }

        public override string ToString()
        {
            switch (DbType)
            {
                case ProviderType.SqlServer:
                    {
                        if (DbPort <= 0) DbPort = 1433;
                        ConnectionString = string.Format("Server={0},{1};Database={2};User Id={3};Password={4};",
                            DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case ProviderType.Oracle:
                case ProviderType.MsOracle:
                    {
                        if (DbPort <= 0) DbPort = 1521;

                        if (string.IsNullOrEmpty(DbName))
                            DbName = "ORCL";
                        ConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4};",
                             DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case ProviderType.DB2:
                    {
                        if (DbPort <= 0) DbPort = 50000;

                        ConnectionString = string.Format("server={0},{1};database={2};uid={3};pwd={4};",
                            DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case ProviderType.MySql:
                    {
                        if (DbPort <= 0) DbPort = 3306;
                        ConnectionString = string.Format("Data Source={0};port={1};Database={2};User Id={3};Password={4};pooling=false;CharSet=utf8;",
                          DbIp, DbPort, DbName, DbUserName, DbPassword);
                    }
                    break;
                case ProviderType.SQLite:
                    ConnectionString = string.Format("Data Source={0};Version=3;Password={1};", DbIp, DbPassword);
                    break;
                case ProviderType.Odbc:
                case ProviderType.OleDb:
                    ConnectionString = "未定义该连接类型字符串";
                    break;
                default:
                    ConnectionString = "未知连接类型";
                    break;
            }
            return ConnectionString;
        }
    }
}
