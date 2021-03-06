﻿/*-------------------------------------------------------------
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
            Type toType = typeof(T);
            PropertyInfo[] pinfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var pi in pinfos)
            {
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    if (NameEqual(pi.Name, reader.GetName(i), ignoreCase))
                    {
                        object attrValue = reader.GetValue(i);

                        if (attrValue == null || attrValue == DBNull.Value)
                            continue;

                        //转换类型
                        var dstPro = toType.GetProperty(pi.Name);
                        var dstType = dstPro.PropertyType;

                        object dstValue = null;

                        if (dstType.IsGenericType && dstType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            dstValue = Convert.ChangeType(attrValue, dstType.GetGenericArguments()[0]);
                        }
                        else if (dstType.IsEnum)
                        {
                            dstValue = Enum.ToObject(dstType, attrValue);
                        }
                        else
                        {
                            dstValue = Convert.ChangeType(attrValue, dstType);
                        }
                        pi.SetValue(value, dstValue, null);
                        break;
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
        public static List<T> GetGenericObjectValues<T>(this DbDataReader reader, bool ignoreCase = false) where T : new()
        {
            List<T> items = new List<T>(16);
            PropertyInfo[] pinfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Type toType = typeof(T);

            while (reader.Read())
            {
                T value = new T();

                foreach (var pi in pinfos)
                {
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        if (NameEqual(pi.Name, reader.GetName(i), ignoreCase))
                        {
                            object attrValue = reader.GetValue(i);

                            if (attrValue == null || attrValue == DBNull.Value)
                                continue;

                            //转换类型
                            var dstPro = toType.GetProperty(pi.Name);
                            var dstType = dstPro.PropertyType;

                            object dstValue = null;

                            if (dstType.IsGenericType && dstType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                dstValue = Convert.ChangeType(attrValue, dstType.GetGenericArguments()[0]);
                            }
                            else if (dstType.IsEnum)
                            {
                                dstValue = Enum.ToObject(dstType, attrValue);
                            }
                            else
                            {
                                dstValue = Convert.ChangeType(attrValue, dstType);
                            }
                            pi.SetValue(value, dstValue, null);
                            break;
                        }
                    }
                }
                items.Add(value);
            }
            return items;
        }

        private static bool NameEqual(string srcName, string dstName, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return srcName.ToLower().Equals(dstName.ToLower());
            }
            else
            {
                return srcName.Equals(dstName);
            }
        }
    }
}
