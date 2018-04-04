using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace WebApp.Helpers
{
    public class OAuthClientMessageInspector : IClientMessageInspector
    {
        private readonly Func<string> _tokenAccessor;

        public OAuthClientMessageInspector(Func<string> tokenAccessor)
        {
            _tokenAccessor = tokenAccessor;
        }

        public const string AuthorizationHeader = "Authorization";

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequestMessage;
            object httpRequestMessageObject;

            var token = _tokenAccessor();
            var headerValue = $"Bearer {token}";
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                if (string.IsNullOrEmpty(httpRequestMessage.Headers[AuthorizationHeader]))
                {
                    httpRequestMessage.Headers[AuthorizationHeader] = headerValue;
                }
            }
            else
            {
                httpRequestMessage = new HttpRequestMessageProperty();
                httpRequestMessage.Headers.Add(AuthorizationHeader, headerValue);
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
            }

            return null;
        }
    }
}