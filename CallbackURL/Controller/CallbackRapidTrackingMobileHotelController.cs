using CallbackURL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Net;
using System.Web.Http;

namespace CallbackURL.Controller
{
    public class CallbackRapidTrackingMobileHotelController : ApiController
    {        
        public static ArrayList alData = new ArrayList();
        public static readonly object obj = new object();


        // GET: api/CallbackRapidTrackingMobile
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


        // POST: api/CallbackRapidTrackingMobile
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
