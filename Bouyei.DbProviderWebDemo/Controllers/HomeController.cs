using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bouyei.DbEntities;
using Bouyei.ProviderFactory;

namespace Bouyei.DbProviderWebDemo.Controllers
{
    public class HomeController : Controller
    {
        private ILayerOrm dbProvider = null;
        public HomeController()
        {
            dbProvider = Manager.dbProvider;
        }

        public ActionResult Index()
        {
            bool rtb = dbProvider.NoTrackQuery<User>(x => x.Id == 1).Any();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}