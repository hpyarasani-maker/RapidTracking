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
        public string ImagesPatternDesktop(string seid, string keyword, HtmlDocument doc, string urlType,out int count)
        {
            string ULinks = string.Empty;
            count = 0;
            try
            {
                htmlsource =doc.DocumentNode.OuterHtml;
                ArrayList alDup = new ArrayList();

                //HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//*[@id=\"rg_s\"]/div/div");//Mw2I7 UkaFJe
                HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//div[@class=\"rg_meta notranslate\"]");
                //HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//div[@class=\"Mw2I7 UkaFJe\"]");

                if (node != null)
                {
                    foreach (HtmlNode links in node)
                    {
                        try
                        {
                            string url = "";

                            if (urlType == "ImageLinks")

                                url = JObject.Parse(links.InnerText)["ou"].Value<string>();
                            else
                                url = JObject.Parse(links.InnerText)["ru"].Value<string>();

                            if (url.StartsWith("http") || url.StartsWith("https"))
                            {
                                int indx = url.LastIndexOf("http://");
                                if (indx < 0)
                                {
                                    indx = url.LastIndexOf("https://");
                                }
                                url = url.Remove(0, indx);
                                ULinks = HttpUtility.HtmlDecode(url);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                else
                {
                    if (urlType == "PageLinks")
                    {
                        string pattern1 = "<table class=\\WIkMU6e\\W><tr><td><a href=(.*?)&";
                        //string pattern = @"\]n,\[""http(.*?)\"",";
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
                            ULinks = HttpUtility.HtmlDecode(url);
                        }
                    }

                    else  // 74
                    {
                        string pattern = "x22 targetx3dx22_blankx22 hrefx3dx22(.*?)x22 ";
                        Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection mc = rx.Matches(htmlsource);
                        foreach (Match m in mc)
                        {
                            string url = m.Groups[1].Value;

                            if (url.StartsWith("http") || url.StartsWith("https"))
                            {
                                int indx = url.LastIndexOf("http://");
                                if (indx < 0)
                                {
                                    indx = url.LastIndexOf("https://");
                                }
                                url = url.Remove(0, indx);
                                ULinks = HttpUtility.HtmlDecode(url);
                            }
                        }
                    }
                }

                //foreach (string s in alDup)
                //{
                //    if (googleList.Contains(s) || string.IsNullOrEmpty(s)) continue;
                //    googleList.Add(s);
                //}

                //if (googleList.Count > 100)
                //{
                //    googleList.RemoveRange(100, googleList.Count - 100);
                //}
            }
            catch (Exception ex)
            {
                throw new Exception("No pattern match,  " + ex.Message);
            }

            return ULinks;
        }

        public string ImagesPatternMobile(string seid, string keyword, HtmlDocument doc, string urlType, out int count)
        {
            string ULinks = string.Empty;
            count = 0;
            try
            {
                htmlsource = doc.DocumentNode.OuterHtml;
                HtmlNodeCollection node;
                if (urlType == "ImageLinks")
                {
                    node = doc.DocumentNode.SelectNodes("//a[@jsname=\"m8x3S\"]/img");
                    if (node == null)
                        node = doc.DocumentNode.SelectNodes("//div[@class=\"rg_meta notranslate\"]");
                }
                else
                {
                    node = doc.DocumentNode.SelectNodes("//a[@class=\"VFACy kGQAp\"]");
                    if (node == null)
                        node = doc.DocumentNode.SelectNodes("//a[@class=\"VFACy\"]");
                    //*03-04-2020
                    if (node == null)
                        node = doc.DocumentNode.SelectNodes("//div[@class=\"rg_meta notranslate\"]");//*03-04-2020  

                }
                if (node != null)
                {
                    foreach (HtmlNode links in node)
                    {
                        try
                        {
                            string url = "";

                            if (urlType == "ImageLinks")
                            {
                                try
                                {
                                    url = links.Attributes["data-iurl"].Value;
                                }
                                catch
                                {
                                    try
                                    {
                                        url = links.Attributes["data-src"].Value;
                                    }
                                    catch
                                    {
                                        url = JObject.Parse(links.InnerText)["ou"].Value<string>();
                                    }

                                }
                            }
                            //*03-04-2020
                            else if (urlType == "PageLinks")
                            {
                                try
                                {
                                    url = JObject.Parse(links.InnerText)["ru"].Value<string>();
                                }
                                catch
                                {
                                    url = links.Attributes["href"].Value;
                                }
                            }//*03-04-2020
                            else
                                url = links.Attributes["href"].Value;

                            if (url.StartsWith("http") || url.StartsWith("https"))
                            {
                                int indx = url.LastIndexOf("http://");
                                if (indx < 0)
                                {
                                    indx = url.LastIndexOf("https://");
                                }
                                url = url.Remove(0, indx);
                                ULinks = HttpUtility.HtmlDecode(url);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    if (urlType == "ImageLinks")
                    {
                        string pattern = @"\]n,\[""http(.*?)\"",";
                        Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
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
                            ULinks = HttpUtility.HtmlDecode(url);
                        }
                    }

                    else
                    {
                        string pattern = "x22 targetx3dx22_blankx22 hrefx3dx22(.*?)x22 ";
                        Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection mc = rx.Matches(htmlsource);
                        foreach (Match m in mc)
                        {
                            string url = m.Groups[1].Value;

                            if (url.StartsWith("http") || url.StartsWith("https"))
                            {
                                int indx = url.LastIndexOf("http://");
                                if (indx < 0)
                                {
                                    indx = url.LastIndexOf("https://");
                                }
                                url = url.Remove(0, indx);
                                ULinks = HttpUtility.HtmlDecode(url);
                            }
                        }
                    }
                }

                //foreach (string s in alDup)
                //{
                //    if (googleList.Contains(s) || string.IsNullOrEmpty(s)) continue;
                //    googleList.Add(s);
                //}

                //if (googleList.Count > 100)
                //{
                //    googleList.RemoveRange(100, googleList.Count - 100);
                //}
            }
            catch (Exception ex)
            {
                throw new Exception("No pattern match,  " + ex.Message);
            }

            return ULinks;
        }

    }
}

