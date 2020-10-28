using EmotionPlatzi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmotionPlatzi.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var modelo = new Home();
            modelo.WelcomeMessage = "Hello World";
            modelo.FooterMessage = "Footer by mike of the website";
            
            return View(modelo);
        }
    }
}