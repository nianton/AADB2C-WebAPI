using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Extensions;

namespace WebApp.Controllers
{
    /// <summary>
    /// This controller delivers the custom UI for the needs of AD B2C. The custom UI pages can be simple static Html pages,
    /// or have server logic during the generation of the wrapping content. 
    /// NOTE: The server hosting the custom UI should allow CORS requests on the resources required.
    /// </summary>
    [AllowCors]
    public class AdB2CController : Controller
    {
        public ActionResult SignInSignUp()
        {
            ViewBag.Title = "Sign in or sign up";
            return View();
        }

        public ActionResult SignIn()
        {
            ViewBag.Title = "Sign In";
            return View();
        }

        public ActionResult SignUp()
        {
            ViewBag.Title = "Sign Up";
            return View();
        }

        public ActionResult SocialSignUp(string campaign)
        {
            ViewBag.Title = "Social Sign Up";
            return View((object)campaign);
        }


        public ActionResult IdpSelector()
        {
            ViewBag.Title = "Identity provider selector";
            return View();
        }

        public ActionResult MultiFactorAuthentication()
        {
            ViewBag.Title = "Two Factor Authentication";
            return View();
        }

        public ActionResult Unified(string campaign)
        {
            ViewBag.Title = "Unified UI";
            return View((object)campaign);
        }

        public ActionResult ResetPassword()
        {
            ViewBag.Title = "Reset Password";
            return View();
        }

        public ActionResult UpdateProfile()
        {
            ViewBag.Title = "Update your profile";
            return View();
        }
    }
}