using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CallbackURL.Filters
{
    public class BasicAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            try
            {
                if (filterContext.Request.Headers.Authorization == null)
                {
                    filterContext.Response = new System.Net.Http.HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Content = new StringContent("{\"error\":\"invalid_client\"}", Encoding.UTF8, "application/json")
                    };
                    filterContext.Response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", "realm=xxxx"));
                }
                else if (filterContext.Request.Headers.Authorization.Scheme != "Basic" ||
                    string.IsNullOrEmpty(filterContext.Request.Headers.Authorization.Parameter))
                {

                    filterContext.Response = new System.Net.Http.HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent("{\"error\":\"invalid_request\"}", Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var authToken = filterContext.Request.Headers.Authorization.Parameter;
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(authToken));

                    int seperatorIndex = usernamePassword.IndexOf(':');
                    string clientId = usernamePassword.Substring(0, seperatorIndex);
                    string clientSecret = usernamePassword.Substring(seperatorIndex + 1);
                    if (!ValidateApiKey(clientId, clientSecret))
                    {
                        filterContext.Response = new System.Net.Http.HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            Content = new StringContent("{\"error\":\"invalid_client\"}", Encoding.UTF8, "application/json")
                        };
                    }                    
                }
            }
            catch (Exception ex)
            {
                filterContext.Response = new System.Net.Http.HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("{\"error\":\"invalid_request\"}", Encoding.UTF8, "application/json")
                };
            }
        }

        private bool ValidateApiKey(string clientId, string clientSecret)
        {
            if (true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}