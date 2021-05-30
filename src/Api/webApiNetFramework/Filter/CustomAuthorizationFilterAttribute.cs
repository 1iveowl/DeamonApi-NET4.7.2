using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace webApiNetFramework.Filter
{
    public class CustomAuthorizationFilterAttribute : AuthorizationFilterAttribute
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {            
            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            var claim = principal?.Claims?.FirstOrDefault(c => c?.Type?.ToLower() == ClaimType?.ToLower());

            if (claim != null && claim?.Value?.ToLower() == ClaimValue?.ToLower())
            {
                return;
            }

            actionContext.Response = actionContext.Request
                    .CreateResponse(
                        HttpStatusCode.Unauthorized,
                        $"Unauthorized! Claim: {ClaimType}={ClaimValue} not found. " +
                        $"Claim found: {claim?.Type ?? "<null>"}={claim?.Value ?? "null"}");

            await Task.CompletedTask;
        }
    }
}