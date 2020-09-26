using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace BingMultiThreadProxies
{
    public class Bing
    {
        string HtmlText1 = string.Empty;
        string strConn = string.Empty;

        public string keyword = "";
        public string seid = "";
        public string kwd = string.Empty;
        public string sd = string.Empty;
        public string ipstore = string.Empty;

        public string readConnection()
        {
            XmlDocument xml = new XmlDocument();
            string fileName = @"C:\Inetpub\wwwroot\ServerIP1.xml";
            // You'll need to put the correct path to your xml file here
            xml.Load(fileName);

            // Select a specific node
            XmlNode node = xml.SelectSingleNode("ConnectionString/con");
            // Get its value
            string name = node.InnerText;

            return name;
        }

        public string GetIP()
        {
            //return GetIPAddress(97);
            return "10.242.3.2";
            //return "82.136.12.130";
            //return "192.168.1.1";
        }

        public int x = 0;
        public DataTable dtIPs;
        Dictionary<string, CookieCollection> cookies = new Dictionary<string, CookieCollection>();

        public Bing()
        {
            strConn = readConnection();
            dtIPs = getIPsFromDB();
        }

        private DataTable getIPsFromDB()
        {
            DataTable dt = new DataTable();
            //string strQry = "Select id, address From IP_AddressIP5 order by Newid()";
            string strQry = "Select id, address From oxylabs_proxies order by Newid()";
            using (SqlDataAdapter da = new SqlDataAdapter(strQry, strConn))
            {
                da.Fill(dt);
            }
            return dt;
        }


        public async Task<string> getWebDataSource(string url)
        {
            //System.Threading.Thread.Sleep(1000);

            // getting IPs from db.
            if (dtIPs == null)
            {
                dtIPs = getIPsFromDB();
                if (dtIPs == null || dtIPs.Rows.Count == 0)
                {
                    throw new Exception("There is no IP to continue...");
                }
            }
            //REQUEST:
            Random rnd = new Random();
            x = rnd.Next(0, dtIPs.Rows.Count);

            //if (x >= dtIPs.Rows.Count) x = 0;

            string sendingIp = dtIPs.Rows[x][1].ToString();
            //string sendingIp = this.GetIP();
            
            StringBuilder stringBuilder = new StringBuilder();
            string value = string.Empty;

            Uri uri = new Uri(url);
            WebClient client = new WebClient();
            try
            {
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
                //ServicePoint servicePoint2 = ServicePointManager.FindServicePoint(uri);
                //servicePoint2.BindIPEndPointDelegate = ((ServicePoint servicePoint, IPEndPoint remoteEp, int retryCount) => new IPEndPoint(IPAddress.Parse(sendingIp), sendingPort));
                //servicePoint2.ConnectionLeaseTimeout = 0;

                WebProxy proxy = new WebProxy("http://" + dtIPs.Rows[x][1].ToString());
                NetworkCredential cred = new NetworkCredential("pidatametrics", "sbj4A3PLyZ");
                proxy.Credentials = cred;
                client.Proxy = proxy;

                string response = string.Empty;
                client.Encoding = Encoding.UTF8;

                response = client.DownloadString(uri);

                stringBuilder.Append(response);
            }
            catch (WebException ex)
            {
                value = ex.Message.ToString();
                stringBuilder.Append(value);
            }
            return await Task.FromResult<string>(stringBuilder.ToString());
        }

        public async Task<string> getWebDataMobileSource(string url)
        {
            //System.Threading.Thread.Sleep(1000);

            // getting IPs from db.
            if (dtIPs == null)
            {
                dtIPs = getIPsFromDB();
                if (dtIPs == null || dtIPs.Rows.Count == 0)
                {
                    throw new Exception("There is no IP to continue...");
                }
            }
            //if (x >= dtIPs.Rows.Count) x = 0;
            Random rnd = new Random();
            x = rnd.Next(0, dtIPs.Rows.Count);

            string sendingIp = dtIPs.Rows[x][1].ToString();
            // string sendingIp = this.GetIP();
           
            StringBuilder stringBuilder = new StringBuilder();
            string value = string.Empty;
            Uri uri = new Uri(url);
            WebClient client = new WebClient();
 

           
            

            try
            {
                //client.Headers.Add("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecho) Version/11.0 Mobile/15E148 Safari/604.1");
                //client.Headers.Add("user-agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Mobile Safari/537.36");
                //ServicePoint servicePoint2 = ServicePointManager.FindServicePoint(uri);
                //servicePoint2.BindIPEndPointDelegate = ((ServicePoint servicePoint, IPEndPoint remoteEp, int retryCount) => new IPEndPoint(IPAddress.Parse(sendingIp), sendingPort));
                //servicePoint2.ConnectionLeaseTimeout = 0;

                WebProxy proxy = new WebProxy("http://" + dtIPs.Rows[x][1].ToString());
                NetworkCredential cred = new NetworkCredential("pidatametrics", "sbj4A3PLyZ");
                proxy.Credentials = cred;
                client.Proxy = proxy;
                string response = string.Empty;
                client.Encoding = Encoding.UTF8;
                response = client.DownloadString(uri);
                stringBuilder.Append(response);

            }
           
            catch (WebException ex)
            {
                value = ex.Message.ToString();
                stringBuilder.Append(value);
            }
            return await Task.FromResult<string>(stringBuilder.ToString());
        }


        public ArrayList getTop100(string keyword, string seid)
        {
            kwd = WebUtility.UrlDecode(keyword);
            sd = seid;

            ArrayList myArrayList = new ArrayList();
            switch (seid)
            {
                case "5":
                    {
                        myArrayList = getTop100MSNUS(keyword);
                        break;
                    }
                case "6":
                    {
                        myArrayList = getTop100MSNUK(keyword);
                        break;
                    }
                case "15":
                    {
                        myArrayList = getTop100MSNNZ(keyword);
                        break;
                    }
                case "17":
                    {
                        myArrayList = getTop100MSNZA(keyword);
                        break;
                    }
                case "34":
                    {
                        myArrayList = getTop100MSNIT(keyword);
                        break;
                    }
                case "37":
                    {
                        myArrayList = getTop100MSNFR(keyword);
                        break;
                    }
                case "190":
                    {
                        myArrayList = getTop100BingUSMobile(keyword);
                        break;
                    }
                case "191":
                    {
                        myArrayList = getTop100BingUKMobile(keyword);
                        break;
                    }
            }
            return myArrayList;
        }

        private ArrayList DesktopPattern(string htmlsource, string ipstore, string kwd, string seid)
        {
            ArrayList googleList1 = new ArrayList();
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//ol[@id='b_results']/li[@class='b_algo']|//ol[@id='b_results']/li[@class='b_algo']/h2|//div[@class='b_algoheader']|//ol[@id='b_results']//li[@class='b_algo']/div[@class='b_title']/h2|//li[@class='b_algo']/h2"); //seid = 5

                //|//li[@class='b_algo']/h2
                foreach (HtmlNode links in node)
                {
                    try
                    {
                        HtmlNode a = links.SelectSingleNode(".//a");
                        string urls = a.Attributes["href"].Value;
                        if (urls.StartsWith("http") || urls.StartsWith("https"))
                        {
                            int indx = urls.LastIndexOf("http://");
                            if (indx < 0)
                            {
                                indx = urls.LastIndexOf("https://");
                            }
                            urls = urls.Remove(0, indx);
                            alDup.Add(HttpUtility.HtmlDecode(urls));
                        }
                    }
                    catch { continue; }
                }

                foreach (string s in alDup)
                {
                    if (googleList1.Contains(s) || string.IsNullOrEmpty(s)) continue;
                    googleList1.Add(s);
                }
                if (googleList1.Count > 100)
                {
                    googleList1.RemoveRange(100, googleList1.Count - 100);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No pattern match,  " + ex.Message);
            }
            return googleList1;
        }


        private ArrayList MobilePattern(string htmlsource, string ipstore, string kwd, string seid)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//ol[@id='b_results']/li[@class='b_algo']/div[@class='b_algoheader']|//div[@class='b_algoheader']|//div[@class='b_algoheader b_removeline12px']|//div[@class='b_algoheader b_removeline8px']");
                foreach (HtmlNode links in node)
                {
                    try
                    {
                        HtmlNode a = links.SelectSingleNode(".//a");
                        string urls = a.Attributes["href"].Value;
                        if (urls.StartsWith("http") || urls.StartsWith("https"))
                        {
                            int indx = urls.LastIndexOf("http://");
                            if (indx < 0)
                            {
                                indx = urls.LastIndexOf("https://");
                            }
                            urls = urls.Remove(0, indx);
                            alDup.Add(HttpUtility.HtmlDecode(urls));
                        }
                    }
                    catch { continue; }
                }
                foreach (string s in alDup)
                {
                    if (googleList.Contains(s) || string.IsNullOrEmpty(s)) continue;
                    googleList.Add(s);
                }
                //InsertIP(ipstore, 0, kwd, sd, googleList.Count);
                if (googleList.Count > 100)
                {
                    googleList.RemoveRange(100, googleList.Count - 100);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No pattern match,  " + ex.Message);
            }
            return googleList;
        }


        public ArrayList getTop100MSNUS(string keyword)
        {
            string HTML1 = "";
            string url = string.Empty;
            ArrayList top100MSNUS = new ArrayList();
            ArrayList iDup = new ArrayList();
            StringBuilder sb = new StringBuilder();
            try
            {

                int st = 1;
                for (int i = 0; i < 10; i++)
                {
                    HTML1 = string.Empty;
                    
                    url = "http://www.bing.com/search?q=" + keyword + "&search=&mkt=en-us&cc=us&form=QBRE&filt=all&first=" + st;

                    HTML1 = getWebDataSource(url).Result;

                    //System.IO.File.WriteAllText(@"c:\inetpub\wwwroot\html\" + kwd + st + ".html", HTML1, Encoding.UTF8);

                    if (HTML1.Contains("No results found for") || HTML1.Contains("There are no results for"))
                        break;
                    //string ipstore = dtIPs.Rows[x][1].ToString();
                    var item = DesktopPattern(HTML1, ipstore, kwd, sd);
                    st += item.Count;
                    iDup.AddRange(item);
                }
                foreach (string u in iDup)
                {
                    if (!top100MSNUS.Contains(u))
                        top100MSNUS.Add(u);
                }
                if (top100MSNUS.Count > 100)
                {
                    top100MSNUS.RemoveRange(100, top100MSNUS.Count - 100);
                }
            }
            catch (Exception ex)
            {
                string errorBingUS =  ex.Message.ToString();
                top100MSNUS.Add(errorBingUS);
            }
            return top100MSNUS;
        }

        public ArrayList getTop100MSNUK(string keyword)
        {
            string HTML1 = "";
            string url = string.Empty;
            ArrayList top100MSNUK = new ArrayList();
            ArrayList iDup = new ArrayList();
            StringBuilder sb = new StringBuilder();
            try
            {
                int st = 1;
                for (int i = 0; i < 10; i++)
                {
                    HTML1 = string.Empty;
                    url = "https://www.bing.com/search?q=" + keyword + "&mkt=en-GB&cc=GB&filt=all&form=QBRE&first=" + st;
                    HTML1 = getWebDataSource(url).Result;

                    //System.IO.File.WriteAllText(@"c:\inetpub\wwwroot\html\" + kwd + st + ".html", HTML1, Encoding.UTF8);

                    if (HTML1.Contains("No results found for") || HTML1.Contains("There are no results for"))
                        break;
                    var item = DesktopPattern(HTML1, ipstore, kwd, sd);
                    st += item.Count;
                    iDup.AddRange(item);
                }
                foreach (string u in iDup)
                {
                    if (!top100MSNUK.Contains(u))
                        top100MSNUK.Add(u);
                }
                if (top100MSNUK.Count > 100)
                {
                    top100MSNUK.RemoveRange(100, top100MSNUK.Count - 100);
                }
            }
            catch (Exception ex)
            {
                string errorBingUK =  ex.Message.ToString();
                top100MSNUK.Add(errorBingUK);
            }
            return top100MSNUK;
        }

        public ArrayList getTop100MSNNZ(string keyword)
        {
            string HTML1 = "";
            string url = string.Empty;
            ArrayList top100MSNNZ = new ArrayList();
            ArrayList iDup = new ArrayList();
            StringBuilder sb = new StringBuilder();
            try
            {
  
                int st = 1;
                for (int i = 0; i < 10; i++)
                {
                    HTML1 = string.Empty;                    
                    url = "http://www.bing.com/search?q=" + keyword + " &go=&form=MSNZHP&mkt=en-nz&filt=all&first=" + st + "&cc=nz";
                    HTML1 = getWebDataSource(url).Result;

                    //System.IO.File.WriteAllText(@"c:\inetpub\wwwroot\html\" + kwd + st + ".html", HTML1, Encoding.UTF8);

                    if (HTML1.Contains("No results found for") || HTML1.Contains("There are no results for"))
                        break;                    
                    var item = DesktopPattern(HTML1, ipstore, kwd, sd);
                    st += item.Count;
                    iDup.AddRange(item);
                }
                foreach (string u in iDup)
                {
                    if (!top100MSNNZ.Contains(u))
                        top100MSNNZ.Add(u);
                }
                if (top100MSNNZ.Count > 100)
                {
                    top100MSNNZ.RemoveRange(100, top100MSNNZ.Count - 100);
                }
            }
            catch (Exception ex)
            {
                string errorBingNZ = ex.Message.ToString();
                top100MSNNZ.Add(errorBingNZ);
            }
            return top100MSNNZ;
        }

        public ArrayList getTop100MSNZA(string keyword)
        {
            string HTML1 = "";
            string url = string.Empty;
            ArrayList top100MSNZA = new ArrayList();
            ArrayList iDup = new ArrayList();
            StringBuilder sb = new StringBuilder();
            try
            {

                int st = 1;
                for (int i = 0; i < 10; i++)
                {
                    HTML1 = string.Empty;
                    //int st = (10 * i) + 1;
                    url = "http://www.bing.com/search?q=" + keyword + "&form=QBRE&mkt=en-za&cc=za&first=" + st;

                    HTML1 = getWebDataSource(url).Result;

                    //System.IO.File.WriteAllText(@"c:\inetpub\wwwroot\html\" + kwd + st + ".html", HTML1, Encoding.UTF8);
                    if (HTML1.Contains("No results found for") || HTML1.Contains("There are no results for"))
                        break;
                    var item = DesktopPattern(HTML1, ipstore, kwd, sd);
                    st += item.Count;
                    iDup.AddRange(item);
                }
                foreach (string u in iDup)
                {
                    if (!top100MSNZA.Contains(u))
                        top100MSNZA.Add(u);
                }
                if (top100MSNZA.Count > 100)
                {
                    top100MSNZA.RemoveRange(100, top100MSNZA.Count - 100);
                }
            }
            catch (Exception ex)
            {
                string errorBingZA = ex.Message.ToString();
                top100MSNZA.Add(errorBingZA);
            }
            return top100MSNZA;
        }

        public ArrayList getTop100MSNIT(string keyword)
        {
            string HTML1 = "";
            string url = string.Empty;
            ArrayList top100MSNIT = new ArrayList();
            ArrayList iDup = new ArrayList();
            StringBuilder sb = new StringBuilder();
            try
            {

                int st = 1;
                for (int i = 0; i < 10; i++)
                {
                    HTML1 = string.Empty;                    
                    url = "http://www.bing.com/search?q=" + keyword + "&form=QBLR&filt=all&first=" + st + "&mkt=en-it&cc=it";
                    HTML1 = getWebDataSource(url).Result;

                    //System.IO.File.WriteAllText(@"c:\inetpub\wwwroot\html\" + kwd + st + ".html", HTML1, Encoding.UTF8);
                    if (HTML1.Contains("No results found for") || HTML1.Contains("There are no results for"))
                        break;
                    var item = DesktopPattern(HTML1, ipstore, kwd, sd);
                    st += item.Count;
                    iDup.AddRange(item);
                }
                foreach (string u in iDup)
                {
                    if (!top100MSNIT.Contains(u))
                        top100MSNIT.Add(u);
                }
                if (top100MSNIT.Count > 100)
                {
                    top100MSNIT.RemoveRange(100, top100MSNIT.Count - 100);
                }
            }
            catch (Exception ex)
            {
                string errorBingIT = ex.Message.ToString();
                top100MSNIT.Add(errorBingIT);
            }
            return top100MSNIT;
        }

        public ArrayList getTop100MSNFR(string keyword)
        {
            string HTML1 = "";
            string url = string.Empty;
            ArrayList top100MSNFR = new ArrayList();
            ArrayList iDup = new ArrayList();
            StringBuilder sb = new StringBuilder();
            try
            {

                int st = 1;
                for (int i = 0; i < 10; i++)
                {
                    HTML1 = string.Empty;

                    url = "http://www.bing.com/search?q=" + keyword + "&form=QBLR&filt=all&first=" + st + "&mkt=en-it&cc=it";
                    HTML1 = getWebDataSource(url).Result;

                    //System.IO.File.WriteAllText(@"c:\inetpub\wwwroot\html\" + kwd + st + ".html", HTML1, Encoding.UTF8);
                    if (HTML1.Contains("No results found for") || HTML1.Contains("There are no results for"))
                        break;
                    
                    var item = DesktopPattern(HTML1, ipstore, kwd, sd);
                    st += item.Count;
                    iDup.AddRange(item);
                }
                foreach (string u in iDup)
                {
                    if (!top100MSNFR.Contains(u))
                        top100MSNFR.Add(u);
                }
                if (top100MSNFR.Count > 100)
                {
                    top100MSNFR.RemoveRange(100, top100MSNFR.Count - 100);
                }
            }
            catch (Exception ex)
            {
                string errorBingFR = ex.Message.ToString();
                top100MSNFR.Add(errorBingFR);
            }
            return top100MSNFR;
        }

        public ArrayList getTop100BingUSMobile(string keyword)
        {
            string HTML1 = "";
            string url = string.Empty;
            ArrayList top100BingUSMobile = new ArrayList();
            ArrayList iDup = new ArrayList();
            StringBuilder sb = new StringBuilder();
            try
            {
                int st = 1;
                for (int i = 0; i < 10; i++)
                {
                    HTML1 = string.Empty;                    
                    url = "http://www.bing.com/search?q=" + keyword + "&mkt=en-us&cc=us&form=QBLH&filt=all&first=" + st;    // + "&count=10";
                    HTML1 = getWebDataMobileSource(url).Result;

                    //System.IO.File.WriteAllText(@"c:\inetpub\wwwroot\html\" + kwd + st + ".html", HTML1, Encoding.UTF8);

                    if (HTML1.Contains("No results found for") || HTML1.Contains("There are no results for"))
                        break;

                    var item = MobilePattern(HTML1, ipstore, kwd, sd);
                    st += item.Count;
                    
                    iDup.AddRange(item);
                }
                foreach (string u in iDup)
                {
                    if (!top100BingUSMobile.Contains(u))
                        top100BingUSMobile.Add(u);
                }
                if (top100BingUSMobile.Count > 100)
                {
                    top100BingUSMobile.RemoveRange(100, top100BingUSMobile.Count - 100);
                }
            }
            catch (Exception ex)
            {
                string errorBingUSMobile = ex.Message.ToString();
                top100BingUSMobile.Add(errorBingUSMobile);
            }
            return top100BingUSMobile;
        }

        public ArrayList getTop100BingUKMobile(string keyword)
        {
            string HTML1 = "";
            string url = string.Empty;
            ArrayList top100BingUKMobile = new ArrayList();
            ArrayList iDup = new ArrayList();
            StringBuilder sb = new StringBuilder();
            try
            {
                int st = 1;
                for (int i = 0; i < 10; i++)
                {
                    HTML1 = string.Empty;

                    url = "http://www.bing.com/search?q=" + keyword + "&form=QBRE&first=" + st + "&mkt=en-gb&cc=gb&filt=all";
                    HTML1 = getWebDataMobileSource(url).Result;

                    //System.IO.File.WriteAllText(@"c:\inetpub\wwwroot\html\" + kwd + st + ".html", HTML1, Encoding.UTF8);
                    if (HTML1.Contains("No results found for") || HTML1.Contains("There are no results for"))
                        break;

                    //string ipstore = dtIPs.Rows[x][1].ToString();
                    var item = MobilePattern(HTML1, ipstore, kwd, sd);

                    st += item.Count;
                    iDup.AddRange(item);
                }
                foreach (string u in iDup)
                {
                    if (!top100BingUKMobile.Contains(u))
                        top100BingUKMobile.Add(u);
                }
                if (top100BingUKMobile.Count > 100)
                {
                    top100BingUKMobile.RemoveRange(100, top100BingUKMobile.Count - 100);
                }
            }
            catch (Exception ex)
            {
                string errorBingUkMobile = ex.Message.ToString();
                top100BingUKMobile.Add(errorBingUkMobile);
            }
            return top100BingUKMobile;
        }
    }
}
