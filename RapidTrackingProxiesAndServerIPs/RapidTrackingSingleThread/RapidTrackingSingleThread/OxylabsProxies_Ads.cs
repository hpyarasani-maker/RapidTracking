using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RapidTrackingSingleThread
{
    public class OxylabsProxies_Ads
    {
        const string googleurl = "https://www.google.";
        const string safesearch = "0";
        const string safe = "off";
        const string num = "100";
        const string aomd = "1";
        string url = string.Empty;

        public OxylabsProxies_Ads()
        {
            
        }
   
        public async Task<string> GetWebDataSource(string url)
        {
            string source = string.Empty;
            Uri uri = new Uri(url);
            string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36 Edg/102.0.1245.39";
            using (var handler = new HttpClientHandler() { UseCookies = false })
            using (var client = new HttpClient(handler))
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    source = await client.GetStringAsync(uri);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return source;
        }

        public async Task<string> GetWebDataMobileSource(string url)
        {
            string source = string.Empty;
            Uri uri = new Uri(url);
            string userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_4_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.2 Mobile/15E148 Safari/604.1";
            using (var handler = new HttpClientHandler() { UseCookies = false })
            using (var client = new HttpClient(handler))
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    source = await client.GetStringAsync(uri);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return source;
        }

        public string[] GetTop100Desktop(string keyword, int seid, string domain, string locale, string uule)
        {
            try
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

                string HTML = GetWebDataSource(url).Result;
                //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + seid + "_" + keyword + ".html", HTML, Encoding.UTF8);
                string[] dr = DesktoppatternTrending(HTML, keyword, seid.ToString());
                return dr;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //----------------------------------------------- Mobile Keywords -------------------------------------//
        public string[] GetTop100Mobile(string keyword, int seid, string domain, string locale, string uule)
        {
            try
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

                string HTML = GetWebDataMobileSource(url).Result;
                //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + seid + "_" + keyword + ".html", HTML, Encoding.UTF8);
                string[] mr = MobilepatternTrending(HTML, keyword, seid.ToString());
                return mr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            IEnumerable<SearchProperties> list = SearchParams.searches.ToList<SearchProperties>().Where(s => s.seid == seid);
            string sip = string.Empty;
            foreach (var value in list)
            {
                if (value.device == "desktop_chrome")
                {
                    seresults = GetTop100Desktop(keyword, seid, value.domain, value.locale, value.uule);
                }
                else if (value.device == "mobile")
                {
                    seresults = GetTop100Mobile(keyword, seid, value.domain, value.locale, value.uule);
                }
            }
            ip = sip;
            return seresults;
        }
    }
}
