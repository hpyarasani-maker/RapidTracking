using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace RapidTrackingMultithreadFirstPageResIPs
{
    class OxyResidentialProxies
    {
        readonly string strConn = string.Empty;
        const string googleurl = "https://www.google.";
        const string safesearch = "0";
        const string safe = "off";
        const string num = "100";
        const string aomd = "1";
        string url = string.Empty;
        //public string IP = string.Empty;

        
        private string GetProxyIP(string country)
        {
            var client = new WebClient();
            client.Proxy = new WebProxy("pr.oxylabs.io:7777");
            string aaa = $"residatametrics-{country}";
            client.Proxy.Credentials = new NetworkCredential($"customer-residatametrics-{country}", "ITQxGcjDdiq2oHM44UBX^");
            string res = client.DownloadString("https://ip.oxylabs.io/location");
            JObject obj = JObject.Parse(res);
            string ip = obj["ip"].Value<string>();
            return ip;
        }


        public string GetWebDataSource(string url, string country, out string ip)
        {
            string ipaddress = string.Empty;
            try
            {
                ip = GetProxyIP(country);
                ipaddress = ip;
                Uri uri = new Uri(url);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.CookieContainer = new CookieContainer();
                req.Headers.Clear();
                req.UseDefaultCredentials = true;
                req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36";
                WebProxy proxy;
                if (ip.Length <= 15)
                {
                    proxy = new WebProxy
                    {
                        Address = new Uri("http://" + ip + ":60000"),//ipv4
                        //Address = new Uri("http://[" + ip + "]:60000"),//ipv6
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("customer-residatametrics", "ITQxGcjDdiq2oHM44UBX^")
                        //Credentials = new NetworkCredential("customer-residatametrics" + "-sessid-" + session_id, "ITQxGcjDdiq2oHM44UBX^")
                    };
                }
                else
                {
                    proxy = new WebProxy
                    {
                        //Address = new Uri("http://" + ip + ":60000"),//ipv4
                        Address = new Uri("http://[" + ip + "]:60000"),//ipv6
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("customer-residatametrics", "ITQxGcjDdiq2oHM44UBX^")
                        //Credentials = new NetworkCredential("customer-residatametrics" + "-sessid-" + session_id, "ITQxGcjDdiq2oHM44UBX^")
                    };
                }
                var handler = new HttpClientHandler
                {
                    Proxy = proxy,
                };
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK) throw new Exception(res.StatusDescription);
                // replace the cookie ...
                //** get the stream of data and read into a string
                Stream respStream = res.GetResponseStream();
                //** Contents of HTML in the Response object to a Stream reader
                StreamReader reader = new StreamReader(respStream, Encoding.UTF8); //windows default code page
                String respHTML = reader.ReadToEnd();
                respStream.Close();
                res.Close();
                return respHTML;
            }
            catch (Exception ex)
            {
                throw new Exception("IP Error: " + ipaddress + ex.Message);
            }
        }

        public string GetWebDataMobileSource(string url, string country, out string ip)
        {
            string ipaddress = string.Empty;
            try
            {
                ip = GetProxyIP(country);
                ipaddress = ip;
                //string session_id = new Random().Next().ToString();
                Uri uri = new Uri(url);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.CookieContainer = new CookieContainer();
                req.Headers.Clear();
                req.UseDefaultCredentials = true;
                req.UserAgent = @"Mozilla/5.0 (iPhone; CPU iPhone OS 14_4_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) FxiOS/124.0 Mobile/15E148 Safari/605.1.15";
                WebProxy proxy;
                if (ip.Length <= 15)
                {
                    proxy = new WebProxy
                    {
                        Address = new Uri("http://" + ip + ":60000"),//ipv4
                        //Address = new Uri("http://[" + ip + "]:60000"),//ipv6
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("customer-residatametrics", "ITQxGcjDdiq2oHM44UBX^")
                        //Credentials = new NetworkCredential("customer-residatametrics" + "-sessid-" + session_id, "ITQxGcjDdiq2oHM44UBX^")
                    };
                }
                else
                {
                    proxy = new WebProxy
                    {
                        //Address = new Uri("http://" + ip + ":60000"),//ipv4
                        Address = new Uri("http://[" + ip + "]:60000"),//ipv6
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("customer-residatametrics", "ITQxGcjDdiq2oHM44UBX^")
                        //Credentials = new NetworkCredential("customer-residatametrics" + "-sessid-" + session_id, "ITQxGcjDdiq2oHM44UBX^")
                    };
                }
                var handler = new HttpClientHandler
                {
                    Proxy = proxy,
                };
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK) throw new Exception(res.StatusDescription);
                // replace the cookie ...
                //** get the stream of data and read into a string
                Stream respStream = res.GetResponseStream();
                //** Contents of HTML in the Response object to a Stream reader
                StreamReader reader = new StreamReader(respStream, Encoding.UTF8); //windows default code page
                String respHTML = reader.ReadToEnd();
                respStream.Close();
                res.Close();
                return respHTML;
            }
            catch (Exception ex)
            {
                throw new Exception("IP Error: " + ipaddress + ex.Message);
            }
        }

        public string[] GetTop100Desktop(string keyword,string country, int seid, out string ip, string domain, string locale, string uule)
        {

            ArrayList DesktopResult = new ArrayList();

            string[] locale1 = locale.Split('-');

            if (locale1.Length == 3)
            {
                if (locale1[1] == "419" || locale1[1] == "TW")
                {
                    locale1[0] = locale1[0] + "-" + locale1[1];
                }
                //url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[2] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "&gws_rd=ssl,cr";
                url = "https://www.google." + domain + "/search?q=" + keyword + "&gl=" + locale1[2] + "&hl=" + locale1[0] + "&safe_search=0&safe=off&aomd=1";

            }

            else if (locale1.Length == 2)
            {
                //url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "&gws_rd=ssl,cr";
                url = "https://www.google." + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&safe_search=0&safe=off&aomd=1";

            }

            string HTML = GetWebDataSource(url,country, out ip);
            string[] dr = DesktoppatternTrending(HTML, keyword, seid.ToString());

            return dr;

        }
        //----------------------------------------------- For Non Hotel Keywords -------------------------------------//
        public string[] GetTop100Mobile(string keyword, string country, int seid, out string ip, string domain, string locale, string uule)
        {
            ArrayList MobileResult = new ArrayList();

            string[] locale1 = locale.Split('-');

            if (locale1.Length == 3)
            {
                if (locale1[1] == "419" || locale1[1] == "TW")
                {
                    locale1[0] = locale1[0] + "-" + locale1[1];
                }
                //url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[2] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "-gws-serp.3.0&gws_rd=ssl,cr";
                url = "https://www.google." + domain + "/search?q=" + keyword + "&gl=" + locale1[2] + "&hl=" + locale1[0] + "&safe_search=0&safe=off&aomd=1";

            }
            else if (locale1.Length == 2)
            {
                //url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "-gws-serp.3.0&gws_rd=ssl,cr";
                url = "https://www.google." + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&safe_search=0&safe=off&aomd=1";

            }

            string HTML = GetWebDataMobileSource(url,country, out ip);
            //File.WriteAllText(@"c:\inetpub\wwwroot\dallas.html", HTML);
            string[] mr = MobilepatternTrending(HTML, keyword, seid.ToString());
            return mr;

        }


        private string[] DesktoppatternTrending(string html, string keyword, string seid)
        {
            string[] array = new string[2];
            string res = "";
            var doc = new HtmlAgilityPack.HtmlDocument();

            Desktop clsDesktop = new Desktop();

            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            res = clsDesktop.ProcessDocument(seid, keyword, doc);
            array[0] = res;
            array[1] = clsDesktop.orgLinks.ToString();
            return array;
        }
        private string[] MobilepatternTrending(string html, string keyword, string seid)
        {
            string[] array = new string[2];
            string res = "";
            var doc = new HtmlAgilityPack.HtmlDocument();

            iOS clsMobile = new iOS();

            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            res = clsMobile.ProcessDocument(seid, keyword, doc);
            array[0] = res;
            array[1] = clsMobile.orgLinks.ToString();
            return array;
        }
        public string[] GetTop100(string keyword, int seid, out string ip)
        {
            string[] seresults = new string[1];

            IEnumerable<SearchParams.SearchProperties> list = SearchParams.searches.ToList<SearchParams.SearchProperties>().Where(s => s.seid == seid);
            string sip = string.Empty;
            foreach (var value in list)
            {
                if (value.device == "desktop")
                {
                    seresults = GetTop100Desktop(keyword, value.country, seid, out sip, value.domain, value.locale, value.uule);
                }
                else if (value.device == "mobile_android")
                {
                    seresults = GetTop100Mobile(keyword, value.country, seid, out sip, value.domain, value.locale, value.uule);
                }
            }
            ip = sip;
            return seresults;
        }
    }
}
