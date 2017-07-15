using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Bouyei.ProviderFactory;

namespace DbProviderDemo
{
    using Bouyei.ProviderFactory.DbAdoProvider;
    using Bouyei.ProviderFactory.DbEntityProvider;
    using Bouyei.ProviderFactory.DbSqlProvider;
    using Bouyei.ProviderFactory.DbSqlProvider.Extensions;
    using Bouyei.ProviderFactory.DbMapper;
    using Bouyei.DbEntities; 

   public class info
    {
        public string name { get; set; }

        public string realname { get; set; }
    }

    class Program
    {
        static void Main(string[] args) 
        {
            var sql = SqlProvider.Singleton.Select("username", "realname", "age")
                .From("sys_user").Where(new KeyValue()
                {
                    Name = "username",
                    Value = "bouyei"
                }).SqlString;

            //string connectionString = string.Empty;

            ////ado.net demo
            //LayerAdo dbProvider = LayerAdo.CreateLayerAdo(connectionString);
            //var rt = dbProvider.Query(new DbExecuteParameter()
            //{
            //    CommandText = "select * from user"
            //});

            //entity framework demo one:
            ILayerOrm efProvider = LayerOrm.CreateLayerOrm("DbConnection");
            try
            {
                //entity framework demo two:
                User item = efProvider.GetById<User>(1);
                UserDto ud = new UserDto()
                {
                    UserName = "http://aileenyin.com/"
                };

                var query = efProvider.Query<User>().FirstOrDefault();

                EntityMapper.MapTo<UserDto, User>(ud, item);
                efProvider.Update(item);
                int rt = efProvider.SaveChanges();
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
