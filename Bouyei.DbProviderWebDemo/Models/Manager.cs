using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bouyei.ProviderFactory;

namespace Bouyei.DbProviderWebDemo
{
    public class Manager
    {
        public static ILayerOrm dbProvider { get; set; }
    }
}