/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/2/10 9:32:41
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: 9d334263-66e6-4107-9e09-361f1b5df138
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace Bouyei.ProviderFactory.DbAdoProvider.Plugins
{
    public class SqlBulk
    {
        SqlBulkCopy bulkCopy = null;
        public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public BulkCopyOptions Option { get; private set; }

        public string ConnectionString { get; private set; }

        public SqlBulk(string ConnectionString, int timeout = 1800, BulkCopyOptions option = BulkCopyOptions.InternalTransaction)
        {
            this.Option = option;
            this.ConnectionString = ConnectionString;

            bulkCopy = new SqlBulkCopy(ConnectionString, (SqlBulkCopyOptions)option);
            bulkCopy.BulkCopyTimeout = timeout;
        }

        public SqlBulk(IDbConnection dbConnection, IDbTransaction dbTrans = null,
            int timeout = 1800, BulkCopyOptions option = BulkCopyOptions.InternalTransaction)
        {
            this.Option = option;
            this.ConnectionString = ConnectionString;

            SqlConnection connection = (SqlConnection)dbConnection;
            if (dbTrans == null) bulkCopy = new SqlBulkCopy(connection);
            else bulkCopy = new SqlBulkCopy(connection, (SqlBulkCopyOptions)option, (SqlTransaction)dbTrans);

            bulkCopy.BulkCopyTimeout = timeout;
        }

        public void Dispose()
        {
            if (bulkCopy != null)
            {
                bulkCopy.Close();
                bulkCopy = null;
            }
        }

        public void Close()
        {
            if (bulkCopy != null)
                bulkCopy.Close();
        }

        private void InitBulkCopy(DataTable dt, int batchSize = 102400)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.ColumnMappings.Capacity = dt.Columns.Count;
            bulkCopy.DestinationTableName = dt.TableName;
            bulkCopy.BatchSize = batchSize;

            for (int i = 0; i < dt.Columns.Count; ++i)
            {
                bulkCopy.ColumnMappings.Add(dt.Columns[i].ColumnName,
                    dt.Columns[i].ColumnName);
            }
            if (BulkCopiedHandler != null)
            {
                bulkCopy.SqlRowsCopied += bulkCopy_SqlRowsCopied;
            }
        }

        private void InitBulkCopy(string tableName, string[] columnNames, int batchSize = 102400)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BatchSize = batchSize;

            for (int i = 0; i < columnNames.Length; ++i)
            {
                bulkCopy.ColumnMappings.Add(columnNames[i],
                    columnNames[i]);
            }
            if (BulkCopiedHandler != null)
            {
                bulkCopy.SqlRowsCopied += bulkCopy_SqlRowsCopied;
            }
        }

        private void InitBulkCopy(string tableName, int batchSize = 102400)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BatchSize = batchSize;

            if (BulkCopiedHandler != null)
            {
                bulkCopy.SqlRowsCopied += bulkCopy_SqlRowsCopied;
            }
        }

        void bulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            if (BulkCopiedHandler != null)
            {
                BulkCopiedHandler(e.RowsCopied);
            }
        }

        public void WriteToServer(DataTable dt, int batchSize = 102400)
        {
            InitBulkCopy(dt, batchSize);
            bulkCopy.WriteToServer(dt);
        }

        public void WriteToServer(string tableName, IDataReader iDataReader, int batchSize = 102400)
        {
            string[] columnNames = new string[iDataReader.FieldCount];
            for (int i = 0; i < columnNames.Length; ++i)
            {
                columnNames[i] = iDataReader.GetName(i);
            }
            InitBulkCopy(tableName, columnNames, batchSize);
            bulkCopy.WriteToServer(iDataReader);
        }

        public void WriteToServer(string tableName, DataRow[] rows)
        {
            InitBulkCopy(tableName);
            bulkCopy.WriteToServer(rows);
        }

        public void WriteToServer(DataTable dt, DataRowState rowState, int batchSize = 102400)
        {
            InitBulkCopy(dt, batchSize);
            bulkCopy.WriteToServer(dt, rowState);
        }

        public void WriteToServer(DataRow[] rows, int batchSize = 102400)
        {
            InitBulkCopy(rows[0].Table, batchSize);
            bulkCopy.WriteToServer(rows);
        }
    }
}
