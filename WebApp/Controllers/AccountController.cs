using Microsoft.Owin.Security;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Helpers;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        /*
         *  Called when requesting to sign up or sign in
         */
        public void SignUpSignIn(string loginHint)
        {
            // Use the default policy to process the sign up / sign in flow
            if (!Request.IsAuthenticated)
            {
                var authenticationProperties = new AuthenticationProperties() { RedirectUri = "/" };
                if (!string.IsNullOrWhiteSpace(loginHint))
                {
                    authenticationProperties.Dictionary[AdB2CProperties.LoginHint] = loginHint;
                }

                HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);
                return;
            }

            Response.Redirect("/");
        }

        /*
         *  Called when requesting to edit a profile
         */
        public void EditProfile()
        {
            if (Request.IsAuthenticated)
            {
                // Let the middleware know you are trying to use the edit profile policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
                HttpContext.GetOwinContext().Set("Policy", Startup.EditProfilePolicyId);

                // Set the page to redirect to after editing the profile
                var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
                HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

                return;
            }

            Response.Redirect("/");
        }

        /*
         *  Called when requesting to reset a password
         */
        public void ResetPassword()
        {
            // Let the middleware know you are trying to use the reset password policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
            HttpContext.GetOwinContext().Set("Policy", Startup.ResetPasswordPolicyId);

            // Set the page to redirect to after changing passwords
            var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
            HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

            return;
        }

        /*
         *  Called when requesting to sign out
         */
        public void SignOut()
        {
            // To sign out the user, you should issue an OpenIDConnect sign out request.
            if (Request.IsAuthenticated)
            {
                IEnumerable<AuthenticationDescription> authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
                Request.GetOwinContext().Authentication.GetAuthenticationTypes();
            }
        }

        #region [ Reserved For Hybrid Authentication ]

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult AdLogin(string userName)
        {
            var model = new LoginModel() { Username = userName };
            return View(model);
        }

        [HttpPost]
        public ActionResult AdLogin(LoginModel model)
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            AdAuthenticationService authService = new AdAuthenticationService(authenticationManager);

            var authenticationResult = authService.SignIn(model.Username, model.Password);
            if (authenticationResult.IsSuccess)
            {
                // we are in!
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", authenticationResult.ErrorMessage);
            return View(model);
        }

        public ActionResult RopcLogin(string userName)
        {
            var model = new RopcLoginModel() { Username = userName };
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> RopcLogin(RopcLoginModel model)
        {
            var authenticationService = new AuthService(Startup.TokenEndpoint, Startup.ClientId);
            var authenticationResult = await authenticationService.AuthorizeAsync(model.Username, model.Password, Startup.Scopes);
            model.Result = authenticationResult;
            return View(model);
        }

        #endregion
    }
}