using CallbackURL.Filters;
using CallbackURL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Net;
using System.Web.Http;

namespace CallbackURL.Controller
{
    public class Callback74ImagesController : ApiController
    {
        public static ArrayList alData = new ArrayList();
        public static readonly object obj = new object();

        // GET: Callback74Images
        [BasicAuthentication]
        [HttpGet]
        public OxyCallbackResponse Get()
        {
            lock (obj)
            {
                OxyCallbackResponse oxy = null;

                if (alData.Count > 0)
                {
                    oxy = (OxyCallbackResponse)alData[0];
                    alData.Remove(oxy);
                }
                return oxy;
            }
        }

        // POST: Callback74Images
        public IHttpActionResult Post([FromBody]JObject model)
        {
            lock (obj)
            {
                var jval = model.ToString();
                OxyCallbackResponse oxy = JsonConvert.DeserializeObject<OxyCallbackResponse>(jval);
                alData.Add(oxy);
            }

            return Ok(HttpStatusCode.OK);
        }


    }
}