using EmotionPlatzi.Web.Data;
using EmotionPlatzi.Web.Util;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
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
    public class FaceUploaderController : Controller
    {
        string serveFolderPath;
        FaceHelper faceHelper;
        string key, endpoint;
        EmotionPlatziWebContext db = new EmotionPlatziWebContext();
        IList<FaceAttributeType?> faceAttributes;
        public FaceUploaderController()
        {
            serveFolderPath = ConfigurationManager.AppSettings["UPLOAD_DIR"];
            key = ConfigurationManager.AppSettings["Key_Azure"];
            endpoint = ConfigurationManager.AppSettings["End_point"];
            faceHelper = new FaceHelper(key,endpoint );
            faceAttributes = new FaceAttributeType?[]
                {
                    FaceAttributeType.Emotion
                };
        }
        // GET: EmoUploader
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(HttpPostedFileBase file)
        {
            if (file?.ContentLength > 0)
            {
                var picturename = Guid.NewGuid().ToString();
                picturename += Path.GetExtension(file.FileName);
                var route = Server.MapPath(serveFolderPath);
                route = $"{route}/{picturename}";
                file.SaveAs(route);

                var emoPicture = await faceHelper.DetectedFaceAndExtractFace(file.InputStream, faceAttributes);

                emoPicture.Nombre = file.FileName;
                emoPicture.Path = $"{serveFolderPath}/{picturename}";

                db.EmoPictures.Add(emoPicture);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "EmoPictures", new { Id = emoPicture.Id });

            }

            return View();
        }
    }
}