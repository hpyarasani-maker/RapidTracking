using HtmlAgilityPack;
using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


namespace RapidTrackingSingleClassicLinks
{
    public class Desktop
    {
        int orgLinks;

        public string ProcessDocument(string seid, string keyword, HtmlDocument doc, out int count)
        {
            count = 0;
            if (doc == null) throw new Exception("No source found.");

            orgLinks = 0;
            try //28-09-2020  try catch.
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<searchResult searchEngine=\"" + seid + "\" keyword=\"" + WebUtility.HtmlEncode(keyword) + "\" date=\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\" >");
                sb.Append("<section col=\"main\">");

                string links = GetDesktopLinks(doc);
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

        public string GetDesktopLinks(HtmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                ArrayList alDup = new ArrayList();
                ArrayList googleList = new ArrayList();
                HtmlNodeCollection node = doc.DocumentNode.SelectNodes(".//h3[@class='r']/a|.//div[@class='r']/a|.//div[@class='r']/div/a|.//div[@class='yuRUbf']/div/span/a|.//g-link/a|.//h3[@class='r dO0Ag']/a|.//div[@class='yuRUbf']/a|.//div[@class='zTpPx']/g-link/a");

                foreach (HtmlNode links in node)
                {
                    try
                    {
                        string url = links.Attributes["href"].Value;
                        string title = links.SelectSingleNode(".//h3")?.InnerText;

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

                        if ((url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("ftp://")) && (!url.Contains("/aclk?") && !url.Contains("///search?") && !url.Contains("search?num=100") && !url.Contains("?sa=X")))
                            if (url.StartsWith("http") || url.StartsWith("https"))
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
                    googleList.RemoveRange(20, googleList.Count - 20);


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