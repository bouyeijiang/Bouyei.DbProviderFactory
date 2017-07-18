/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/4/26 9:19:46
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: Microsoft
 *   guid: 93caaa3a-b22b-4b82-8d92-62d598962222
---------------------------------------------------------------*/
using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading;

namespace Bouyei.ProviderFactory.DbAdoProvider
{
    using UtilIO;
    /// <summary>
    /// 连接池
    /// </summary>
    public class DbProvider : DbConn, IDbProvider, IDisposable
    {
        #region variable
        private int signal = 0;

        public string DbConnectionString { get; set; }
        public ProviderType ProviderType { get; set; }
        public int LockTimeout { get; set; }

        #endregion

        #region  dispose
        ~DbProvider()
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
                if (this.dbConn != null) this.dbConn.Dispose();
                if (this.dbDataAdapter != null) this.dbDataAdapter.Dispose();
                if (this.dbCommand != null) this.dbCommand.Dispose();
                if (this.dbBulkCopy != null) this.dbBulkCopy.Dispose();
                if (this.dbCommandBuilder != null) dbCommandBuilder.Dispose();
                if (this.dbTransaction != null) dbTransaction.Dispose();
            }
        }

        #endregion

        #region  structure
        public DbProvider(
            string connectionString,
            ProviderType providerType = ProviderType.SqlServer,
            bool isSingleton = false)
            : base(providerType, isSingleton)
        {
            this.ProviderType = providerType;
            this.DbConnectionString = connectionString;
        }

        public DbProvider(
            ProviderType providerType = ProviderType.SqlServer,
            bool isSingleton = false)
            : base(providerType, isSingleton)
        {
            this.ProviderType = providerType;
        }

        public static DbProvider CreateProvider(string connectionString,
            ProviderType providerType=ProviderType.SqlServer)
        {
            return new DbProvider(connectionString, providerType);
        }

        #endregion

        #region public
        public ResultInfo<bool, string> Connect(string ConnectionString)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            this.DbConnectionString = ConnectionString;
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    return new ResultInfo<bool, string>(true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<bool, string>(false, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<DataTable, string> Query(DbExecuteParameter dbExecuteParameter)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbDataAdapter adapter = this.CreateAdapter())
                    {
                        adapter.SelectCommand = this.CreateCommand(dbExecuteParameter.CommandText, conn, null, dbExecuteParameter.ExectueTimeout);
                        if (dbExecuteParameter.dbProviderParameters != null)
                        {
                            foreach (DbProviderParameter param in dbExecuteParameter.dbProviderParameters)
                            {
                                adapter.SelectCommand.Parameters.Add(CreateParameter(param));
                            }
                        }
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return new ResultInfo<DataTable, string>(dt, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<DataTable, string>(null, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<DataSet, string> QueryToSet(DbExecuteParameter dbExecuteParameter)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbDataAdapter adapter = CreateAdapter())
                    {
                        adapter.SelectCommand = CreateCommand(dbExecuteParameter.CommandText, conn,
                            null, dbExecuteParameter.ExectueTimeout);

                        if (dbExecuteParameter.dbProviderParameters != null)
                        {
                            foreach (DbProviderParameter param in dbExecuteParameter.dbProviderParameters)
                            {
                                adapter.SelectCommand.Parameters.Add(CreateParameter(param));
                            }
                        }

                        DataSet dt = new DataSet();
                        adapter.Fill(dt);
                        return new ResultInfo<DataSet, string>(dt, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<DataSet, string>(null, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<int, string> QueryToReader(DbExecuteParameter dbExecuteParameter, Action<IDataReader> rowAction)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                int rows = 0;
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbCommand cmd = CreateCommand(dbExecuteParameter.CommandText, conn))
                    {
                        if (dbExecuteParameter.dbProviderParameters != null)
                        {
                            foreach (DbProviderParameter param in dbExecuteParameter.dbProviderParameters)
                            {
                                cmd.Parameters.Add(CreateParameter(param));
                            }
                        }
                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == false)
                                return ResultInfo<int, string>.Create(-1, "no data rows");

                            while (reader.Read())
                            {
                                rowAction(reader);
                                ++rows;
                            }
                        }
                    }
                }

                return ResultInfo<int, string>.Create<int, string>(rows, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResultInfo<int, string>(-1, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<IDataReader, string> QueryToReader(DbExecuteParameter dbExecuteParameter)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                DbConnection conn = CreateConnection(DbConnectionString);
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    DbCommand cmd = CreateCommand(dbExecuteParameter.CommandText, conn);
                    {
                        if (dbExecuteParameter.dbProviderParameters != null)
                        {
                            foreach (DbProviderParameter param in dbExecuteParameter.dbProviderParameters)
                            {
                                cmd.Parameters.Add(CreateParameter(param));
                            }
                        }
                        IDataReader reader = cmd.ExecuteReader();
                        {
                            return ResultInfo<IDataReader, string>.Create(reader, string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<IDataReader, string>(null, ex.Message);
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<int, string> ExecuteCmd(DbExecuteParameter dbExecuteParameter)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (DbCommand cmd = CreateCommand(dbExecuteParameter.CommandText, conn,
                        null, dbExecuteParameter.ExectueTimeout))
                    {
                        if (dbExecuteParameter.dbProviderParameters != null)
                        {
                            foreach (DbProviderParameter param in dbExecuteParameter.dbProviderParameters)
                            {
                                cmd.Parameters.Add(CreateParameter(param));
                            }
                        }
                        int rt = cmd.ExecuteNonQuery();
                        return new ResultInfo<int, string>(rt, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<int, string>(-1, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<int, string> QueryToTable(DbExecuteParameter dbExecuteParameter, DataTable dstTable)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbCommand cmd = CreateCommand(dbExecuteParameter.CommandText, conn,
                        null, dbExecuteParameter.ExectueTimeout))
                    {
                        if (dbExecuteParameter.dbProviderParameters != null)
                        {
                            foreach (DbProviderParameter param in dbExecuteParameter.dbProviderParameters)
                            {
                                cmd.Parameters.Add(CreateParameter(param));
                            }
                        }

                        using (DbDataReader datareader = cmd.ExecuteReader())
                        {
                            dstTable.Load(datareader);
                            return new ResultInfo<int, string>(datareader.RecordsAffected, string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<int, string>(-1, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        /// <summary>
        /// 同时执行多个sql语句使用
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public ResultInfo<int, string> ExecuteTransaction(DbExecuteParameter dbExecuteParameter)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (DbTransaction tran = BeginTransaction(conn))
                    {
                        try
                        {
                            using (DbCommand cmd = CreateCommand(dbExecuteParameter.CommandText, conn, tran, dbExecuteParameter.ExectueTimeout))
                            {
                                if (dbExecuteParameter.dbProviderParameters != null)
                                {
                                    foreach (DbProviderParameter param in dbExecuteParameter.dbProviderParameters)
                                    {
                                        cmd.Parameters.Add(param);
                                    }
                                }
                                int rt = cmd.ExecuteNonQuery();
                                tran.Commit();
                                return new ResultInfo<int, string>(rt, string.Empty);
                            }
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            return new ResultInfo<int, string>(-1, ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<int, string>(-1, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        /// <summary>
        /// command集合事务批量执行
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="CommandTexts"></param>
        /// <returns></returns>
        public ResultInfo<int, string> ExecuteTransaction(string[] CommandTexts, int timeout = 1800)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (DbTransaction tran = BeginTransaction(conn))
                    {
                        try
                        {
                            int rows = 0;
                            for (int i = 0; i < CommandTexts.Length; ++i)
                            {
                                using (DbCommand cmd = CreateCommand(CommandTexts[i], conn, tran, timeout))
                                {
                                    rows += cmd.ExecuteNonQuery();
                                }
                            }
                            tran.Commit();
                            return new ResultInfo<int, string>(rows, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            return new ResultInfo<int, string>(-1, ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<int, string>(-1, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<T, string> ExecuteScalar<T>(DbExecuteParameter dbExecuteParameter)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (DbCommand cmd = CreateCommand(dbExecuteParameter.CommandText, conn,
                        null, dbExecuteParameter.ExectueTimeout))
                    {
                        if (dbExecuteParameter.dbProviderParameters != null)
                        {
                            foreach (DbProviderParameter param in dbExecuteParameter.dbProviderParameters)
                            {
                                cmd.Parameters.Add(CreateParameter(param));
                            }
                        }

                        object obj = cmd.ExecuteScalar();
                        if (obj == null)
                            return new ResultInfo<T, string>(default(T), string.Empty);
                        return
                            new ResultInfo<T, string>((T)obj, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<T, string>(default(T), ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<int, string> BulkCopy(DbExecuteBulkParameter dbExecuteParameter)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                Exception temex = null;
                int cnt = 0;
                using (DbBulkCopy bulkCopy = CreateBulkCopy(DbConnectionString, dbExecuteParameter.IsTransaction))
                {
                    bulkCopy.Open();
                    bulkCopy.BulkCopiedHandler = dbExecuteParameter.BulkCopiedHandler;

                    bulkCopy.BatchSize = dbExecuteParameter.BatchSize;
                    bulkCopy.BulkCopyTimeout = dbExecuteParameter.ExectueTimeout;

                    try
                    {
                        if ((dbExecuteParameter.DstDataTable == null || dbExecuteParameter.DstDataTable.Rows.Count == 0)
                            && dbExecuteParameter.IDataReader != null)
                        {
                            bulkCopy.WriteToServer(dbExecuteParameter.IDataReader, dbExecuteParameter.DstTableName);
                            cnt = 1;
                        }
                        else
                        {
                            bulkCopy.DestinationTableName = dbExecuteParameter.DstDataTable.TableName;
                            bulkCopy.WriteToServer(dbExecuteParameter.DstDataTable);
                            cnt = dbExecuteParameter.DstDataTable.Rows.Count;
                        }

                        //use transaction
                        if (dbExecuteParameter.IsTransaction)
                        {
                            //有事务回调则由外边控制事务提交,否则直接提交事务
                            if (dbExecuteParameter.TransactionCallback != null)
                                dbExecuteParameter.TransactionCallback(bulkCopy.dbTrans, cnt);
                            else
                                bulkCopy.dbTrans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        temex = ex;
                        if (dbExecuteParameter.IsTransaction)
                        {
                            if (bulkCopy.dbTrans != null)
                                bulkCopy.dbTrans.Rollback();
                        }
                    }
                }

                if (temex != null) throw temex;

                return new ResultInfo<int, string>(cnt, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResultInfo<int, string>(-1, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<List<T>, string> Query<T>(DbExecuteParameter dbExecuteParameter) where T : new()
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbCommand cmd = CreateCommand(dbExecuteParameter.CommandText, conn))
                    {
                        if (dbExecuteParameter.dbProviderParameters != null)
                        {
                            foreach (DbProviderParameter param in dbExecuteParameter.dbProviderParameters)
                            {
                                cmd.Parameters.Add(CreateParameter(param));
                            }
                        }
                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == false)
                                return new ResultInfo<List<T>, string>(new List<T>(1), string.Empty);

                            var items = reader.GetGenericObjectValues<T>(dbExecuteParameter.IgnoreCase);

                            return new ResultInfo<List<T>, string>(items, string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<List<T>, string>(null, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<int, string> QueryChanged(DbExecuteParameter dbExecuteParameter, Action<DataTable> action)
        {
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(LockTimeout);
                //waiting for lock to do;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbDataAdapter adapter = this.CreateAdapter())
                    {
                        adapter.SelectCommand = this.CreateCommand(dbExecuteParameter.CommandText, conn);
                        adapter.SelectCommand.CommandTimeout = dbExecuteParameter.ExectueTimeout;
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count == 0) return new ResultInfo<int, string>(-1, "无可更新的数据行");

                        action(dt);

                        DataTable changedt = dt.GetChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
                        if (changedt != null && changedt.Rows.Count > 0)
                        {
                            using (DbCommandBuilder dbBuilder = this.CreateCommandBuilder())
                            {
                                dbBuilder.DataAdapter = adapter;
                                int rt = adapter.Update(changedt);
                                return new ResultInfo<int, string>(rt, string.Empty);
                            }
                        }
                        else
                        {
                            return new ResultInfo<int, string>(-1, "无变更的数据行");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo<int, string>(-1, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }
        #endregion

        #region private
        private DbParameter CreateParameter(DbProviderParameter dbProviderParameter)
        {
            switch (ProviderType)
            {
                case ProviderType.SqlServer:
                    return new System.Data.SqlClient.SqlParameter()
                    {
                        DbType = dbProviderParameter.DbType,
                        ParameterName = dbProviderParameter.ParameterName,
                        Value = dbProviderParameter.Value,
                        Size = dbProviderParameter.Size,
                        Direction = dbProviderParameter.Direction,
                        SourceColumn = dbProviderParameter.SourceColumn,
                        SourceVersion = dbProviderParameter.SourceVersion,
                        SourceColumnNullMapping = dbProviderParameter.SourceColumnNullMapping
                    };
                case ProviderType.DB2:
                    return new IBM.Data.DB2.DB2Parameter()
                    {
                        DbType = dbProviderParameter.DbType,
                        ParameterName = dbProviderParameter.ParameterName,
                        Value = dbProviderParameter.Value,
                        Size = dbProviderParameter.Size,
                        Direction = dbProviderParameter.Direction,
                        SourceColumn = dbProviderParameter.SourceColumn,
                        SourceVersion = dbProviderParameter.SourceVersion,
                        SourceColumnNullMapping = dbProviderParameter.SourceColumnNullMapping
                    };
                default:
                    return dbProviderParameter;
            }
        }
        #endregion
    }
}
