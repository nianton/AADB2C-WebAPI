using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WcfServiceApp
{
    public class AuthSettings
    {
        // These values are pulled from web.config
        public static string AadInstance = ConfigurationManager.AppSettings["ida:AadInstance"];
        public static string Tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        public static string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static string SignUpSignInPolicy = ConfigurationManager.AppSettings["ida:SignUpSignInPolicyId"];
        public static string DefaultPolicy = SignUpSignInPolicy;

        public static string AadTenant = ConfigurationManager.AppSettings["aad:Tenant"];
        public static string AadMetadataInstance = ConfigurationManager.AppSettings["aad:AadInstance"];
        public static string AadAudience = ConfigurationManager.AppSettings["aad:Audience"];
        public const string AadAuthType = "AzureActiveDirectory";
    }
}