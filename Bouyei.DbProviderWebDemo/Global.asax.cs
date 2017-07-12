using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Bouyei.DbProviderWebDemo
{
    using Bouyei.ProviderFactory;
    using DbEntities;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            LayerOrm orm = LayerOrm.CreateLayerOrm();
            bool rtb = orm.NoTrackQuery<User>(x => x.Id == 1).Any();
        }
    }
}
