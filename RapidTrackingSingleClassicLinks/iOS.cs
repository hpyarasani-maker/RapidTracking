using HtmlAgilityPack;
using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RapidTrackingSingleClassicLinks
{
    class iOS
    {
        int orgLinks;

        public string ProcessDocument(string seid, string keyword, HtmlDocument doc, out int count)
        {
            count = 0;
            orgLinks = 0;
            if (doc == null) throw new Exception("No source found.");

            StringBuilder sb = new StringBuilder();
            sb.Append("<searchResult searchEngine=\"" + seid + "\" keyword=\"" + WebUtility.HtmlEncode(keyword) + "\" date=\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\" >");
            try  //28-09-2020  try catch.
            {
                sb.Append("<section col=\"main\">");

                string links = GetMobileLinks(doc);
                sb.Append(links);

                sb.Append("</section>");

                sb.Append("<section col=\"right\">");

                sb.Append("</section>");
                sb.Append("</searchResult>");

                if (links.Length > 0)
                {
                    count = orgLinks;
                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return string.Empty;

        }

        private string GetMobileLinks(HtmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                ArrayList alDup = new ArrayList();
                ArrayList googleList = new ArrayList();
                HtmlNodeCollection node = doc.DocumentNode.SelectNodes(".//div[@class='ZINbbc xpd']/div/a|.//div[@class='ZINbbc xpd']/div[1]/a|.//a[@class='C8nzq JTuIPc amp_r']|.//a[@class='C8nzq JTuIPc']|.//a[@class='C8nzq BmP5tf amp_r']|.//a[@class='C8nzq Tj0U2 BmP5tf']|.//a[@class='C8nzq BmP5tf']|.//a[@class='C8nzq Tj0U2 BmP5tf amp_r']|.//a[@class='sXtWJb amp_r']|.//g-link/a|.//div[@class='fM8c FUksre']/a|.//div[@class='ytwLQd']|.//div[@class='rc']|.//h3[@class='r']/a|.//h3[@class='r']/div/a|.//h3[contains(@class,'yuRUbf JtG40d')]/a|.//a[@class='cz3goc BmP5tf']");  //cz3goc BmP5tf

                foreach (HtmlNode links in node)
                {
                    try
                    {
                        string url = links.Attributes["href"].Value;
                        string title = links.SelectSingleNode(".//div[@role='heading']")?.InnerText;

                        url = url.Replace("HTTPS://", "https://").Replace("HTTP://", "http://");
                        //if (string.IsNullOrEmpty(url.Trim())) return string.Empty;
                        //29-09-2020            
                        if (url.IndexOf("https://") == 0 || url.IndexOf("https://") >= 0) //01-10-2020
                            url = url.Remove(0, url.IndexOf("https://"));
                        else if (url.IndexOf("http://") == 0 || url.IndexOf("http://") >= 0) //14-10-2020 included indexof for http)
                            url = url.Remove(0, url.IndexOf("http://"));
                        //end 29-09-2020

                        Regex rx = new Regex("http[\\w]?://(.*)", RegexOptions.Singleline);
                        if (!rx.Match(url).Success && !url.Contains("/aclk?"))
                            if (!url.Contains("://")) // 30-04-2020
                                url = "http://" + url;

                        if (url.StartsWith("http:////") || url.StartsWith("https:////")) //18-09-2020 condition applied if appears http:////
                            url = url.Replace("////", "//").Replace("///", "//"); //18-09-2020

                        if (url.Contains("&amp;grqid="))
                            url = url.Remove(url.IndexOf("&amp;grqid="));
                        //13-12-2019
                        if (url.Contains("&grqid="))
                            url = url.Remove(url.IndexOf("&grqid="));
                        //23-09-2020
                        if (url.Contains("&amp;gclid="))
                            url = url.Remove(url.IndexOf("&amp;gclid="));
                        if (url.Contains("&gclid="))
                            url = url.Remove(url.IndexOf("&gclid="));
                        //end 23-09-2020

                        if (url.Contains("\0"))
                            url = url.Replace("\0", "%00");

                        if ((url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("ftp://")) && (!url.Contains("/aclk?") && !url.Contains("///search?") && !url.Contains("search?num=100") && !url.Contains("?sa=X") && !url.Contains("http://#") && !url.Contains("www.google.") && !url.Contains("maps.google.") && !url.Contains("https://www.google.com/maps")))
                            if (url.StartsWith("http") || url.StartsWith("https") || !url.Contains("https://www.google.com/maps"))
                            {
                                url = GetRedirectedUrl(WebUtility.HtmlDecode(url).Trim());
                                if (url.ToLower().Contains("%2f") || url.ToLower().Contains("%2e"))
                                    url = GetRedirectedUrl(WebUtility.UrlDecode(WebUtility.HtmlDecode(url)).Trim());

                                string[] item = { WebUtility.HtmlDecode(url.Replace("\x00", "%00").Replace("|", "%7C").Replace("^", "%5E").Replace("\\\\u003d", "=").Replace('\u0002', ' ').Replace('\u0018', ' ').Replace('\f', ' ').Trim()) ,
                                                   title };
                                alDup.Add(item);

                            }
                    }
                    catch { continue; }
                }

                foreach (string[] s in alDup)
                {
                    if (googleList.Contains(s[0]) || string.IsNullOrEmpty(s[0])) continue;
                    googleList.Add(s);
                }

                if (googleList.Count > 20)
                {
                    googleList.RemoveRange(20, googleList.Count - 20);
                }
                foreach (string[] s in googleList)
                {
                    sb.Append("<item url=\"" + s[0] + "\"  title=\"" + SetTitle(s[1]) + "\"  />");
                    orgLinks++;
                }
            }
            catch { }

            return sb.ToString();
        }

        private string GetRedirectedUrl(string url)
        {
            //21-11-2020
            url = url.Replace("HTTPS://", "https://").Replace("HTTP://", "http://");
            if (string.IsNullOrEmpty(url.Trim())) return string.Empty;

            if (url.LastIndexOf("https://") > 0)
                url = url.Remove(0, url.LastIndexOf("https://"));
            if (url.LastIndexOf("http://") > 0)
                url = url.Remove(0, url.LastIndexOf("http://"));

            Regex rx = new Regex("http[\\w]?://(.*)", RegexOptions.Singleline);
            if (!rx.Match(url).Success && !url.Contains("/aclk?"))
                if (!url.Contains("://"))
                    url = "http://" + url;

            if (url.Contains("&amp;grqid="))
                url = url.Remove(url.IndexOf("&amp;grqid="));

            //  13-12-2020
            if (url.Contains("&grqid="))
                url = url.Remove(url.IndexOf("&grqid="));

            if (url.Contains("\0"))
                url = url.Replace("\0", "%00");

            if ((url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("ftp://")) && (!url.Contains("/aclk?") && !url.Contains("search?num=100")))
                return url;

            return string.Empty;

        }

        // There are chances method was used for title contains in case any issues in xml applied decode/encode.
        public string SetTitle(string unicodestring)
        {
            //return WebUtility.HtmlEncode(WebUtility.HtmlDecode(unicodestring));
            return WebUtility.HtmlEncode(WebUtility.HtmlDecode(unicodestring)).Replace("\\x27", "'").Replace("\\\\u0026", "&amp;").Replace("\\\\\\x22", "&quot;").Replace("\\u2013", "–"); //17-03-2022

        }

    }
}