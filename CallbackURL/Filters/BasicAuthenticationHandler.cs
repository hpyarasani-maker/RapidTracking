using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace CallbackURL.Filters
{
    public class BasicAuthenticationHandler : DelegatingHandler
    {
        private const string WWWAuthenticateHeader = "WWW-Authenticate";
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)

        {
            var credentials = ParseAuthorizationHeader(request);//ParseAuthorizationHeader

            if (credentials != null)
            {
                var identity = new BasicAuthenticationIdentity(credentials.Name, credentials.Password);
                var principal = new GenericPrincipal(identity, null);

                Thread.CurrentPrincipal = principal;
                
            }
            
           
            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var response = task.Result;
                    if (credentials == null && response.StatusCode == HttpStatusCode.Unauthorized)
                      Challenge(request, response);
                    
                    JObject job = new JObject(
             new JProperty("meta", new JObject(
            new JProperty("status", 401), new JProperty("errors", new JObject(new JProperty("code", "101"), new JProperty("context", "UnAuthorized"), new JProperty("message", "Unable To Authenticate"))))));
                     string json = JsonConvert.SerializeObject(job, Formatting.Indented);
                    var unserializedContent = JsonConvert.DeserializeXmlNode(json);
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
                    if (credentials != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    { 
                        response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Content = new StringContent(json)
                    };
                    }
                    return response;
                });
            
            
        }
        
        /// <summary>
        /// Parses the Authorization header and creates user credentials
        /// </summary>
        /// <param name="actionContext"></param>
        protected virtual BasicAuthenticationIdentity ParseAuthorizationHeader(HttpRequestMessage request)
        {
            string authHeader = null;
            var auth = request.Headers.Authorization;
            if (auth != null && auth.Scheme == "Basic")
                authHeader = auth.Parameter;

            if (string.IsNullOrEmpty(authHeader))
                return null;

            authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

            var tokens = authHeader.Split(':');
            if (tokens.Length < 2)
                return null;

            return new BasicAuthenticationIdentity(tokens[0], tokens[1]);
        }
        /// <summary>
        /// Send the Authentication Challenge request
        /// </summary>
        /// <param name="message"></param>
        /// <param name="actionContext"></param>
       public void Challenge(HttpRequestMessage request, HttpResponseMessage response)
        {
            var host = request.RequestUri.DnsSafeHost;
            response.Headers.Add(WWWAuthenticateHeader, string.Format("Basic realm=\"{0}\"", host));
            HttpStatusCode status = response.StatusCode;
            
            var credentials = ParseAuthorizationHeader(request);
            if (credentials != null&&status == HttpStatusCode.Unauthorized)
            {

                response = response.RequestMessage.CreateResponse(HttpStatusCode.Unauthorized, "project not found");
                
            }
        }

        
    }
}