/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/11/22 16:35:29
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: 85661d58-c751-4319-b34b-90c396695f4b
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.ComponentModel;

namespace Bouyei.DbProviderFactory.UtilIO
{
    public static class DataTableHelper
    {
        #region public
        public static T DataReaderTo<T>(this IDataReader dataReader, bool IgnoreCase = false) where T : new()
        {
            return DbReflection.GetGenericObjectValue<T>((System.Data.Common.DbDataReader)dataReader);
        }

        public static List<T> DataReaderToList<T>(this IDataReader dataReader, bool IgnoreCase = false) where T : new()
        {
            return DbReflection.GetGenericObjectValues<T>((System.Data.Common.DbDataReader)dataReader, IgnoreCase);
        }

        /// <summary>
        /// 根据结构体创建DataTable数据集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DataTable ConvertTo<T>()
        {
            PropertyDescriptorCollection properties = null;
            DataTable table = CreateTable<T>(out properties);

            return table;
        }

        /// <summary>
        /// 只复制List集合到DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int CopyTo<T>(ref DataTable table, IList<T> list)
        {
            Type type = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);

            //添加数据行
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item);

                table.Rows.Add(row);
            }
            return list.Count;
        }

        /// <summary>
        /// 创建并复制表到DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ConvertTo<T>(this IList<T> list)
        {
            PropertyDescriptorCollection properties = null;
            DataTable table = CreateTable<T>(out properties);

            //添加数据行
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item);

                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// DataTable数据转换到List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ConvertTo<T>(this DataTable table)
        {
            List<T> list = new List<T>(table.Rows.Count);
            Type type = typeof(T);

            //换行列
            List<PropertyInfo> properties = new List<PropertyInfo>(table.Columns.Count);
            foreach (DataColumn column in table.Columns)
            {
                PropertyInfo pinfo = type.GetProperty(column.ColumnName);
                properties.Add(pinfo);
            }

            //转换行
            foreach (DataRow dr in table.Rows)
            {
                T value = DataRowTo<T>(dr, properties);
                list.Add(value);
            }

            return list;
        }

        #endregion

        #region private

        private static DataTable CreateTable<T>(out PropertyDescriptorCollection properties)
        {
            Type type = typeof(T);
            DataTable table = new DataTable(type.Name);
            properties = TypeDescriptor.GetProperties(type);

            foreach (PropertyDescriptor propertie in properties)
                table.Columns.Add(propertie.Name, propertie.PropertyType);

            return table;
        }

        private static T DataRowTo<T>(DataRow dr, List<PropertyInfo> properties)
        {
            T obj = default(T);
            obj = Activator.CreateInstance<T>();
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    object value = dr[property.Name];
                    if (value.GetType() == typeof(DBNull)) value = null;

                    property.SetValue(obj, value, null);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return obj;
        }

        #endregion
    }
}
