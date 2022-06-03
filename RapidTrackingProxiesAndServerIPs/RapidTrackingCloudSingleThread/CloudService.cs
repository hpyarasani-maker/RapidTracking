using HtmlAgilityPack;
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
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace RapidTrackingCloudSingleThread
{
    class CloudService
    {
        SourceService.Service1Client cs;

        readonly string strConn = string.Empty;
        string url = string.Empty;

        public CloudService()
        {
            strConn = Common.ReadConnection();
        }
        
        public string[] GetTop100Desktop(string keyword, int seid)
        {
            cs = new SourceService.Service1Client();
            string HTML = cs.GetGoogleSource(seid.ToString(), keyword);
            File.WriteAllText(@"c:\inetpub\wwwroot\html\cartyres.html", HTML);
            string[] dr = DesktoppatternTrending(System.Net.WebUtility.HtmlDecode(HTML), keyword, seid.ToString());
            return dr;
        }
        //----------------------------------------------- For Non Hotel Keywords -------------------------------------//
         public string[] GetTop100Mobile(string keyword, int seid)
         {
            cs = new SourceService.Service1Client();
            string HTML = cs.GetGoogleSource(seid.ToString(),keyword);
            File.WriteAllText(@"c:\inetpub\wwwroot\html\cartyres.html", HTML);
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
        public string[] GetTop100(string keyword, int seid)
        {
            string[] seresults = new string[1];

            // IEnumerable<SearchProperties> list = SearchParams.searches.ToList<SearchProperties>().Where(s => s.seid == seid);
            SearchProperties sp = SearchParams.searches.Where(s => s.seid == seid).SingleOrDefault();
            string device = sp.device;
            
                if (device == "desktop")
                {
                    seresults = GetTop100Desktop(keyword, seid);
                }
                else 
                {
                    seresults = GetTop100Mobile(keyword, seid);
                }
            return seresults;
        }
    }
}
