using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class PhotosController : Controller
    {
        private Photos _photos;

        public ActionResult Index()
        {
            _photos = new Photos();
            return View(_photos);
        }
    }
}