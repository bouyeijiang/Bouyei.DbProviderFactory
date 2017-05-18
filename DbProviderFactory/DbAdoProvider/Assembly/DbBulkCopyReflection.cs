/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/1/13 9:47:29
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: ea7f489d-a3a0-4eeb-996e-2af5efa5a860
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

namespace Bouyei.ProviderFactory.DbAdoProvider
{
    public class DbBulkCopyReflection:IDisposable
    {
        private Assembly assembly { get; set; }

        Type bulkCopyType { get; set; }

        object bulkCopyObject { get; set; }

        Type mappingCollectionType { get; set; }

        object mappingCollectionObject { get; set; }

        MethodInfo addColumnNameMethod { get; set; }

        MethodInfo WriteServerMethod { get; set; }

        MethodInfo WriteServerParamMethod { get; set; }

        MethodInfo mappingClearMethd { get; set; }

        public int BulkCopyTimeout { get; set; }

        public DbBulkCopyProvider BulkCopyProvider { get; private set; }

         ~DbBulkCopyReflection()
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
                Close();

                if (bulkCopyObject != null)
                {
                    if (BulkCopyProvider.DbProviderType == ProviderType.DB2)
                    {
                        bulkCopyType.InvokeMember("Dispose", BindingFlags.InvokeMethod, null, bulkCopyObject, null);
                    }
                }
            }
        }

        public DbBulkCopyReflection(DbBulkCopyProvider bulkCopyProvider,
            string ConnectionStrinng,
            int BulkCopyTimeout = 1800)
        {
            if (assembly != null) return;

            this.BulkCopyProvider = bulkCopyProvider;
            this.BulkCopyTimeout = BulkCopyTimeout;
            if (bulkCopyProvider.Path == null) bulkCopyProvider.Path = Environment.CurrentDirectory+"\\";
            assembly = Assembly.LoadFile(bulkCopyProvider.Path + bulkCopyProvider.DllName);

            bulkCopyType = assembly.GetType(bulkCopyProvider.BulkCopyClassName);
            bulkCopyObject = Activator.CreateInstance(bulkCopyType, new object[] { ConnectionStrinng });

            mappingCollectionType = assembly.GetType(bulkCopyProvider.BulkCopyColumnMappingCollection);
            mappingCollectionObject = bulkCopyType.GetProperty("ColumnMappings").GetValue(bulkCopyObject, null);
        }

        private void InitAssemblyProperty()
        {
            if (addColumnNameMethod == null)
                addColumnNameMethod = mappingCollectionType.GetMethod("Add", new Type[] { typeof(string), typeof(string) });

            if (mappingClearMethd == null)
                mappingClearMethd = mappingCollectionType.GetMethod("Clear");

            if (WriteServerMethod == null)
                WriteServerMethod = bulkCopyType.GetMethod("WriteToServer", new Type[] { typeof(DataTable) });

            if (WriteServerParamMethod == null)
                WriteServerParamMethod = bulkCopyType.GetMethod("WriteToServer", new Type[] { typeof(DataTable), typeof(DataRowState) });

            //设置超时
            PropertyInfo bulkCopyTimeout = bulkCopyType.GetProperty("BulkCopyTimeout");
            bulkCopyTimeout.SetValue(bulkCopyObject, BulkCopyTimeout, null);
        }

        public void Close()
        {
            if (bulkCopyObject != null)
            {
                if (bulkCopyType != null)
                {
                    bulkCopyType.InvokeMember("Close", BindingFlags.InvokeMethod, null, bulkCopyObject, null);
                }
            }
        }

        public void WriteToServer(DataTable dt)
        {
            InitAssemblyProperty();

            //清空集合
            mappingClearMethd.Invoke(mappingCollectionObject, null);

            //设置表名
            PropertyInfo bulkCopyTableName = bulkCopyType.GetProperty("DestinationTableName");
            bulkCopyTableName.SetValue(bulkCopyObject, dt.TableName, null);

            //映射列名
            foreach (DataColumn column in dt.Columns)
                addColumnNameMethod.Invoke(mappingCollectionObject, new string[] { column.ColumnName, column.ColumnName });
            try
            {
                WriteServerMethod.Invoke(bulkCopyObject, new object[] { dt });
            }
            catch(Exception ex)
            {
                Close();
                throw ex;
            }
        }

        public void WriteToServer(DataTable dt, DataRowState state)
        {
            InitAssemblyProperty();

            //清空集合
            mappingClearMethd.Invoke(mappingCollectionObject, null);

            //设置表名
            PropertyInfo bulkCopyTableName = bulkCopyType.GetProperty("DestinationTableName");
            bulkCopyTableName.SetValue(bulkCopyObject, dt.TableName, null);

            //映射列名
            foreach (DataColumn column in dt.Columns)
                addColumnNameMethod.Invoke(mappingCollectionObject, new string[] { column.ColumnName, column.ColumnName });

            try
            {
                WriteServerMethod.Invoke(bulkCopyObject, new object[] { dt, state });
            }
            catch (Exception ex)
            {
                Close();
                throw ex;
            }
        }
    }
}
