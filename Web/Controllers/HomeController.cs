using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Config()
        {
            ViewBag.Message = "This is the configuration page.";
            return View();
        }

        public ActionResult Photos()
        {
            ViewBag.Message = "All your selfies and other photos are here.";

            return View();
        }

        public ActionResult Logs()
        {
            ViewBag.Message = "Useless log entries that no one cares about.";

            return View();
        }
    }
}