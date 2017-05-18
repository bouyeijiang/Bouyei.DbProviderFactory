/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/1/19 15:29:36
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: 35f8c29a-ef2e-4e41-8671-69816404e8a2
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Bouyei.ProviderFactory.DbAdoProvider
{
    using System.Data.SqlClient;

    public class DbBulkCopyDynamic : IDbBulkCopy, IDisposable
    {
        public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public string DestinationTableName { get; set; }

        public int BulkCopyTimeout { get; set; }

        public int BatchSize { get; set; }

        public string ConnectionString { get; private set; }

        public ProviderType DbProviderType { get; private set; }

        private DbBulkCopyReflection dbBulkCopy  { get; set; }
 
        SqlBulkCopy sqlBulkCopy = null;
        SqlConnection SqlConn = null;

        ~DbBulkCopyDynamic()
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
                #region sql
                if (DbProviderType == ProviderType.SqlServer)
                {
                    if (sqlBulkCopy != null)
                    {
                        sqlBulkCopy.Close();
                        sqlBulkCopy = null;
                    }
                    if (SqlConn != null)
                    {
                        SqlConn.Close();
                        SqlConn.Dispose();
                        SqlConn = null;
                    }
                }
                #endregion

  
                if (DbProviderType == ProviderType.DB2
                    ||DbProviderType==ProviderType.Oracle)
                {
                    dbBulkCopy.Dispose(); 
                }
 
            }
        }

        public DbBulkCopyDynamic(ProviderType DbProviderType,
            string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            this.DbProviderType = DbProviderType;

            if (DbProviderType == ProviderType.SqlServer)
            {
                if (sqlBulkCopy == null ||
                    sqlBulkCopy.ColumnMappings == null ||
                    this.ConnectionString != ConnectionString)
                    sqlBulkCopy = new SqlBulkCopy(ConnectionString, SqlBulkCopyOptions.UseInternalTransaction);
            }
            else
            {
                if (dbBulkCopy == null ||
                    this.ConnectionString != ConnectionString)
                    dbBulkCopy = new DbBulkCopyReflection(new DbBulkCopyProvider(DbProviderType), ConnectionString);
            }
        }

        public void Close()
        {
            if (DbProviderType == ProviderType.SqlServer)
            {
                if (sqlBulkCopy != null) sqlBulkCopy.Close();
            }
            else
            {
                dbBulkCopy.Close();
            }
        }

        public void Open()
        {
            if (DbProviderType == ProviderType.SqlServer)
            {
                if (SqlConn != null && SqlConn.State != ConnectionState.Open) SqlConn.Open();
            }
        }

        public void WriteToServer(DataTable dataTable)
        {
            if (DbProviderType == ProviderType.SqlServer)
            {
                #region sql bulkcopy
                if (sqlBulkCopy.ColumnMappings == null) sqlBulkCopy = new SqlBulkCopy(ConnectionString);

                sqlBulkCopy.ColumnMappings.Clear();
                sqlBulkCopy.ColumnMappings.Capacity = dataTable.Columns.Count;
                sqlBulkCopy.DestinationTableName = DestinationTableName;

                for (int i = 0; i < dataTable.Columns.Count; ++i)
                {
                    sqlBulkCopy.ColumnMappings.Add(dataTable.Columns[i].ColumnName,
                        dataTable.Columns[i].ColumnName);
                }

                if (BulkCopiedHandler != null)
                {
                    sqlBulkCopy.SqlRowsCopied += sqlBulkCopy_SqlRowsCopied;
                }

                sqlBulkCopy.BulkCopyTimeout = BulkCopyTimeout;
                sqlBulkCopy.BatchSize = BatchSize;
                sqlBulkCopy.WriteToServer(dataTable);
                #endregion
            }
            else
            {
                dbBulkCopy.BulkCopyTimeout = BulkCopyTimeout;
               
                dbBulkCopy.WriteToServer(dataTable);
            }
        }
     
        public void WriteToServer(DataTable dataTable, DataRowState rowState)
        {
            if (DbProviderType == ProviderType.SqlServer)
            {
                #region sql bulkcopy
                if (sqlBulkCopy.ColumnMappings == null) sqlBulkCopy = new SqlBulkCopy(ConnectionString);
                sqlBulkCopy.ColumnMappings.Clear();
                sqlBulkCopy.ColumnMappings.Capacity = dataTable.Columns.Count;
                sqlBulkCopy.DestinationTableName = DestinationTableName;

                for (int i = 0; i < dataTable.Columns.Count; ++i)
                {
                    sqlBulkCopy.ColumnMappings.Add(dataTable.Columns[i].ColumnName,
                        dataTable.Columns[i].ColumnName);
                }

                if (BulkCopiedHandler != null)
                {
                    sqlBulkCopy.SqlRowsCopied += sqlBulkCopy_SqlRowsCopied;
                }

                sqlBulkCopy.BulkCopyTimeout = BulkCopyTimeout;
                sqlBulkCopy.BatchSize = BatchSize;

                sqlBulkCopy.WriteToServer(dataTable, rowState);
                #endregion
            }
            else
            {
                dbBulkCopy.BulkCopyTimeout = BulkCopyTimeout;
                dbBulkCopy.WriteToServer(dataTable, rowState);
            }
        }

        void sqlBulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            BulkCopiedHandler(e.RowsCopied);
        }
    }
}
