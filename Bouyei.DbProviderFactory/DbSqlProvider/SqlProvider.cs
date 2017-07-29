/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:23:45
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: 4fb595c5-2190-4efa-93c2-5f79e573042f
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.ProviderFactory.DbSqlProvider
{
    using Expression;

    public class SqlProvider
    {
        public SqlProvider()
        {
        }

        public static SqlProvider Create()
        {
            return new SqlProvider();
        }

        class instance
        {
            internal static SqlProvider provider = new SqlProvider();
            static instance()
            { }
        }

        public static SqlProvider Singleton
        {
            get { return instance.provider; }
        }

        public Select Select(params string[] columns)
        {
            Select _select = new Select(columns);
            _select.SqlString = _select.ToString();
            return _select;
        }

        public Select<T> Select<T>() where T : class
        {
            Select<T> _select = new Select<T>();
            _select.SqlString = _select.ToString();
            return _select;
        }

        public Insert InsertInto(string tableName, string[] Columns, string[,] Values)
        {
            Insert insert = new Insert(tableName, Columns, Values);
            insert.SqlString = insert.ToString();

            return insert;
        }

        public Insert<T> InsertInto<T>(string tableName, T Value) where T : class
        {
            Insert<T> insert = new Insert<T>(Value);
            insert.SqlString = insert.ToString();

            return insert;
        }

        public Insert<T> InsertInto<T>(string tableName, T[] Values) where T : class
        {
            Insert<T> insert = new Insert<T>(Values);
            insert.SqlString = insert.ToString();

            return insert;
        }

        public Update Update(string tableName)
        {
            Update up = new Update(tableName);
            up.SqlString = up.ToString();
            return up;
        }

        public Update<T> Update<T>()
        {
            Update<T> up = new Expression.Update<T>();
            up.SqlString = up.ToString();

            return up;
        }

        public Delete<T> Delete<T>() where T : class
        {
            Delete<T> delete = new Expression.Delete<T>();
            delete.SqlString = delete.ToString();

            return delete;
        }

        public Delete Delete()
        {
            Delete delete = new Expression.Delete();
            delete.SqlString = delete.ToString();

            return delete;
        }
    }

    public class SqlProvider<T> where T : class
    {
        public SqlProvider()
        {
        }

        public static SqlProvider<T> Create()
        {
            return new SqlProvider<T>();
        }

        public Select Select(params string[] columns)
        {
            Select _select = new Select(columns);
            _select.SqlString = _select.ToString();
            return _select;
        }
    }
}
