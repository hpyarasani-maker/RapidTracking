using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace RapidTrackingResSingleThread
{
    public class Client : WebClient
    {
        public static string username = "respidatametrics";
        public static string password = "SeEx6#^dwuu#6";
        public string session_id = new Random().Next().ToString();

        public Client(string country_iso = null)
        {
            this.Proxy = new WebProxy("pr.oxylabs.io:7777");
            var login = $"customer-" + username + (country_iso != null ? "-cc-" + country_iso : "")
                + "-sessid-" + session_id;
            this.Proxy.Credentials = new NetworkCredential(login, password);
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            request.ConnectionGroupName = session_id;
            //request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36";
            //request.Method = "GET";
            return request;
        }
    }

}