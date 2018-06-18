using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationFuncApp.Models
{
    public class SignInModel
    {
        [JsonProperty("signInName")]
        public string SignInName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
