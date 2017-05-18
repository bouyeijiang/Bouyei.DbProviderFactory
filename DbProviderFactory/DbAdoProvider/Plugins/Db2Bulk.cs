/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/2/10 9:33:53
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: fd219fea-b1b9-48b2-b864-2f26d24f678e
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Bouyei.ProviderFactory.DbAdoProvider.Plugins
{
    using IBM.Data.DB2;

    public class Db2Bulk
    {
        DB2BulkCopy bulkCopy = null;
        public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public BulkCopyOptions Option { get; private set; }

        public string ConnectionString { get; private set; }

        public Db2Bulk(string ConnectionString, int timeout = 1800, BulkCopyOptions option = BulkCopyOptions.InternalTransaction)
        {
            this.Option = option;
            this.ConnectionString = ConnectionString;
            bulkCopy = new DB2BulkCopy(ConnectionString, (DB2BulkCopyOptions)option);
            bulkCopy.BulkCopyTimeout = timeout;
        }

        public Db2Bulk(IDbConnection dbConnection, int timeout = 1800, BulkCopyOptions option = BulkCopyOptions.InternalTransaction)
        {
            this.Option = option;
            this.ConnectionString = ConnectionString;
            DB2Connection oracleConnection = (DB2Connection)dbConnection;
            bulkCopy = new DB2BulkCopy(oracleConnection, (DB2BulkCopyOptions)option);
            bulkCopy.BulkCopyTimeout = timeout;
        }

        public void Dispose()
        {
            if (bulkCopy != null)
            {
                bulkCopy.Close();
                bulkCopy.Dispose();
                bulkCopy = null;
            }
        }

        public void Close()
        {
            if (bulkCopy != null)
                bulkCopy.Close();
        }

        private void InitBulkCopy(DataTable dt)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.ColumnMappings.Capacity = dt.Columns.Count;
            bulkCopy.DestinationTableName = dt.TableName;

            for (int i = 0; i < dt.Columns.Count; ++i)
            {
                bulkCopy.ColumnMappings.Add(dt.Columns[i].ColumnName,
                    dt.Columns[i].ColumnName);
            }
            if (BulkCopiedHandler != null)
            {
                bulkCopy.DB2RowsCopied += bulkCopy_DB2RowsCopied;
            }
        }

        private void InitBulkCopy(string tableName, string[] columnNames)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.DestinationTableName = tableName;
            for (int i = 0; i < columnNames.Length; ++i)
            {
                bulkCopy.ColumnMappings.Add(columnNames[i],
                    columnNames[i]);
            }

            if (BulkCopiedHandler != null)
            {
                bulkCopy.DB2RowsCopied += bulkCopy_DB2RowsCopied;
            }
        }

        private void InitBulkCopy(string tableName)
        {
            if (bulkCopy.ColumnMappings.Count > 0) bulkCopy.ColumnMappings.Clear();

            bulkCopy.DestinationTableName = tableName;

            if (BulkCopiedHandler != null)
            {
                bulkCopy.DB2RowsCopied += bulkCopy_DB2RowsCopied;
            }
        }

        void bulkCopy_DB2RowsCopied(object sender, DB2RowsCopiedEventArgs e)
        {
            if (BulkCopiedHandler != null)
            {
                BulkCopiedHandler(e.RowsCopied);
            }
        }

        public void WriteToServer(DataTable dt)
        {
            InitBulkCopy(dt);
            bulkCopy.WriteToServer(dt);

            if (bulkCopy.Errors.Count > 0)
            {
                throw new Exception(string.Format("入库失败条数:{0}信息;{1}", bulkCopy.Errors.Count, bulkCopy.Errors[0].Message));
            }
        }

        public void WriteToServer(string tableName, IDataReader iDataReader)
        {
            string[] columnNames = new string[iDataReader.FieldCount];
            for (int i = 0; i < columnNames.Length; ++i)
            {
                columnNames[i] = iDataReader.GetName(i);
            }
            InitBulkCopy(tableName, columnNames);
            bulkCopy.WriteToServer(iDataReader);
        }

        public void WriteToServer(string tableName, DataRow[] rows)
        {
            InitBulkCopy(tableName);

            bulkCopy.WriteToServer(rows);
        }

        public void WriteToServer(DataTable dt, DataRowState rowState)
        {
            InitBulkCopy(dt);
            bulkCopy.WriteToServer(dt, rowState);

            if (bulkCopy.Errors.Count > 0)
            {
                throw new Exception(string.Format("入库失败条数:{0}信息;{1}", bulkCopy.Errors.Count, bulkCopy.Errors[0].Message));
            }
        }

        public void WriteToServer(DataRow[] rows)
        {
            InitBulkCopy(rows[0].Table);
            bulkCopy.WriteToServer(rows);

            if (bulkCopy.Errors.Count > 0)
            {
                throw new Exception(string.Format("入库失败条数:{0}信息;{1}", bulkCopy.Errors.Count, bulkCopy.Errors[0].Message));
            }
        }

    }
}
