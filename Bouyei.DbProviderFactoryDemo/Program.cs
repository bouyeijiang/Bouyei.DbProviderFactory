﻿using System;
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
            //string connectionString = string.Empty;

            ////ado.net demo
            //DbLayer dbProvider = DbLayer.CreateDbLayer(connectionString);
            //var rt = dbProvider.Query(new DbExecuteParameter()
            //{
            //    CommandText = "select * from user"
            //});

            //entity framework demo one:
            DbEFLayer efProvider = DbEFLayer.CreateEFLayer("DbConnection");
            try
            {
                bool rtb = efProvider.Provider.NoTrackQuery<User>(x => x.Id == 1).Any();
                User item = efProvider.Provider.GetById<User>(1);

                ////entity framework demo two:
                //IEntityProvider iEfProvider = EntityProvider.CreateProvider();
                // var item1 = iEfProvider.Query<User>(x => x.UserName.Contains("admin")).FirstOrDefault();

                //entity mapper
                User u = new User()
                {
                    Id=3,
                    Name = "sdfasdf",
                    UserName = "bouyei"
                };

                //efProvider.Provider.Insert(u);
                //int rt = efProvider.SaveChanges();

                UserDto ud = new UserDto()
                {
                    UserName = "http://aileenyin.com/111"
                };

                //EntityMapper.MapTo<UserDto, User>(ud, item);
                efProvider.Provider.Update(u);
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