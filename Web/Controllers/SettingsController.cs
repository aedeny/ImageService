using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class SettingsController : Controller
    {

        // GET: Settings
        public ActionResult Index()
        {
            return View(new Settings());
        }
    }
}