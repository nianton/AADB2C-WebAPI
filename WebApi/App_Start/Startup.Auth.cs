using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using WebApi.App_Start;

namespace WebApi
{
    public partial class Startup
    {
        // These values are pulled from web.config
        public static string AadInstance = ConfigurationManager.AppSettings["ida:AadInstance"];
        public static string Tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        public static string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static string SignUpSignInPolicy = ConfigurationManager.AppSettings["ida:SignUpSignInPolicyId"];
        public static string DefaultPolicy = SignUpSignInPolicy;
        public const string AadAuthType = "AzureActiveDirectory";

        /// Azure Active Directory settings
        public static string AadTenant = ConfigurationManager.AppSettings["aad:Tenant"];
        public static string AadMetadataInstance = ConfigurationManager.AppSettings["aad:AadInstance"];
        public static string AadClientId = ConfigurationManager.AppSettings["aad:ClientId"];
        public static string AadIssuer = string.Format(ConfigurationManager.AppSettings["aad:AadIssuer"], AadTenant);
        public static string AadAudience = ConfigurationManager.AppSettings["aad:Audience"];

        /*
         * Configure the authorization OWIN middleware 
         */
        public void ConfigureAuth(IAppBuilder app)
        {
            TokenValidationParameters tvps = new TokenValidationParameters
            {
                // Accept only those tokens where the audience of the token is equal to the client ID of this app
                ValidAudience = ClientId,
                AuthenticationType = Startup.DefaultPolicy
            };

            string metadataEndPoint = string.Format(AadInstance, Tenant, DefaultPolicy);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                // This SecurityTokenProvider fetches the Azure AD B2C metadata & signing keys from the OpenIDConnect metadata endpoint
                AccessTokenFormat = new JwtFormat(tvps, new OpenIdConnectCachingSecurityTokenProvider(metadataEndPoint))
            });
        }

        /// <summary>
        /// Configure authentication for AAD and AAD B2C
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureAuthForAAD(IAppBuilder app)
        {
#if DEBUG
            IdentityModelEventSource.ShowPII = true;
#endif

            TokenValidationParameters tvps = new TokenValidationParameters
            {
                // Accept only those tokens where the audience of the token is equal to the client ID of this app
                ValidAudience = AadAudience,
                ValidIssuer = AadIssuer,         
                SignatureValidator = new SignatureValidator((token, validationParameters) =>
                {
                    return new JwtSecurityToken(token);
                })
            };

            string metadataEndPoint = string.Format(AadMetadataInstance, AadTenant);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                // This SecurityTokenProvider fetches the Azure AD metadata & signing keys from the OpenIDConnect metadata endpoint
                AccessTokenFormat = new JwtFormat(tvps, new OpenIdConnectCachingSecurityTokenProvider(metadataEndPoint))                
            });
        }
    }
}
