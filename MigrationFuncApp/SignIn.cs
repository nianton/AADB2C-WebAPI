using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MigrationFuncApp.Models;

namespace MigrationFuncApp
{
    public static class SignIn
    {
        [FunctionName("SignIn")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequestMessage req, 
            TraceWriter log)
        {
            log.Info("SignIn HTTP trigger function processed a request.");

            var ctx = (HttpContextWrapper)req.Properties["MS_HttpContext"];
            var callerAddress = ctx.Request.UserHostAddress;
            log.Info($"Received request from: {callerAddress}");
            
            var model = await req.Content.ReadAsAsync<SignInModel>();            
            log.Info($"SigninName: {model?.SignInName}, Password: {model?.Password}");

            var name = model?.SignInName;

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, ErrorResponseModel.Create("Please pass the 'signInName' claim in the request body", HttpStatusCode.BadRequest))
                : req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
