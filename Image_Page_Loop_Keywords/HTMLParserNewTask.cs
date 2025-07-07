using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Threading;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;


namespace Image_Page_Loop_Keywords
{
    class HTMLParserNewTask
    {
        string htmlsource = string.Empty;
        public string ImagesPatternDesktop74(string seid, string keyword, HtmlDocument doc, string urlType,out int count)
        {
            string ULinks = string.Empty;
            count = 0;
            try
            {
                htmlsource = doc.DocumentNode.OuterHtml;
                ArrayList alDup = new ArrayList();
                HtmlNodeCollection node = doc.DocumentNode.SelectNodes(".//div[@class='juwGPd BwPElf OCzgxd']/a[@class='EZAeBe']");
                if (node != null)
                {
                    if (urlType == "PageLinks")
                    {
                        foreach (HtmlNode links in node)
                        {
                            try
                            {
                                string url = links.Attributes["href"].Value.Replace("/url?q=", "");

                                if (url.StartsWith("http") || url.StartsWith("https"))
                                {
                                    int indx = url.LastIndexOf("http://");
                                    if (indx < 0)
                                    {
                                        indx = url.LastIndexOf("https://");
                                    }
                                    url = url.Remove(0, indx);
                                    if (url.Contains("&amp;sa="))
                                        url = url.Remove(url.IndexOf("&amp;sa="));
                                    if (url.Contains("&sa="))
                                        url = url.Remove(url.IndexOf("&sa="));
                                    ULinks = HttpUtility.HtmlDecode(url);
                                }
                            }
                            catch { continue; }
                        }
                    }
                }
            }
            catch { }
            return ULinks;

        }
        public string ImagesPatternDesktop401(string seid, string keyword, HtmlDocument doc, string urlType, out int count)
        {
            string ULinks = string.Empty;
            count = 0;
            try
            {
                htmlsource = doc.DocumentNode.OuterHtml;
                ArrayList alDup = new ArrayList();
                string pattern1 = @"\],\[""(.*?).jpg"",";

                Regex rx = new Regex(pattern1, RegexOptions.IgnoreCase);
                MatchCollection mc = rx.Matches(htmlsource);

                foreach (Match m in mc)
                {
                    string url = "http" + m.Groups[1].Value;
                    int indx = url.LastIndexOf("http://");
                    if (indx < 0)
                    {
                        indx = url.LastIndexOf("https://");
                    }
                    url = url.Remove(0, indx);
                    if (url.StartsWith("http") || url.StartsWith("https"))
                    {
                        if (!url.Contains("htm") || !url.Contains(",null"))
                            if (!url.Contains("jpg"))
                            {
                                url += ".jpg";
                            }
                        ULinks = HttpUtility.HtmlDecode(url);
                    }
                }
            }
            catch { }
            return ULinks;
        }

        public string ImagesPatternMobile382(string seid, string keyword, HtmlDocument doc, string urlType, out int count)
        {
            string ULinks = string.Empty;
            count = 0;
            try
            {
                htmlsource = doc.DocumentNode.OuterHtml;
                ArrayList alDup = new ArrayList();
                HtmlNodeCollection node = doc.DocumentNode.SelectNodes(".//div[@class='kb0PBd cvP2Ce']/a[@class='LBcIee']");
                if (node != null)
                {
                    if (urlType == "PageLinks")
                    {
                        foreach (HtmlNode links in node)
                        {
                            try
                            {
                                string url = links.Attributes["href"].Value.Replace("/url?q=", "");

                                if (url.StartsWith("http") || url.StartsWith("https"))
                                {
                                    int indx = url.LastIndexOf("http://");
                                    if (indx < 0)
                                    {
                                        indx = url.LastIndexOf("https://");
                                    }
                                    url = url.Remove(0, indx);
                                    if (url.Contains("&amp;sa="))
                                        url = url.Remove(url.IndexOf("&amp;sa="));
                                    if (url.Contains("&sa="))
                                        url = url.Remove(url.IndexOf("&sa="));
                                    ULinks = HttpUtility.HtmlDecode(url);
                                }
                            }
                            catch { continue; }
                        }
                    }
                }
            }
            catch { }
            return ULinks;
        }

        public string ImagesPatternMobile402(string seid, string keyword, HtmlDocument doc, string urlType, out int count)
        {
            string ULinks = string.Empty;
            count = 0;
            try
            {
                htmlsource = doc.DocumentNode.OuterHtml;
                ArrayList alDup = new ArrayList();
                string pattern1 = @"\],\[""(.*?).jpg"",";

                Regex rx = new Regex(pattern1, RegexOptions.IgnoreCase);
                MatchCollection mc = rx.Matches(htmlsource);

                foreach (Match m in mc)
                {
                    string url = "http" + m.Groups[1].Value;
                    int indx = url.LastIndexOf("http://");
                    if (indx < 0)
                    {
                        indx = url.LastIndexOf("https://");
                    }
                    url = url.Remove(0, indx);
                    if (url.StartsWith("http") || url.StartsWith("https"))
                    {
                        if (!url.Contains("htm") || !url.Contains(",null"))
                            if (!url.Contains("jpg"))
                            {
                                url += ".jpg";
                            }
                        ULinks = HttpUtility.HtmlDecode(url);
                    }
                }
            }
            catch { }
            return ULinks;
        }

    }
}

