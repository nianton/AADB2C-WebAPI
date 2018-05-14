using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApp.Helpers
{
    public interface IAuthService
    {
        Task<AuthResult> AuthorizeAsync(string username, string password, IEnumerable<string> scopes);
        Task<AuthResult> AuthorizeViaRefreshTokenAsync(string refreshToken);
    }

    public class AuthService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _tokenEndpoint;
        private readonly string _clientId;

        public AuthService(string tokenEndpoint, string clientId)
        {
            _tokenEndpoint = tokenEndpoint;
            _clientId = clientId;
        }


        public async Task<AuthResult> AuthorizeAsync(string username, string password, IEnumerable<string> scopes)
        {
            // Appending 'offline_access' to get back a refresh token as well.
            var scopeEntries = new HashSet<string>(scopes) { "offline_access" };

            var contentParameters = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "grant_type", "password" },
                { "scope", string.Join(" ", scopeEntries) },
                { "username", username },
                { "password", password }
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
                    var authTokenResponse = JsonConvert.DeserializeObject<AuthTokenResponse>(responseString);
                    return AuthResult.Success(authTokenResponse);
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorInfo>(responseString);
                    errorResponse.Status = response.StatusCode;
                    return AuthResult.Failed(errorResponse);
                }
            }
            catch (Exception ex)
            {
                return AuthResult.Failed(new ErrorInfo()
                {
                    Error = ex.Message,
                    Description = ex.ToString()
                });
            }
        }

        public async Task<AuthResult> AuthorizeViaRefreshTokenAsync(string refreshToken)
        {
            var contentParameters = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "grant_type", "refresh_token" },
                { "response_type", "id_token" },
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
                    var authTokenResponse = JsonConvert.DeserializeObject<RefreshTokenResponse>(responseString);
                    return AuthResult.Success(authTokenResponse);
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorInfo>(responseString);
                    errorResponse.Status = response.StatusCode;
                    return AuthResult.Failed(errorResponse);
                }
            }
            catch (Exception ex)
            {
                return AuthResult.Failed(new ErrorInfo()
                {
                    Error = ex.Message,
                    Description = ex.ToString()
                });
            }
        }
    }

    public class AuthTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }

    public class RefreshTokenResponse : AuthTokenResponse
    {
        [JsonProperty("not_before")]
        public int NotBefore { get; set; }
        [JsonProperty("refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }
    }


    public class AuthResult
    {
        private AuthResult() { }

        public bool IsSuccessful => Error == null;

        public ErrorInfo Error { get; private set; }

        public AuthTokenResponse TokenResponse { get; private set; }

        public static AuthResult Success(AuthTokenResponse tokenResponse)
        {
            return new AuthResult() { TokenResponse = tokenResponse };
        }

        public static AuthResult Failed(ErrorInfo error)
        {
            return new AuthResult() { Error = error };
        }
    }

    public class ErrorInfo
    {
        public HttpStatusCode? Status { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string Description { get; set; }
    }
}