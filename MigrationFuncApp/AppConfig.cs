using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationFuncApp
{
    public class AppConfig
    {
        static AppConfig()
        {
            Tenant = Environment.GetEnvironmentVariable("tenant");
            ApplicationId = Environment.GetEnvironmentVariable("appId");
            ApplicationSecret = Environment.GetEnvironmentVariable("appSecret");
        }

        public static string Tenant { get; }
        public static string ApplicationId { get; }
        public static string ApplicationSecret { get; }
    }
}
