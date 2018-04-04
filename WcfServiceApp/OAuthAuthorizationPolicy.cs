using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Web;

namespace WcfServiceApp
{
    public class OAuthAuthorizationPolicy : IAuthorizationPolicy
    {
        private readonly Guid id = Guid.NewGuid();

        public ClaimSet Issuer => ClaimSet.System;

        public string Id => id.ToString();

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            evaluationContext.Properties["Principal"] = null;            
            return true;
        }
    }
}