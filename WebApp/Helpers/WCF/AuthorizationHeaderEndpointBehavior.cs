using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WebApp.Helpers
{
    public class AuthorizationHeaderEndpointBehavior : IEndpointBehavior
    {
        private Func<string> _tokenAccessor;

        public AuthorizationHeaderEndpointBehavior(Func<string> tokenAccessor)
        {
            _tokenAccessor = tokenAccessor;
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new OAuthClientMessageInspector(_tokenAccessor));
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}