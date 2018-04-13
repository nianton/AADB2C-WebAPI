using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApp.Helpers;
using WebApp.Models;
using WebApp.TaskServiceReference;

namespace WebApp.Controllers
{
    [Authorize]
    public class TaskServiceController : Controller
    {
        /// <summary>
        /// Makes a WCF service call to retrieve the todo items for the current user.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            try
            {
                // Retrieve the token with the specified scope
                using (var serviceClient = GetServiceClient())
                {
                    var items = await serviceClient.GetUserTodoItemsAsync();
                    ViewBag.Tasks = items;
                    return View();
                }
            }
            catch (Exception ex)
            {
                return ErrorAction("Error reading to do list: " + ex.Message);
            }
        }

        /// <summary>
        /// Makes a WCF service call to add a new todo item.
        /// </summary>
        /// <param name="id"></param>
        [HttpPost]
        public async Task<ActionResult> Create(string description)
        {
            try
            {
                var todoItem = new TodoItem { Text = description }; 
                using (var serviceClient = GetServiceClient())
                {
                    todoItem = await serviceClient.AddTodoItemAsync(todoItem);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return ErrorAction("Error writing to list: " + ex.Message);
            }
        }

        /// <summary>
        /// Makes a WCF service all to delete request to the Web api to delete a todo item.
        /// </summary>
        /// <param name="id"></param>
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                using (var serviceClient = GetServiceClient())
                {
                    await serviceClient.DeleteTodoItemAsync(id);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return ErrorAction("Error deleting todo item: " + ex.Message);
            }
        }

        /// <summary>
        /// Helper function for returning an error message
        /// </summary>
        private ActionResult ErrorAction(String message)
        {
            return new RedirectResult("/Error?message=" + message);
        }

        /// <summary>
        /// Factory method to create the Wcf Service client applying the default settings.
        /// </summary>
        /// <returns>A new task service client instance.</returns>
        private TaskServiceClient GetServiceClient()
        {
            var serviceClient = new TaskServiceClient();
            serviceClient.Endpoint.EndpointBehaviors.Add(new AuthorizationHeaderEndpointBehavior(GetAccessToken));
            return serviceClient;
        }

        /// <summary>
        /// Method to get back the access token for the current request.
        /// </summary>
        /// <returns>The access token.</returns>
        private string GetAccessToken()
        {
            var accessToken = GetAccessTokenAsync(Startup.ReadTasksScope, Startup.WriteTasksScope).Result;
            return accessToken;
        }

        /// <summary>
        /// Retrieves an access token for the specified scopes.
        /// </summary>
        private async Task<string> GetAccessTokenAsync(params string[] scopes)
        {
            string signedInUserID = User.GetId();
            TokenCache userTokenCache = new MSALSessionCache(signedInUserID, this.HttpContext).GetMsalCacheInstance();
            ConfidentialClientApplication cca = new ConfidentialClientApplication(Startup.ClientId, Startup.Authority, Startup.RedirectUri, new ClientCredential(Startup.ClientSecret), userTokenCache, null);

            var user = cca.Users.FirstOrDefault();
            if (user == null)
            {
                throw new Exception("The User is NULL.  Please clear your cookies and try again.  Specifically delete cookies for 'login.microsoftonline.com'.  See this GitHub issue for more details: https://github.com/Azure-Samples/active-directory-b2c-dotnet-webapp-and-webapi/issues/9");
            }

            AuthenticationResult result = await cca.AcquireTokenSilentAsync(scopes, user, Startup.Authority, false);
            return result.AccessToken;
        }
    }
}