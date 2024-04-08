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


namespace RapidTrackingResSingleThread
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

        public OxyResidentialProxies()
        {
            strConn = Common.ReadConnection();
            dtIPs = Common.GetIPsFromDB();
        }


        public int x = 0;
        public DataTable dtIPs;

        Random rnd;
        public string GetWebDataSource(string url, out string ip)
        {
            int x = 0;
            try
            {
                // getting IPs from db.
                if (dtIPs == null)
                {
                    dtIPs = Common.GetIPsFromDB();
                }
                rnd = new Random();
                x = rnd.Next(dtIPs.Rows.Count);
                ip = dtIPs.Rows[x][1].ToString();

                Uri uri = new Uri(url);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.CookieContainer = new CookieContainer();
                req.Headers.Clear();
                req.UseDefaultCredentials = true;
                //req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36";
                //req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.97 Safari/537.36";
                req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.135 Safari/537.36";

                // port is changed from '6747' to '6747'.
                WebProxy proxy = new WebProxy(dtIPs.Rows[x][1].ToString());
                NetworkCredential cred = new NetworkCredential("pidatametrics", "sbj4A3PLyZ");

                proxy.Credentials = cred;

                req.Proxy = proxy;

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                if (res.StatusCode != HttpStatusCode.OK) throw new Exception(res.StatusDescription);
                // replace the cookie ...
                //** get the stream of data and read into a string
                Stream respStream = res.GetResponseStream();

                //** Contents of HTML in the Response object to a Stream reader
                StreamReader reader = new StreamReader(respStream, Encoding.UTF8); //windows default code page
                //** Store all the contents
                String respHTML = reader.ReadToEnd();

                respStream.Close();
                res.Close();
                return respHTML;
            }
            catch (Exception ex)
            {
                throw new Exception("IP: " + dtIPs.Rows[x][1].ToString() + " : Error: " + ex.Message);
            }
        }

        public string GetWebDataMobileSource(string url, out string ip)
        {
            int x = 0;
            try
            {
                // getting IPs from db.
                if (dtIPs == null)
                {
                    dtIPs = Common.GetIPsFromDB();
                }
                rnd = new Random();
                x = rnd.Next(0, dtIPs.Rows.Count);
                ip = dtIPs.Rows[x][1].ToString();
                Uri uri = new Uri(url);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.CookieContainer = new CookieContainer();
                req.Headers.Clear();
                req.UseDefaultCredentials = true;
                req.UserAgent = @"Mozilla/5.0 (iPhone; CPU iPhone OS 12_4_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.2 Mobile/15E148 Safari/604.1";
                //req.UserAgent = @"Mozilla /5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1";
                //req.UserAgent = @"Mozilla/5.0 (iPod; CPU iPhone OS 12_0 like macOS) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/12.0 Mobile/14A5335d Safari/602.1.50";
                //req.UserAgent = @"Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Mobile Safari/537.36";                
                //req.UserAgent = @"Mozilla/5.0 (Linux; Android 8.1.0; Mi A2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.105 Mobile Safari/537.36";
                //req.UserAgent = @"Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Mobile Safari/537.36";
                //req.UserAgent = @"Mozilla/5.0 (iPod; CPU iPhone OS 12_0 like macOS) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/12.0 Mobile/14A5335d Safari/602.1.50";
                WebProxy proxy = new WebProxy(dtIPs.Rows[x][1].ToString());
                NetworkCredential cred = new NetworkCredential("pidatametrics", "sbj4A3PLyZ");

                proxy.Credentials = cred;

                req.Proxy = proxy;

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK) throw new Exception(res.StatusDescription);

                //** get the stream of data and read into a string
                Stream respStream = res.GetResponseStream();
                //** Contents of HTML in the Response object to a Stream reader
                StreamReader reader = new StreamReader(respStream, Encoding.UTF8); //windows default code page
                //** Store all the contents
                String respHTML = reader.ReadToEnd();

                respStream.Close();
                res.Close();
                return respHTML;

            }
            catch (Exception ex)
            {
                throw new Exception("IP: " + dtIPs.Rows[x][1].ToString() + " : Error: " + ex.Message);
            }
        }
        string carona = "&stick=H4sIAAAAAAAAAONgVuLVT9c3NMwySk6OL8zJecTozS3w8sc9YSmnSWtOXmO04eIKzsgvd80rySypFNLjYoOyVLgEpVB1ajBI8XOhCvHsYuLIL0stKstMLV_Eyu2cX5Sfl1iWWVRaDADEmcfgeAAAAA&ictx=1&ved=2ahUKEwizy5j8x_HvAhWg7HMBHer6DvAQyNoBKAB6BQiGARAG";

        public object SearchParams { get; private set; }

        public string[] GetTop100Desktop(string keyword, int seid, out string ip, string domain, string locale, string uule)
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
                url = "https://www.google." + domain + "/search?q=" + keyword + "&gl=" + locale1[2] + "&hl=" + locale1[0] + "&num=100&safe_search=0&safe=off&aomd=1" + "&uule=" + uule;

            }

            else if (locale1.Length == 2)
            {
                //url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "&gws_rd=ssl,cr";
                url = "https://www.google." + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&num=100&safe_search=0&safe=off&aomd=1" + "&uule=" + uule;

            }

            string HTML = GetWebDataSource(url, out ip);
            string[] dr = DesktoppatternTrending(HTML, keyword, seid.ToString());

            return dr;

        }
        //----------------------------------------------- For Non Hotel Keywords -------------------------------------//
        public string[] GetTop100Mobile(string keyword, int seid, out string ip, string domain, string locale, string uule)
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
                url = "https://www.google." + domain + "/search?q=" + keyword + "&gl=" + locale1[2] + "&hl=" + locale1[0] + "&num=100&safe_search=0&safe=off&aomd=1" + "&uule=" + uule;

            }
            else if (locale1.Length == 2)
            {
                //url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "-gws-serp.3.0&gws_rd=ssl,cr";
                url = "https://www.google." + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&num=100&safe_search=0&safe=off&aomd=1" + "&uule=" + uule;

            }

            string HTML = GetWebDataMobileSource(url, out ip);
            //File.WriteAllText(@"c:\inetpub\wwwroot\dallas.html", HTML);
            string[] mr = MobilepatternTrending(HTML, keyword, seid.ToString());
            return mr;

        }


        //---------------------------------------For Hotel Keywords---------------------------//
        /*public string[] getTop100Mobile(string keyword, int seid, out string oIP, string domain, string locale, string uule, string device)
        {
            ArrayList MobileResult = new ArrayList();
            string HTML = string.Empty;
            string[] locale1 = locale.Split('-');

            if (locale1.Length == 3)
            {
                if (locale1[1] == "419" || locale1[1] == "TW")
                {
                    locale1[0] = locale1[0] + "-" + locale1[1];
                }
                url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[2] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&filter=" + filter + "&uule=" + uule + "&gs_l=" + device + "-gws-serp.3.0&gws_rd=ssl,cr";
            }
            else if (locale1.Length == 2)
            {
                for (int i = 0; i <= 4; i++)
                {
                    int t = (20 * i);
                    url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&num=" + num + "&start=" + t +"&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "-gws-serp.3.0&gws_rd=ssl,cr";
                    HTML += getWebDataMobileSource(url);


                }
            }
           // File.WriteAllText(@"c:\inetpub\wwwroot", HTML, Encoding.UTF8);
            //url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&filter=" + filter + "&uule=" + uule + "&gs_l=" + device + "-gws-serp.3.0&gws_rd=ssl,cr";

            //url = "https://www.google.co.uk/search?source=hp&ei=M33ZXJuXFsn5rQHuoKHAAg&q="+keyword+"&num=100&gs_l=mobile-gws-wiz-hp.1.0.41j0l8.3767.4827..5920...0.0..0.141.513.0j4......0....1.......1..0i131j46.xoJA43z_r8Y";

            //string HTML = getWebDataMobileSource(url);
            //GetImages(HTML);
            string[] mr = mobilepatternTrending(HTML, keyword, seid.ToString());
            oIP = sIP;
            return mr;
        }*/


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

            IEnumerable<OxyResSearchParams.SearchProperties> list = OxyResSearchParams.searches.ToList<OxyResSearchParams.SearchProperties>().Where(s => s.seid == seid);
            string sip = string.Empty;
            foreach (var value in list)
            {
                if (value.device == "desktop")
                {
                    seresults = GetTop100Desktop(keyword, seid, out sip, value.domain, value.locale, value.uule);
                }
                else if (value.device == "mobile_android")
                {
                    seresults = GetTop100Mobile(keyword, seid, out sip, value.domain, value.locale, value.uule);
                }
            }
            ip = sip;
            return seresults;
        }
    }
}
