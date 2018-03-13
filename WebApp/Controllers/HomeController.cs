using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        [Authorize]
        public ActionResult Claims()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [Authorize]
        public ActionResult Secure()
        {
            ViewBag.Message = "This is a secure page.";
            return View();
        }

        public ActionResult SinglePageApp()
        {
            ViewBag.Message = "This is a Single Page Application (javascript only).";
            return View();
        }

        public ActionResult Error(string message)
        {
            ViewBag.Message = message;

            return View("Error");
        }
    }
}