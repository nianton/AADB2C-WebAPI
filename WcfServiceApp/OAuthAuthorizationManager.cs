using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace WcfServiceApp
{
    /// <summary>
    /// 
    /// </summary>
    public class OAuthAuthorizationManager : ServiceAuthorizationManager
    {
        private const string HttpRequestPropertyName = "httpRequest";
        private const string AuthorizationHeaderName = "Authorization";
        private const string BearerTokenPrefix = "Bearer";
        private const string AuthContextPrincipalPropertyName = "Principal";

        private readonly IDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>> _configurationManagersIndex;

        public OAuthAuthorizationManager()
        {
            // Build B2C configuration manager
            var b2cEndpoint = string.Format(AuthSettings.AadInstance, AuthSettings.Tenant, AuthSettings.DefaultPolicy);
            var b2cConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(b2cEndpoint, new OpenIdConnectConfigurationRetriever());
            var b2cConfig = b2cConfigurationManager.GetConfigurationAsync().Result;

            // Build AAD configuration manager
            var aadEndpoint = string.Format(AuthSettings.AadMetadataInstance, AuthSettings.AadTenant);
            var aadConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(aadEndpoint, new OpenIdConnectConfigurationRetriever());
            var aadConfig = aadConfigurationManager.GetConfigurationAsync().Result;

            // Register configuration managers based on the issued
            _configurationManagersIndex = new Dictionary<string, ConfigurationManager<OpenIdConnectConfiguration>>();
            _configurationManagersIndex.Add(b2cConfig.Issuer, b2cConfigurationManager);
            _configurationManagersIndex.Add(aadConfig.Issuer, aadConfigurationManager);
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            // Extract the action URI from the OperationContext. Match this against the claims 
            // in the AuthorizationContext. 
            string action = operationContext.RequestContext.RequestMessage.Headers.Action;

            try
            {
                // Get the message
                var message = operationContext.RequestContext.RequestMessage;

                // Retrieve Http headers from the message
                var httpHeaders = ((HttpRequestMessageProperty)message.Properties[HttpRequestPropertyName]).Headers;

                // Get authorization header token value
                var authHeader = httpHeaders.GetValues(AuthorizationHeaderName)?.SingleOrDefault();
                if (authHeader != null)
                {
                    var parts = authHeader.Split(' ');
                    if (parts.Length == 2 && string.Equals(parts[0], BearerTokenPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        var jwtToken = parts[1];

                        // Validate JWT token and get the user principal
                        var userPrincipal = ValidateJwtAsync(jwtToken).Result;
                        if (userPrincipal != null)
                        {
                            // Injecting the principal in the operation context to be available as Thread.CurrentPrincipal in the service instance
                            operationContext.ServiceSecurityContext.AuthorizationContext.Properties[AuthContextPrincipalPropertyName] = userPrincipal;
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

        private ConfigurationManager<OpenIdConnectConfiguration> GetConfigurationManager(string issuer)
        {
            if (_configurationManagersIndex.TryGetValue(issuer, out var configurationManager))
            {
                return configurationManager;
            }

            return null;
        }

        private async Task<TokenValidationParameters> GetValidationParametersAsync(string issuer)
        {
            var configurationManager = GetConfigurationManager(issuer);
            if (configurationManager == null)
            {
                return null;
            }

            var isAad = issuer.StartsWith("https://sts.windows.net");
            var config = await configurationManager.GetConfigurationAsync();

            return new TokenValidationParameters()
            {
                ValidAudience = isAad ?  AuthSettings.AadAudience : AuthSettings.ClientId,
                IssuerSigningKeys = config.SigningKeys.OfType<RsaSecurityKey>(),
                ValidIssuer = config.Issuer,
                AuthenticationType = isAad ? AuthSettings.AadAuthType : AuthSettings.SignUpSignInPolicy,
                RequireExpirationTime = true
            };
        }

        /// <summary>
        /// Validates the JWT token and returns back the user's <see cref="ClaimsPrincipal"/> instance, if valid.
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        private async Task<ClaimsPrincipal> ValidateJwtAsync(string jwt)
        {
            // TODO: Remove in production, helps with exception messages during debugging
            IdentityModelEventSource.ShowPII = true;

            // Read Jwt token and create validation parameters based on issuer
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(jwt);

            var validationParameters = await GetValidationParametersAsync(jwtToken.Issuer);
            if (validationParameters == null)
            {
                return null;
            }

            try
            {
                var principal = handler.ValidateToken(jwt, validationParameters, out SecurityToken validatedToken);
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