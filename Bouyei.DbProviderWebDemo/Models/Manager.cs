using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bouyei.DbProviderFactory;

namespace Bouyei.DbProviderWebDemo
{
    public class Manager
    {
        public static IOrmProvider dbProvider { get; set; }
    }
}