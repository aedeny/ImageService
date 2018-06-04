using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private Home _home;

        // GET: Home
        public ActionResult Index()
        {
            _home = new Home();
            return View(_home);
        }
    }
}