/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/7/11 9:17:06
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: Microsoft
 *   guid: 6ceef553-44aa-427b-8bbb-b592657843da
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbProviderFactory.UtilIO
{
    internal static class DbReflection
    {
        public static T GetGenericObjectValue<T>(this DbDataReader reader, bool ignoreCase = false) where T : new()
        {
            T value = new T();

            PropertyInfo[] pinfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (ignoreCase == false)
            {
                foreach (var pi in pinfos)
                {
                    object attrValue = reader[pi.Name];
                    if (attrValue == null || attrValue == DBNull.Value) continue;

                    pi.SetValue(value, attrValue, null);
                }
            }
            else
            {
                foreach (var pi in pinfos)
                {
                    string fieldColName = pi.Name.ToLower();

                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        string dbColName = reader.GetName(i);
                        if (dbColName.ToLower() == fieldColName)
                        {
                            object attrValue = reader.GetValue(i);

                            if (attrValue == null || attrValue == DBNull.Value)
                                continue;

                            pi.SetValue(value, attrValue, null);
                            break;
                        }
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// 根据DbDataReader映射到结构体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> GetGenericObjectValues<T>(this DbDataReader  reader, bool ignoreCase = false) where T : new()
        {
            List<T> items = new List<T>(64);
            PropertyInfo[] pinfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            while (reader.Read())
            {
                T value = new T();
                if (ignoreCase == false)
                {
                    foreach (var pi in pinfos)
                    {
                        object attrValue = reader[pi.Name];
                        if (attrValue == null || attrValue == DBNull.Value) continue;

                        pi.SetValue(value, attrValue, null);
                    }
                }
                else
                {
                    foreach (var pi in pinfos)
                    {
                        string fieldColName = pi.Name.ToLower();

                        for (int i = 0; i < reader.FieldCount; ++i)
                        {
                            string dbColName = reader.GetName(i);
                            if (dbColName.ToLower() == fieldColName)
                            {
                                object attrValue = reader.GetValue(i);

                                if (attrValue == null || attrValue == DBNull.Value)
                                    continue;

                                pi.SetValue(value, attrValue, null);
                                break;
                            }
                        }
                    }
                }
                items.Add(value);
            }
            return items;
        }
    }
}
