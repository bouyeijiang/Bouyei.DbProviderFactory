using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace DbMapping
{
    using DbEntities;
    public class UserMapping:EntityTypeConfiguration<User>
    {
        public UserMapping()
        {
            ToTable("user");
        }
    }
}
