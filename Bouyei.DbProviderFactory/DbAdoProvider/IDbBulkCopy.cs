/*-------------------------------------------------------------
 *auth:bouyei
 *date:2016/7/12 12:00:15
 *contact:qq453840293
 *machinename:BOUYEI-PC
 *company/organization:Microsoft
 *profile:www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Text;
using System.Data;

namespace Bouyei.DbProviderFactory.DbAdoProvider
{
    interface IDbBulkCopy
    {
        void WriteToServer(DataTable dataTable);

        void WriteToServer(DataTable dataTable,DataRowState state);

        //void WriteToServer(IDataReader dataReader);

        //void WriteToServer(DataRow[] dataRow);
    }
}
