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

        public ActionResult DeletePhoto(string image, string thumbnail)
        {
            System.IO.File.Delete(image);
            System.IO.File.Delete(thumbnail);
            return RedirectToAction("Index");
        }
    }
}