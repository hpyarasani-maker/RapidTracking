using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace RapidTrackingMultithread
{
    class ServerIP
    {

        string strConn = string.Empty;
        public string sIP = string.Empty;
        const string googleurl = "https://www.google.";
        const string safesearch = "0";
        const string safe = "off";
        const string num = "100";
        const string aomd = "1";
        string url = string.Empty;

        public ServerIP()
        {
            strConn = readConnection();
            dtIPs = getIPsFromDB();
        }

        string error1 = string.Empty;
        public string readConnection()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";

                // You'll need to put the correct path to your xml file here
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

        public string GetIPAddress(int id)
        {
            string address = string.Empty;
            string strQuery = "exec [dbo].[GetIPAddress] '" + id + "'";

            SqlConnection objCon = new SqlConnection(strConn);
            try
            {
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strQuery, objCon);
                objCmd.CommandTimeout = 0;
                SqlDataReader objData = null;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (objData.Read())
                {
                    address = objData[0].ToString();
                }
                objData.Close();
            }
            catch (SqlException e)
            {
                string errMsg = "Database Connection is temporarily not working\n" + e.ToString();
            }
            finally
            {
                if (objCon.State == ConnectionState.Open)
                {
                    objCon.Close();
                }
            }
            return address;
        }
        public int x = 0;
        public DataTable dtIPs;
        Dictionary<string, CookieCollection> cookies = new Dictionary<string, CookieCollection>();
        private DataTable getIPsFromDB()
        {
            DataTable dt = new DataTable();
            string strQry = "Select id, address From IP_Address";

            using (SqlDataAdapter da = new SqlDataAdapter(strQry, strConn))
            {
                da.Fill(dt);
            }
            return dt;
        }

        public string GetIP()
        {
            //return GetIPAddress(97);
            return "10.242.3.3";
        }
        public string getWebDataSource(string url)
        {
            System.Threading.Thread.Sleep(1000);

            // getting IPs from db.
            if (dtIPs == null)
            {
                dtIPs = getIPsFromDB();
                if (dtIPs == null || dtIPs.Rows.Count == 0)
                {
                    throw new Exception("There is no IP to continue...");
                }
            }

            Random rnd = new Random();
            x = rnd.Next(0, dtIPs.Rows.Count);

            string sendingIp = dtIPs.Rows[x][1].ToString();
            //string sendingIp = this.GetIP();
            int sendingPort = 0;
            StringBuilder stringBuilder = new StringBuilder();
            string value = string.Empty;
            Uri uri = new Uri(url);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Clear();
            httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.97 Safari/537.36";            
            try
            {
                ServicePoint servicePoint2 = ServicePointManager.FindServicePoint(uri);
                servicePoint2.BindIPEndPointDelegate = ((ServicePoint servicePoint, IPEndPoint remoteEp, int retryCount) => new IPEndPoint(IPAddress.Parse(sendingIp), sendingPort));
                servicePoint2.ConnectionLeaseTimeout = 0;
                HttpWebResponse res = (HttpWebResponse)httpWebRequest.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK) throw new Exception(res.StatusDescription);
                value = new StreamReader(res.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                stringBuilder.Append(value);
                res.Close();
            }
            catch (WebException ex)
            {
                value = ex.Message.ToString();
                stringBuilder.Append(value);
            }
            return stringBuilder.ToString();
        }

        public string getWebDataMobileSource(string url)
        {
            System.Threading.Thread.Sleep(1000);

            // getting IPs from db.
            if (dtIPs == null)
            {
                dtIPs = getIPsFromDB();
                if (dtIPs == null || dtIPs.Rows.Count == 0)
                {
                    throw new Exception("There is no IP to continue...");
                }
            }
            Random rnd = new Random();
            x = rnd.Next(0, dtIPs.Rows.Count);

            string sendingIp = dtIPs.Rows[x][1].ToString();
            //string sendingIp = this.GetIP();
            int sendingPort = 0;
            StringBuilder stringBuilder = new StringBuilder();
            string value = string.Empty;
            Uri uri = new Uri(url);            
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Clear();
            httpWebRequest.UserAgent = @"Mozilla/5.0 (iPhone; CPU iPhone OS 12_4_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.2 Mobile/15E148 Safari/604.1";
            try
            {
                ServicePoint servicePoint2 = ServicePointManager.FindServicePoint(uri);
                servicePoint2.BindIPEndPointDelegate = ((ServicePoint servicePoint, IPEndPoint remoteEp, int retryCount) => new IPEndPoint(IPAddress.Parse(sendingIp), sendingPort));
                servicePoint2.ConnectionLeaseTimeout = 0;
                HttpWebResponse res = (HttpWebResponse)httpWebRequest.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK) throw new Exception(res.StatusDescription);
                value = new StreamReader(res.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                stringBuilder.Append(value);
            }
            catch (WebException ex)
            {
                value = ex.Message.ToString();
                stringBuilder.Append(value);
            }
            return stringBuilder.ToString();
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

            try
            {                
                IEnumerable<SearchProperties> list = SearchParams.searches.ToList<SearchProperties>().Where(s => s.seid == seid);

                foreach (var value in list)
                {
                    if (value.device == "desktop")
                    {
                        seresults = getTop100Desktop(keyword, seid, out sIP, value.domain, value.locale, value.uule, value.device);
                    }
                    else if (value.device == "mobile_android")
                    {
                        seresults = getTop100Mobile(keyword, seid, out sIP, value.domain, value.locale, value.uule, value.device);
                    }
                }
            }
            catch (Exception ex)
            {
                string geterror = ex.Message.ToString();
            }
            return seresults;
        }
    }
}
