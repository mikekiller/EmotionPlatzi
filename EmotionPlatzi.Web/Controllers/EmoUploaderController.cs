using EmotionPlatzi.Web.Data;
using EmotionPlatzi.Web.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EmotionPlatzi.Web.Controllers
{
    public class EmoUploaderController : Controller
    {
        string serveFolderPath;
        EmotionHelper emoHelper;
        string key;
        EmotionPlatziWebContext db = new EmotionPlatziWebContext();
        public EmoUploaderController()
        {
            serveFolderPath = ConfigurationManager.AppSettings["UPLOAD_DIR"];
            key = ConfigurationManager.AppSettings["Key_Azure"];
            emoHelper = new EmotionHelper(key);
        }
        // GET: EmoUploader
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task< ActionResult> Index(HttpPostedFileBase file )
        {
            if (file?.ContentLength>0)
            {
                var picturename = Guid.NewGuid().ToString();
                picturename += Path.GetExtension(file.FileName);
                var route = Server.MapPath(serveFolderPath);
                route = $"{route}/{picturename}";
                file.SaveAs(route);

                var emoPicture  = await emoHelper.DetectandExtracFacesAsync(file.InputStream);

                emoPicture.Nombre = file.FileName;
                emoPicture.Path = $"{serveFolderPath}/{picturename}";

                db.EmoPictures.Add(emoPicture);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "EmoPictureController", new { Id = emoPicture.Id});

            }

            return View();
        }
    }
}