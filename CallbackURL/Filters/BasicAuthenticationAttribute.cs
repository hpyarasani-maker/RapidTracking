using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CallbackURL.Filters
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public int status;


        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                var authHeader = actionContext.Request.Headers.Authorization;

                if (authHeader != null)
                {
                    var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                    var decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                    var usernamePasswordArray = decodedAuthenticationToken.Split(':');
                    var userName = usernamePasswordArray[0];
                    var password = usernamePasswordArray[1];

                    var isValid = userName == "pisoftware" && password == "r00t123456";


                    bool authenticated;
                    
                    if (isValid)
                    {
                        var principal = new GenericPrincipal(new GenericIdentity(userName), null);
                        Thread.CurrentPrincipal = principal;
                        authenticated = Thread.CurrentPrincipal.Identity.IsAuthenticated;
                        return;
                    }

                    else
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                        {
                            Error = new JProperty("meta", new JObject(
                                                   new JProperty("status", 401), new JProperty("errors", new JObject(new JProperty("Code", "101"), new JProperty("Context", "UnAuthorized"), new JProperty("Message", "Unable To Authenticate")))))
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                throw new System.Web.Http.HttpResponseException(actionContext.Response);
            }
            HandleUnathorized(actionContext);
        }


        private static void HandleUnathorized(HttpActionContext actionContext)
        {
            try
            {

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new { Error = true, Message = "Token is invalid" });

                actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='Data' location = 'http://localhost:");
            }
            catch (Exception ex)
            {
                throw new System.Web.Http.HttpResponseException(actionContext.Response);
            }
        }
    }
}