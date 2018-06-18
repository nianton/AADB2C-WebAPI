using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GraphLite;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MigrationFuncApp.Models;

namespace MigrationFuncApp
{
    public static class SignUp
    {
        [FunctionName("SignUp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("SignUp HTTP trigger function processed a request.");

            var model = await req.Content.ReadAsAsync<SignUpModel>();
            var name = model?.SignInName;

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, ErrorResponseModel.Create("Please pass a name in the request body", HttpStatusCode.BadRequest))
                : req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
