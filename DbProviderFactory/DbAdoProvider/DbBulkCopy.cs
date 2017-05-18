/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/7/12 11:59:12
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: Microsoft
 *   guid: 83d74724-7c1b-4d29-be1e-b758a8a2f17c
---------------------------------------------------------------*/
using System;
using System.Data;

namespace Bouyei.ProviderFactory.DbAdoProvider
{
    using Plugins;

    public class DbBulkCopy : IDbBulkCopy, IDisposable
    {
        public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public string DestinationTableName { get; set; }

        public int BulkCopyTimeout { get; set; }

        public int BatchSize { get; set; }

        public string ConnectionString { get; private set; }

        public ProviderType ProviderName { get; private set; }

        public BulkCopyOptions DbBulkCopyOption { get; set; }

        public bool IsTransaction { get; private set; }

        public IDbTransaction dbTrans { get; private set; }

        public IDbConnection dbConn { get; private set; }

        SqlBulk sqlBulkCopy = null;
        Db2Bulk db2BulkCopy = null;
        OracleBulk oracleBulkCopy = null;

        ~DbBulkCopy()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ProviderName == ProviderType.SqlServer) sqlBulkCopy.Dispose();
                else if (ProviderName == ProviderType.DB2) db2BulkCopy.Dispose();
                else if (ProviderName == ProviderType.Oracle) oracleBulkCopy.Dispose();
            }
        }

        protected DbBulkCopy(ProviderType providerType,
           string connectionString)
        {
            this.ConnectionString = connectionString;
            this.ProviderName = providerType;
        }

        public DbBulkCopy(ProviderType providerType, string connectionString,
            int bulkcopyTimeout = 1800,
            int batchSize = 102400,
            BulkCopyOptions dbBulkCopyOption = BulkCopyOptions.Default)
            : this(providerType, connectionString)
        {
            this.BatchSize = batchSize;
            this.BulkCopyTimeout = bulkcopyTimeout;
            this.DbBulkCopyOption = dbBulkCopyOption;

            if (ProviderName == ProviderType.SqlServer)
            {
                if (sqlBulkCopy == null || this.ConnectionString != ConnectionString)
                {
                    sqlBulkCopy = new SqlBulk(ConnectionString, BulkCopyTimeout, DbBulkCopyOption);
                }
                sqlBulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            }
            else if (ProviderName == ProviderType.DB2)
            {
                if (db2BulkCopy == null || this.ConnectionString != ConnectionString)
                {
                    db2BulkCopy = new Db2Bulk(ConnectionString, BulkCopyTimeout, DbBulkCopyOption);
                }
                db2BulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            }
            else if (ProviderName == ProviderType.Oracle)
            {
                if (oracleBulkCopy == null || this.ConnectionString != ConnectionString)
                {
                    oracleBulkCopy = new OracleBulk(ConnectionString, BulkCopyTimeout, DbBulkCopyOption);
                }
                oracleBulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            }
        }

        public DbBulkCopy(ProviderType providerType, string connectionString, IDbConnection dbConnection,
            int bulkcopyTimeout = 1800,
            int batchSize = 102400,
            BulkCopyOptions dbBulkCopyOption = BulkCopyOptions.Default,
            bool isTransaction = true)
            : this(providerType, connectionString)
        {
            this.BatchSize = batchSize;
            this.BulkCopyTimeout = bulkcopyTimeout;
            this.DbBulkCopyOption = dbBulkCopyOption;
            this.IsTransaction = isTransaction;
            this.dbConn = dbConnection;

            if (ProviderName == ProviderType.SqlServer)
            {
                if (sqlBulkCopy != null || this.ConnectionString != connectionString)
                {
                    if (sqlBulkCopy != null)
                        sqlBulkCopy.Dispose();
                }
                if (dbConn.State != ConnectionState.Open) dbConn.Open();

                if (IsTransaction)
                {
                    dbTrans = dbConn.BeginTransaction();
                }
                sqlBulkCopy = new SqlBulk(dbConn, dbTrans, BulkCopyTimeout, DbBulkCopyOption);

                sqlBulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            }
            else if (ProviderName == ProviderType.DB2)
            {
                if (db2BulkCopy != null || this.ConnectionString != connectionString)
                {
                    if (db2BulkCopy != null)
                        db2BulkCopy.Dispose();
                }

                if (dbConn.State != ConnectionState.Open) dbConn.Open();

                if (isTransaction)
                {
                    dbTrans = dbConn.BeginTransaction();
                }
                db2BulkCopy = new Db2Bulk(dbConn, BulkCopyTimeout, DbBulkCopyOption);

                db2BulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            }
            else if (ProviderName == ProviderType.Oracle)
            {
                if (oracleBulkCopy != null || this.ConnectionString != connectionString)
                {
                    if (oracleBulkCopy != null)
                        oracleBulkCopy.Dispose();
                }

                if (dbConn.State != ConnectionState.Open) dbConn.Open();

                if (isTransaction)
                {
                    dbTrans = dbConn.BeginTransaction();
                }

                oracleBulkCopy = new OracleBulk(dbConn, BulkCopyTimeout, DbBulkCopyOption);
                oracleBulkCopy.BulkCopiedHandler = BulkCopiedHandler;
            }
        }

        public void Close()
        {
            if (ProviderName == ProviderType.DB2) db2BulkCopy.Close();
            else if (ProviderName == ProviderType.SqlServer) sqlBulkCopy.Close();
            else if (ProviderName == ProviderType.Oracle) oracleBulkCopy.Close();
        }

        public void Open()
        {
            if (!IsTransaction) return;

            if (dbConn.State != ConnectionState.Open) dbConn.Open();
        }

        public void WriteToServer(DataTable dataTable)
        {
            if (ProviderName == ProviderType.SqlServer)
            {
                sqlBulkCopy.WriteToServer(dataTable, BatchSize);
            }
            else if (ProviderName == ProviderType.DB2)
            {
                db2BulkCopy.WriteToServer(dataTable);
            }
            else if (ProviderName == ProviderType.Oracle)
            {
                oracleBulkCopy.WriteToServer(dataTable, BatchSize);
            }
            else
            {
                throw new Exception("暂时不支持" + ProviderName.ToString() + "的批量方法...");
            }
        }

        public void WriteToServer(DataTable dataTable, DataRowState rowState)
        {
            if (ProviderName == ProviderType.SqlServer)
            {
                sqlBulkCopy.WriteToServer(dataTable, rowState, BatchSize);
            }
            else if (ProviderName == ProviderType.DB2)
            {
                db2BulkCopy.WriteToServer(dataTable, rowState);
            }
            else if (ProviderName == ProviderType.Oracle)
            {
                oracleBulkCopy.WriteToServer(dataTable, rowState, BatchSize);
            }
            else
            {
                throw new Exception("暂时不支持" + ProviderName.ToString() + "的批量方法...");
            }
        }

        public void WriteToServer(IDataReader iDataReader, string dstTableName)
        {
            if (ProviderName == ProviderType.SqlServer)
            {
                sqlBulkCopy.WriteToServer(dstTableName, iDataReader, this.BatchSize);
            }
            else if (ProviderName == ProviderType.DB2)
            {
                db2BulkCopy.WriteToServer(dstTableName, iDataReader);
            }
            else if (ProviderName == ProviderType.Oracle)
            {
                oracleBulkCopy.WriteToServer(dstTableName, iDataReader, BatchSize);
            }
            else
            {
                throw new Exception("暂时不支持" + ProviderName.ToString() + "的批量方法...");
            }
        }
    }
}
