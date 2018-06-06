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

        public ActionResult Delete(string dirHandlerToDelete)
        {
            // string command = (int)CommandEnum.CloseDirectoryHandlerCommand + "|" + dirHandlerToDelete;
            // GuiTcpClientSingleton.Instance.Writer.Write(command);
            return RedirectToAction("Index");
        }
    }
}