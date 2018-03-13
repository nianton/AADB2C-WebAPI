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
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult SocialSignUp(string campaign)
        {
            return View((object)campaign);
        }


        public ActionResult IdpSelector()
        {
            return View();
        }

        public ActionResult MultiFactorAuthentication()
        {
            return View();
        }

        public ActionResult Unified(string campaign)
        {
            return View((object)campaign);
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        public ActionResult UpdateProfile()
        {
            return View();
        }
    }
}