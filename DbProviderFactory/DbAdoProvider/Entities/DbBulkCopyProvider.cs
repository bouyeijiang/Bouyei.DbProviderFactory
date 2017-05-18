/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/1/19 15:27:55
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: 4692b015-aa90-45cb-b1c8-99f633ab44b1
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bouyei.ProviderFactory.DbAdoProvider
{
    public class DbBulkCopyProvider
    {
        public string Path { get; set; }
        public string DllName { get; private set; }

        public string Namespace { get; private set; }

        public string BulkCopyClassName { get; private set; }

        public string BulkCopyColumnMappingCollection { get; private set; }

        public ProviderType DbProviderType { get; private set; }

        public DbBulkCopyProvider(ProviderType DbProviderType)
        {
            this.DbProviderType = DbProviderType;
            switch (DbProviderType)
            {
                case ProviderType.DB2:
                    DllName = "IBM.Data.DB2.dll";
                    Namespace = "IBM.Data.DB2";
                    BulkCopyClassName = "IBM.Data.DB2.DB2BulkCopy";
                    BulkCopyColumnMappingCollection = "IBM.Data.DB2.DB2BulkCopyColumnMappingCollection";
                    break;
                case ProviderType.Oracle:
                    DllName = "Oracle.DataAccess.dll";
                    Namespace = "Oracle.DataAccess.Client";
                    BulkCopyClassName = "Oracle.DataAccess.Client.OracleBulkCopy";
                    BulkCopyColumnMappingCollection = "Oracle.DataAccess.Client.OracleBulkCopyColumnMappingCollection";
                    break;
                case ProviderType.MySql:
                    break;
            }
        }
    }
}
