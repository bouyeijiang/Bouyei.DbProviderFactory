using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using Bouyei.DbProviderFactory;
using System.Configuration;

namespace DbProviderDemo
{
    using Bouyei.DbProviderFactory.DbAdoProvider;
    using Bouyei.DbProviderFactory.DbSqlProvider;
    using Bouyei.DbProviderFactory.DbSqlProvider.Extensions;
    using Bouyei.DbProviderFactory.DbMapper;
    using Bouyei.DbEntities; 

    class Program
    {
        static void Main(string[] args) 
        {
            //生成简单查询脚本
            var sql = SqlProvider.Singleton.Select("username", "realname", "age")
                .From("sys_user").Where(new KeyValue()
                {
                    Name = "username",
                    Value = "bouyei"
                }).SqlString;

            //结果:Select username,realname,age From sys_user Where username='bouyei' 

            ////ado.net 使用例子
            string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            AdoProvider dbProvider = AdoProvider.CreateAdoProvider(connectionString,ProviderType.SqlServer);
            var adort = dbProvider.Query(new DbExecuteParameter()
            {
                CommandText = "select * from [user]"
            });


            DataTable dt = new DataTable();
            var qrt= dbProvider.QueryToTable(new DbExecuteParameter("select * from [user]"), dt);

            //entity framework 使用例子
            IOrmProvider ormProvider = OrmProvider.CreateOrmProvider("DbConnection");
            try
            {
                User item = ormProvider.GetById<User>(1);
                UserDto ud = new UserDto()
                {
                    UserName = "http://aileenyin.com/"
                };

                var query = ormProvider.Query<User>().FirstOrDefault();

                //使用mapper修改对象
                EntityMapper.MapTo<UserDto, User>(ud, item);
                ormProvider.Update(item);
                //保存修改
                int rt = ormProvider.SaveChanges();
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        class UserDto
        {
           public string UserName { get; set; }

            public string Pwd { get; set; }
        }
    }
}
