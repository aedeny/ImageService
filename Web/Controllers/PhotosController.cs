using System;
using System.Diagnostics;
using System.Web.Mvc;
using Communication;
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
            try
            {
                System.IO.File.Delete(image);
            }
            catch (Exception e)
            {
                GuiTcpClientSingleton.Instance.Log("Cannot delete " + image, EventLogEntryType.Warning);
                Console.WriteLine(e);
            }

            try
            {
                System.IO.File.Delete(thumbnail);
            }
            catch (Exception e)
            {
                GuiTcpClientSingleton.Instance.Log("Cannot delete " + thumbnail, EventLogEntryType.Warning);
                Console.WriteLine(e);
            }

            return RedirectToAction("Index");
        }
    }
}