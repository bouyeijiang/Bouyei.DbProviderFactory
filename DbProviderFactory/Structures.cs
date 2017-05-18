using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Data.Odbc;
using System.Data.OleDb;
//using System.Data.commandite;
using System.Data.SqlClient;

namespace Bouyei.ProviderFactory
{
    /// <summary>
    /// 通道类型
    /// </summary>
    public enum ChannelType : int
    {
        /// <summary>
        /// Tcp通道
        /// </summary>
        Tcp,
        /// <summary>
        /// Http通道
        /// </summary>
        Http,
        /// <summary>
        /// Ipc通道
        /// </summary>
        Ipc
    }

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

    [Serializable]
    public enum DbInterfaceType
    {
        commandclient,
        Odbc,
        Oledb,
        commandite
    }

    [Serializable]
    public class DbSerializeParameter
    {
        private int psize = 4;

        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 参数大小,默认4位
        /// </summary>
        public int ParamSize {
            get { return psize; }
            set { psize = value; }
        }

        /// <summary>
        /// 参数类型
        /// </summary>
        public DbType ParamType { get; set; }

        /// <summary>
        /// 查询参数方向为输入或输出
        /// </summary>
        public ParameterDirection ParamDirection { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public object ParamValue { get; set; }

        public DbSerializeParameter()
        { }

        public DbSerializeParameter(SqlParameter sparam)
        {
            this.ParamName = sparam.ParameterName;
            this.psize = sparam.Size;
            this.ParamType = (DbType)sparam.SqlDbType;
            this.ParamDirection = sparam.Direction;
            this.ParamValue = sparam.Value;
        }

        public DbSerializeParameter(OdbcParameter sparam)
        {
            this.ParamName = sparam.ParameterName;
            this.psize = sparam.Size;
            this.ParamType = (DbType)sparam.OdbcType;
            this.ParamDirection = sparam.Direction;
            this.ParamValue = sparam.Value;
        }

        public DbSerializeParameter(OleDbParameter sparam)
        {
            this.ParamName = sparam.ParameterName;
            this.psize = sparam.Size;
            this.ParamType = (DbType)sparam.OleDbType;
            this.ParamDirection = sparam.Direction;
            this.ParamValue = sparam.Value;
        }

        //public DbParameter(commanditeParameter sparam)
        //{
        //    this.ParamName = sparam.ParameterName;
        //    this.psize = sparam.Size;
        //    this.ParamType = sparam.DbType;
        //    this.ParamDirection = sparam.Direction;
        //    this.ParamValue = sparam.Value;
        //}

        public SqlParameter TocommandParameter()
        {
            return new SqlParameter()
            {
                ParameterName = ParamName,
                Size = psize,
                SqlDbType = (SqlDbType)ParamType,
                Direction = ParamDirection,
                Value = ParamValue
            };
        }

        public OdbcParameter ToOdbcParameter()
        {
            return new OdbcParameter()
            {
                ParameterName = ParamName,
                Size = psize,
                OdbcType = (OdbcType)ParamType,
                Direction = ParamDirection,
                Value = ParamValue
            };
        }

        public OleDbParameter ToOledbParameter()
        {
            return new  OleDbParameter()
            {
                ParameterName = ParamName,
                Size = psize,
                OleDbType = (OleDbType)ParamType,
                Direction = ParamDirection,
                Value = ParamValue
            };
        }

        //public commanditeParameter TocommanditeParameter()
        //{
        //    return new commanditeParameter()
        //    {
        //        ParameterName = ParamName,
        //        Size = psize,
        //        DbType = ParamType,
        //        Direction = ParamDirection,
        //        Value = ParamValue
        //    };
        //}
    }

    [Serializable]
    public class CommandParamsInfo
    {
        /// <summary>
        /// command语句
        /// </summary>
        public string dbcommand { get; set; }

        /// <summary>
        /// command对应的参数
        /// </summary>
        public DbSerializeParameter[] dbParams { get; set; }
    }
}
