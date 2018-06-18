using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MigrationFuncApp.Models
{
    public class ErrorResponseModel
    {        
        private static readonly string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        [JsonProperty("userMessage")]
        public string UserMessage { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("version")]
        public string Version => AssemblyVersion;

        public static ErrorResponseModel Create(string userErrorMessage, HttpStatusCode statusCode)
        {
            return new ErrorResponseModel
            {
                UserMessage = userErrorMessage,
                Status = (int)statusCode
            };
        }
    }
}
