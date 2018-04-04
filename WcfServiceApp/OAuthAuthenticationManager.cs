using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WcfServiceApp
{
    public class OAuthAuthorizationManager : ServiceAuthorizationManager
    {
        private const string HttpRequestPropertyName = "httpRequest";
        private const string AuthorizationHeaderName = "Authorization";

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            // Extract the action URI from the OperationContext. Match this against the claims 
            // in the AuthorizationContext. 
            string action = operationContext.RequestContext.RequestMessage.Headers.Action;

            try
            {
                //get the message
                var message = operationContext.RequestContext.RequestMessage;

                //get the http headers
                var httpHeaders = ((HttpRequestMessageProperty)message.Properties[HttpRequestPropertyName]).Headers;

                //get authorization header
                var authHeader = httpHeaders.GetValues(AuthorizationHeaderName)?.SingleOrDefault();
                if (authHeader != null)
                {
                    var parts = authHeader.Split(' ');
                    if (parts[0] == "Bearer")
                    {
                        var userPrincipal = ValidateJwt(parts[1]).Result;
                        if (userPrincipal != null)
                        {
                            //foreach (Claim c in userPrincipal.Claims.Where(c => c.Type == "http://www.contoso.com/claims/allowedoperation"))
                            //{
                            //    var authorized = true;
                            //    //other claims authorization logic etc....
                            //    if (authorized)
                            //    {
                            //        return true;
                            //    }
                            //}
                            Thread.CurrentPrincipal = userPrincipal;
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Validates the JWT token and returns back the user's <see cref="ClaimsPrincipal"/> instance, if valid.
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        private static async Task<ClaimsPrincipal> ValidateJwt(string jwt)
        {
            var endpoint = string.Format(AuthSettings.AadInstance, AuthSettings.Tenant, AuthSettings.DefaultPolicy);
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(endpoint, new OpenIdConnectConfigurationRetriever());
            var config = await configManager.GetConfigurationAsync();
            var handler = new JwtSecurityTokenHandler();
            
            var validationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new[] { AuthSettings.ClientId },                
                IssuerSigningKeys = config.SigningKeys,
                ValidIssuer = config.Issuer,
                
                //CertificateValidator = X509CertificateValidator.None,
                AuthenticationType = AuthSettings.SignUpSignInPolicy,
                RequireExpirationTime = true
            };

            try
            {
                var principal = handler.ValidateToken(jwt, validationParameters, out Microsoft.IdentityModel.Tokens.SecurityToken validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}