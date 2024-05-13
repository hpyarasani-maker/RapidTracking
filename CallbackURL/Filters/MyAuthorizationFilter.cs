using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace CallbackURL.Filters
{
    public class MyAuthorizationFilter : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var identity = Thread.CurrentPrincipal.Identity;
            if (identity == null && HttpContext.Current != null)
                identity = HttpContext.Current.User.Identity;

            if (identity != null && identity.IsAuthenticated)
            {
                var basicAuth = identity as BasicAuthenticationIdentity;

                var user = new CallbackURL.Filters.ApiSecurity();
                
                if (user.Authenticate(basicAuth.Name, basicAuth.Password))
                    return true;
            }

            return false;
        }
    }
}