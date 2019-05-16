using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApp.Helpers;

namespace WebApp.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private String apiEndpoint = Startup.ServiceUrl.TrimEnd('/') + "/api/tasks/";

        /// <summary>
        /// Makes a GET call to get the todo items for the current user.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            ViewBag.ApiEndpoint = apiEndpoint;

            try
            {
                // Retrieve the token with the specified scope
                var accessToken = await GetAccessToken(Startup.ReadTasksScope);

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

                // Add token to the Authorization header and make the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        String responseString = await response.Content.ReadAsStringAsync();
                        JArray tasks = JArray.Parse(responseString);
                        ViewBag.Tasks = tasks;
                        return View();
                    case HttpStatusCode.Unauthorized:
                        return ErrorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return ErrorAction("Error reading to do list: " + ex.Message);
            }
        }

        /// <summary>
        /// Makes a POST request to the Web api to delete a todo item.
        /// </summary>
        /// <param name="id"></param>
        [HttpPost]
        public async Task<ActionResult> Create(string description)
        {
            try
            {
                var accessToken = await GetAccessToken(Startup.WriteTasksScope);

                // Set the content
                var httpContent = new[] { new KeyValuePair<string, string>("Text", description) };

                // Create the request
                HttpClient client = new HttpClient();
                HttpContent content = new FormUrlEncodedContent(httpContent);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = content;
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.Created:
                        return new RedirectResult("/Tasks");
                    case HttpStatusCode.Unauthorized:
                        return ErrorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return ErrorAction("Error writing to list: " + ex.Message);
            }
        }


        /// <summary>
        /// Makes a DELETE request to the Web api to delete a todo item.
        /// </summary>
        /// <param name="id"></param>
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var accessToken = await GetAccessToken(Startup.WriteTasksScope);

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, apiEndpoint + id);

                // Add token to the Authorization header and send the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                        return new RedirectResult("/Tasks");
                    case HttpStatusCode.Unauthorized:
                        return ErrorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return ErrorAction("Error deleting from list: " + ex.Message);
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
        /// Retrieves an access token for the specified scopes.
        /// </summary>
        private async Task<string> GetAccessToken(params string[] scopes)
        {
            string signedInUserID = User.GetId();

            // MSAL v3 confidential application creation
            IConfidentialClientApplication cca = ConfidentialClientApplicationBuilder.Create(Startup.ClientId)
                .WithB2CAuthority(Startup.Authority)
                .WithClientSecret(Startup.ClientSecret)
                .WithRedirectUri(Startup.RedirectUri)
                .Build();

            // Enable encrypted persistence to local file for tokens
            TokenCacheHelper.EnableSerialization(cca.UserTokenCache);
            IEnumerable<IAccount> accounts = await cca.GetAccountsAsync();
            IAccount account = accounts.FirstOrDefault(acc => acc.HomeAccountId.Identifier.StartsWith(signedInUserID));
            if (account == null)
            {
                throw new Exception("The account is NULL.  Please clear your cookies and try again.  Specifically delete cookies for 'login.microsoftonline.com'.  See this GitHub issue for more details: https://github.com/Azure-Samples/active-directory-b2c-dotnet-webapp-and-webapi/issues/9");
            }

            AuthenticationResult result = await cca.AcquireTokenSilent(Startup.Scopes, account)
                .WithB2CAuthority(Startup.Authority)
                .ExecuteAsync();

            return result.AccessToken;
        }
    }
}