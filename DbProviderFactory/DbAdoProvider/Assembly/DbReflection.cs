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

namespace Bouyei.ProviderFactory.DbAdoProvider
{
    internal static class DbReflection
    {
        /// <summary>
        /// 根据DbDataReader映射到结构体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T GetGenericObjectValue<T>(this IDataReader reader) where T : new()
        {
            T value = new T();

            PropertyInfo[] pinfos=typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var pi in pinfos)
            {
                object attrValue = reader[pi.Name];
                if (attrValue == null || attrValue == DBNull.Value) continue;

                var mi = pi.GetSetMethod();
                if (mi == null) continue;

                pi.SetValue(value, attrValue, null);
            }

            return value;
        }

        /// <summary>
        /// 根据DbDataReader映射到结构体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> GetGenericObjectValues<T>(this IDataReader reader)where T:new()
        {
            List<T> items = new List<T>(64);
            PropertyInfo[] pinfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            while (reader.Read())
            {
                T value = new T();

                foreach (var pi in pinfos)
                {
                    object attrValue = reader[pi.Name];
                    if (attrValue == null || attrValue == DBNull.Value) continue;

                    //var mi = pi.GetSetMethod();
                    //if (mi == null) continue;
        
                    pi.SetValue(value, attrValue, null);
                }

                items.Add(value);
            }
            return items;
        }
    }
}
