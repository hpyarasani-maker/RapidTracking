using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace RapidTrackingSingleThread
{
    class OxylabsProxies
    {
        string strConn = string.Empty;
        public string sIP = string.Empty;
        const string googleurl = "https://www.google.";
        const string safesearch = "0";
        const string safe = "off";
        const string num = "100";
        const string aomd = "1";
        string url = string.Empty;


        public OxylabsProxies()
        {
            strConn = readConnection();
            //dtIPs = getIPsFromDB();
        }

        public string readConnection()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("ConnectionString/con");
                // Get its value
                string name = node.InnerText;

                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int x = 0;
        public DataTable dtIPs;
        Dictionary<string, CookieCollection> cookies = new Dictionary<string, CookieCollection>();
        private DataTable getIPsFromDB()
        {

            DataTable dt = new DataTable();
            string strQry = "Select id, address From oxylabs_proxies";

            using (SqlDataAdapter da = new SqlDataAdapter(strQry, strConn))
            {
                da.Fill(dt);
            }
            return dt;
        }


        Random rnd;
        public string getWebDataSource(string url)
        {
            int x = 0;
            sIP = string.Empty;
            try
            {
                // getting IPs from db.
                if (dtIPs == null)
                {
                    dtIPs = getIPsFromDB();
                }
                rnd = new Random();
                x = rnd.Next(0, dtIPs.Rows.Count);
                Uri uri = new Uri(url);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.Headers.Clear();
                req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36";
                // port is changed from '6747' to '6747'.
                WebProxy proxy = new WebProxy("http://" + dtIPs.Rows[x][1].ToString());
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

        string[] lastIP = new string[2];
        ArrayList al = new ArrayList();
        public string getWebDataMobileSource(string url)
        {

            int x = 0;
            sIP = string.Empty;
            try
            {
                // getting IPs from db.
                if (dtIPs == null)
                {
                    dtIPs = getIPsFromDB();
                }
                rnd = new Random();
                x = rnd.Next(0, dtIPs.Rows.Count);
                Uri uri = new Uri(url);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.Headers.Clear();
                req.UserAgent = @"Mozilla/5.0 (iPhone; CPU iPhone OS 12_4_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.2 Mobile/15E148 Safari/604.1";
                WebProxy proxy = new WebProxy("http://" + dtIPs.Rows[x][1].ToString());
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

        public string[] getTop100Desktop(string keyword, int seid, out string oIP, string domain, string locale, string uule, string device)
        {
            ArrayList DesktopResult = new ArrayList();

            string[] locale1 = locale.Split('-');

            if (locale1.Length == 3)
            {
                if (locale1[1] == "419" || locale1[1] == "TW")
                {
                    locale1[0] = locale1[0] + "-" + locale1[1];
                }
                url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[2] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "&gws_rd=ssl,cr";
            }
            else if (locale1.Length == 2)
            {
                url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "&gws_rd=ssl,cr";
            }

            string HTML = getWebDataSource(url);
            string[] dr = desktoppatternTrending(HTML, keyword, seid.ToString());

            oIP = sIP;
            return dr;
        }

        public string[] getTop100Mobile(string keyword, int seid, out string oIP, string domain, string locale, string uule, string device)
        {
            ArrayList MobileResult = new ArrayList();

            string[] locale1 = locale.Split('-');

            if (locale1.Length == 3)
            {
                if (locale1[1] == "419" || locale1[1] == "TW")
                {
                    locale1[0] = locale1[0] + "-" + locale1[1];
                }
                url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[2] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "-gws-serp.3.0&gws_rd=ssl,cr";
            }
            else if (locale1.Length == 2)
            {
                url = "" + googleurl + "" + domain + "/search?q=" + keyword + "&gl=" + locale1[1] + "&hl=" + locale1[0] + "&num=" + num + "&safe_search=" + safesearch + "&safe=" + safe + "&aomd=" + aomd + "&uule=" + uule + "&gs_l=" + device + "-gws-serp.3.0&gws_rd=ssl,cr";
            }

            string HTML = getWebDataMobileSource(url);
            string[] mr = mobilepatternTrending(HTML, keyword, seid.ToString());
            oIP = sIP;
            return mr;
        }


        private string[] desktoppatternTrending(string html, string keyword, string seid)
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
        private string[] mobilepatternTrending(string html, string keyword, string seid)
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
        public string[] GetTop100(string keyword, int seid)
        {
            string[] seresults = new string[1];
            IEnumerable<SearchProperties> list = SearchParams.searches.ToList<SearchProperties>().Where(s => s.seid == seid);

            foreach (var value in list)
            {
                if (value.device == "desktop")
                {
                    seresults = getTop100Desktop(keyword, seid, out sIP, value.domain, value.locale, value.uule, value.device);
                }
                else if (value.device == "mobile")
                {
                    seresults = getTop100Mobile(keyword, seid, out sIP, value.domain, value.locale, value.uule, value.device);
                }
            }
            return seresults;
        }
    }
}
