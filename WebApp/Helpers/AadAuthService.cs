using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApp.Helpers
{
    public interface IAADAuthService
    {
        Task<AadAuthResult> AuthorizeAsync(string username, string password, IEnumerable<string> scopes);
        Task<AadAuthResult> AuthorizeViaRefreshTokenAsync(string refreshToken);
    }

    public class AADAuthService : IAADAuthService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _tokenEndpoint;
        private readonly string _clientId;
        private readonly string _resource;

        public AADAuthService(string tokenEndpoint, string clientId, string resource)
        {
            _tokenEndpoint = tokenEndpoint;
            _clientId = clientId;
            _resource = resource;
        }

        public async Task<AadAuthResult> AuthorizeAsync(string username, string password, IEnumerable<string> scopes)
        {
            // Appending 'offline_access' to get back a refresh token as well.
            var scopeEntries = new HashSet<string>(scopes) { "offline_access" };

            var contentParameters = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "grant_type", "password" },
                { "scope", string.Join(" ", scopeEntries) },
                { "username", username },
                { "password", password },
                { "resource", _resource }
            };

            var content = new FormUrlEncodedContent(contentParameters);
            var request = new HttpRequestMessage(HttpMethod.Post, _tokenEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = content;

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var authTokenResponse = JsonConvert.DeserializeObject<AadAuthTokenResponse>(responseString);
                    return AadAuthResult.Success(authTokenResponse);
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorInfo>(responseString);
                    errorResponse.Status = response.StatusCode;
                    return AadAuthResult.Failed(errorResponse);
                }
            }
            catch (Exception ex)
            {
                return AadAuthResult.Failed(new ErrorInfo()
                {
                    Error = ex.Message,
                    Description = ex.ToString()
                });
            }
        }

        public async Task<AadAuthResult> AuthorizeViaRefreshTokenAsync(string refreshToken)
        {
            var contentParameters = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var content = new FormUrlEncodedContent(contentParameters);
            var request = new HttpRequestMessage(HttpMethod.Post, _tokenEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = content;

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var authTokenResponse = JsonConvert.DeserializeObject<AadAuthTokenResponse>(responseString);
                    return AadAuthResult.Success(authTokenResponse);
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorInfo>(responseString);
                    errorResponse.Status = response.StatusCode;
                    return AadAuthResult.Failed(errorResponse);
                }
            }
            catch (Exception ex)
            {
                return AadAuthResult.Failed(new ErrorInfo()
                {
                    Error = ex.Message,
                    Description = ex.ToString()
                });
            }
        }
    }

    public class AadAuthTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("not_before")]
        public int NotBefore { get; set; }
        [JsonProperty("refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }
    }


    public class AadAuthResult
    {
        private AadAuthResult() { }

        public bool IsSuccessful => Error == null;

        public ErrorInfo Error { get; private set; }

        public AadAuthTokenResponse TokenResponse { get; private set; }

        public static AadAuthResult Success(AadAuthTokenResponse tokenResponse)
        {
            return new AadAuthResult() { TokenResponse = tokenResponse };
        }

        public static AadAuthResult Failed(ErrorInfo error)
        {
            return new AadAuthResult() { Error = error };
        }
    }
}