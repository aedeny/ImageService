using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class LogsController : Controller
    {
        private Logs _logs;
        // GET: Logs
        public ActionResult Index()
        {
            _logs = new Logs();
            return View(_logs);
        }
    }
}