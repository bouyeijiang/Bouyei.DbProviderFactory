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
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Bouyei.DbProviderFactory.DbAdoProvider
{
    using UtilIO;
    /// <summary>
    /// 连接池
    /// </summary>
    public class DbProvider : DbCommonBuilder, IDbProvider
    {
        #region variable
        private int signal = 0;
        private bool disposed = false;
        private int lockInterval = 1000;

        public string DbConnectionString { get; set; }
        
        public ProviderType DbType { get; set; }

        private int timeoutLock = 5000;//millseconds
        public int TimeoutLock
        {
            get { return timeoutLock; }
            set
            {
                timeoutLock = value < 5000 ? 5000 : timeoutLock;
            }
        }

        #endregion

        #region  dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (this.dbConn != null) this.dbConn.Dispose();
                if (this.dbDataAdapter != null) this.dbDataAdapter.Dispose();
                if (this.dbCommand != null) this.dbCommand.Dispose();
                if (this.dbBulkCopy != null) this.dbBulkCopy.Dispose();
                if (this.dbCommandBuilder != null) dbCommandBuilder.Dispose();
                if (this.dbTransaction != null) dbTransaction.Dispose();
            }
            disposed = true;
        }

        #endregion

        #region  structure
        public DbProvider(
            string connectionString,
            ProviderType providerType = ProviderType.SqlServer,
            bool isSingleton = false)
            : base(providerType, isSingleton)
        {
            this.DbType = providerType;
            this.DbConnectionString = connectionString;
        }

        public DbProvider(
            ProviderType providerType = ProviderType.SqlServer,
            bool isSingleton = false)
            : base(providerType, isSingleton)
        {
            this.DbType = providerType;
        }

        public static DbProvider CreateProvider(string connectionString,
            ProviderType providerType=ProviderType.SqlServer)
        {
            return new DbProvider(connectionString, providerType);
        }

        public static DbProvider CreateProvider(
            ConnectionConfiguraton connectionConfiguration)
        {
            return new DbProvider(connectionConfiguration.ToString(),
                connectionConfiguration.DbType);
        }

        #endregion

        #region public
        public ResultInfo<bool, string> Connect(string ConnectionString)
        {
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock))
                {
                    return ResultInfo<bool,string>.Create(false, "等待锁超时...");
                }
                _times += lockInterval;
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
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<DataTable, string>.Create(null, "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    DataTable dt = new DataTable();

                    using (DbDataAdapter adapter = this.CreateAdapter())
                    {
                        using (DbCommand cmd = this.CreateCommand(dbExecuteParameter, conn))
                        {
                            adapter.SelectCommand = cmd;
                            adapter.Fill(dt);
                        }
                    }
                    return new ResultInfo<DataTable, string>(dt, string.Empty);
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
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<DataSet, string>.Create(null, "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    DataSet ds = new DataSet();

                    using (DbDataAdapter adapter = CreateAdapter())
                    {
                        using (DbCommand cmd = CreateCommand(dbExecuteParameter, conn))
                        {
                            adapter.SelectCommand = cmd;
                            adapter.Fill(ds);
                        }
                    }
                    return ResultInfo<DataSet, string>.Create(ds, string.Empty);
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

        public ResultInfo<int, string> QueryToReader(DbExecuteParameter dbExecuteParameter, Func<IDataReader,bool> rowAction)
        {
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if(_times>= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<int, string>.Create(-1, "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                int rows = 0;
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbCommand cmd = CreateCommand(dbExecuteParameter, conn))
                    {
                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == false)
                                return ResultInfo<int, string>.Create(0, string.Empty);
                            bool isContinue = false;

                            while (reader.Read())
                            {
                                isContinue = rowAction(reader);
                                if (isContinue == false) break;
                                ++rows;
                            }
                        }
                    }
                }

                return ResultInfo<int, string>.Create(rows, string.Empty);
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

        public ResultInfo<int,string> QueryTo<T>(DbExecuteParameter dbExecuteParameter,Func<T,bool> rowAction)
             where T:new()
        {
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<int, string>.Create(-1, "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                int rows = 0;
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbCommand cmd = CreateCommand(dbExecuteParameter, conn))
                    {
                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == false)
                                return ResultInfo<int, string>.Create(0, string.Empty);

                            bool isContinue = false;
                            while (reader.Read())
                            {
                                T row = reader.DataReaderTo<T>(dbExecuteParameter.IgnoreCase);
                                isContinue = rowAction(row);
                                if (isContinue == false) break;
                                ++rows;
                            }
                        }
                    }
                }

                return ResultInfo<int, string>.Create(rows, string.Empty);
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
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<IDataReader, string>.Create(null, "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                DbConnection conn = CreateConnection(DbConnectionString);
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    DbCommand cmd = CreateCommand(dbExecuteParameter, conn);
                    {
                        IDataReader reader = cmd.ExecuteReader();
                        return ResultInfo<IDataReader, string>.Create(reader, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                return   ResultInfo<IDataReader, string>.Create(null, ex.Message);
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<int, string> ExecuteCmd(DbExecuteParameter dbExecuteParameter)
        {
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<int, string>.Create(-1, "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (DbCommand cmd = CreateCommand(dbExecuteParameter, conn))
                    {
                        int rt = cmd.ExecuteNonQuery();

                        var rValue = GetReturnParameter(cmd);

                        return   ResultInfo<int, string>.Create(rt < 0 ? 0 : rt,
                            rValue == null ? string.Empty : rValue.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return   ResultInfo<int, string>.Create(-1, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<int, string> QueryToTable(DbExecuteParameter dbExecuteParameter, DataTable dstTable)
        {
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<int, string>.Create(-1, "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbCommand cmd = CreateCommand(dbExecuteParameter, conn ))
                    {
                        using (DbDataReader dReader = cmd.ExecuteReader())
                        {
                            int oCnt = dstTable.Rows.Count;

                            dstTable.Load(dReader);

                            return   ResultInfo<int, string>.Create(dstTable.Rows.Count - oCnt, string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return   ResultInfo<int, string>.Create(-1, ex.ToString());
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
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<int, string>.Create(-1, "等待锁超时...");
                }
                _times += lockInterval;
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
                            using (DbCommand cmd = CreateCommand(dbExecuteParameter, conn, tran))
                            {
                                int rt = cmd.ExecuteNonQuery();
                                tran.Commit();

                                var rValue = GetReturnParameter(cmd);

                                return   ResultInfo<int, string>.Create(rt < 0 ? 0 : rt,
                                    rValue != null ? rValue.ToString() : string.Empty);
                            }
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            return   ResultInfo<int, string>.Create(-1, ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return   ResultInfo<int, string>.Create(-1, ex.ToString());
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
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + timeout))
                {
                    return ResultInfo<int, string>.Create(-1, "等待锁超时...");
                }
                _times += lockInterval;
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
                                using (DbCommand cmd = CreateCommand(new DbExecuteParameter()
                                {
                                    CommandText = CommandTexts[i],
                                    ExectueTimeout = timeout
                                }, conn, tran))
                                {
                                    rows += cmd.ExecuteNonQuery();
                                }
                            }
                            tran.Commit();

                            return new ResultInfo<int, string>(rows < 0 ? 0 : rows, string.Empty);
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
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<T, string>.Create(default(T), "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbCommand cmd = CreateCommand(dbExecuteParameter, conn))
                    {
                        object obj = cmd.ExecuteScalar();

                        var rValue = GetReturnParameter(cmd);

                        return   ResultInfo<T, string>.Create(obj == null ? default(T) : (T)obj,
                          rValue == null ? string.Empty : rValue.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return ResultInfo<T, string>.Create(default(T), ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<int, string> BulkCopy(DbExecuteBulkParameter dbExecuteParameter)
        {
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<int, string>.Create(-1, "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                Exception temex = null;
                int cnt = 0;
                using (DbCommonBulkCopy bulkCopy = CreateBulkCopy(DbConnectionString, dbExecuteParameter.IsTransaction))
                {
                    bulkCopy.Open();
                    bulkCopy.BulkCopiedHandler = dbExecuteParameter.BulkCopiedHandler;

                    bulkCopy.BatchSize = dbExecuteParameter.BatchSize;
                    bulkCopy.BulkCopyTimeout = dbExecuteParameter.ExectueTimeout;

                    try
                    {
                        if ((dbExecuteParameter.DstDataTable == null 
                            || dbExecuteParameter.DstDataTable.Rows.Count == 0)
                            && dbExecuteParameter.IDataReader != null)
                        {
                            bulkCopy.WriteToServer(dbExecuteParameter.IDataReader, dbExecuteParameter.DstTableName);
                            cnt = 1;
                        }
                        else
                        {
                            if (dbExecuteParameter.BatchSize > dbExecuteParameter.DstDataTable.Rows.Count)
                                dbExecuteParameter.BatchSize = dbExecuteParameter.DstDataTable.Rows.Count;

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

                return   ResultInfo<int, string>.Create(cnt, string.Empty);
            }
            catch (Exception ex)
            {
                return   ResultInfo<int, string>.Create(-1, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        public ResultInfo<List<T>, string> Query<T>(DbExecuteParameter dbExecuteParameter) where T : new()
        {
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<List<T>, string>.Create(null, "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbCommand cmd = CreateCommand(dbExecuteParameter, conn))
                    {
                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == false)
                                return ResultInfo<List<T>, string>.Create(new List<T>(1), string.Empty);

                            var items = reader.GetGenericObjectValues<T>(dbExecuteParameter.IgnoreCase);

                            return ResultInfo<List<T>, string>.Create(items, string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ResultInfo<List<T>, string>.Create(null, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }

        /// <summary>
        /// Fun<DataTable,bool>回调方法返回false则中断执行直接返回
        /// </summary>
        /// <param name="dbExecuteParameter"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ResultInfo<int, string> QueryChanged(DbExecuteParameter dbExecuteParameter, Func<DataTable,bool> action)
        {
            int _times = 0;
            while (Interlocked.CompareExchange(ref signal, 1, 0) == 1)
            {
                Thread.Sleep(lockInterval);

                //waiting for lock to do;
                if (_times >= (timeoutLock + dbExecuteParameter.ExectueTimeout))
                {
                    return ResultInfo<int, string>.Create(-1, "等待锁超时...");
                }
                _times += lockInterval;
            }
            try
            {
                using (DbConnection conn = CreateConnection(DbConnectionString))
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    using (DbDataAdapter adapter = this.CreateAdapter())
                    {
                        using (DbCommand cmd = this.CreateCommand(dbExecuteParameter, conn))
                        {
                            DataTable dt = new DataTable();
                            adapter.SelectCommand = cmd;
                            adapter.Fill(dt);

                            if (dt.Rows.Count == 0) return ResultInfo<int, string>.Create(-1, "无可更新的数据行");

                            bool isContinue = action(dt);
                            if (isContinue == false) return ResultInfo<int, string>.Create(0, string.Empty);

                            DataTable changedt = dt.GetChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
                            if (changedt != null && changedt.Rows.Count > 0)
                            {
                                using (DbCommandBuilder dbBuilder = this.CreateCommandBuilder())
                                {
                                    dbBuilder.DataAdapter = adapter;
                                    int rt = adapter.Update(changedt);
                                    return ResultInfo<int, string>.Create(rt, string.Empty);
                                }
                            }
                            else
                            {
                                return ResultInfo<int, string>.Create(0, string.Empty);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ResultInfo<int, string>.Create(-1, ex.ToString());
            }
            finally
            {
                Interlocked.Exchange(ref signal, 0);
            }
        }
        #endregion

        #region private

        private object GetReturnParameter(DbCommand cmd)
        {
            if (cmd.Parameters != null && cmd.Parameters.Count > 0)
            {
                for(int i = 0; i < cmd.Parameters.Count; ++i)
                {
                    if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                    {
                        return cmd.Parameters[i].Value;
                    }
                }
            }

            return null;
        }
        #endregion
    }
}
