using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bouyei.ProviderFactory;

namespace DbProviderDemo
{
    using Bouyei.ProviderFactory.DbAdoProvider;
    using Bouyei.ProviderFactory.DbEntityProvider;
    using Bouyei.ProviderFactory.Mapper;
    using DbEntities; 

    class Program
    {
        static void Main(string[] args) 
        {
            string connectionString = string.Empty;

            //ado.net demo
            DbLayer dbProvider = DbLayer.CreateDbLayer(connectionString);
            var rt = dbProvider.Query(new DbExecuteParameter()
            {
                CommandText = "select * from user"
            });

            //entity framework demo one:
            DbEFLayer efProvider = DbEFLayer.CreateEFLayer("DbConnection");
            var item = efProvider.Provider.Query<User>(x => x.UserName.Contains("admin")).FirstOrDefault();

            //entity framework demo two:
            IEntityProvider iEfProvider = EntityProvider.CreateProvider();
            var item1 = iEfProvider.Query<User>(x => x.UserName.Contains("admin")).FirstOrDefault();

            //entity mapper
            User u = new User()
            {
                Name = "hello",
                UserName = "bouyei"
            };
            UserDto ud = new UserDto()
            {
                UserName = "http://aileenyin.com/"
            };

            EntityMapper.MapTo<User, UserDto>(u, ud);
        }

        class UserDto
        {
           public string UserName { get; set; }

            public string Pwd { get; set; }
        }
    }
}
