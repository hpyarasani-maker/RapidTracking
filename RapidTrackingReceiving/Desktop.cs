using HtmlAgilityPack;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Oxylabs_BulkKeywords
{
    public class Desktop
    {
        int orgLinks;
        string html;

        public string ProcessDocument(string seid, string keyword, string htmlsource, out int organicurls)
        {
            if (string.IsNullOrEmpty(htmlsource))
            {
                organicurls = 0;
                return string.Empty;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlsource);

            HtmlNode htmlNode = doc.DocumentNode.SelectSingleNode("//table[@id='mn']");
            if (htmlNode != null)
            {
                organicurls = 0;
                throw new Exception("Old page found.");
            }

            html = htmlsource;
            orgLinks = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("<searchResult searchEngine=\"" + seid + "\" keyword=\"" + WebUtility.HtmlEncode(keyword) + "\" date=\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\" >");
            sb.Append("<section col=\"main\">");
            string topStuff = GetTopStuff(doc);
            sb.Append(topStuff);

            HtmlNodeCollection nodeCol = doc.DocumentNode.SelectNodes("//div[@class='_NId']");
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@class='bkWMgd']");
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@id='ires']/ol/div");
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div|//div[@id='rso']/g-section-with-header");//13-03-2020  //01-05-2020         

            if (nodeCol == null)
            {
                organicurls = 0;
                return string.Empty;
            }

            string ndText = "";

            foreach (HtmlNode node in nodeCol)
            {
                if (node.HasClass("kp-wholepage"))
                {
                    continue;
                }
                try
                {
                    if (node.InnerHtml != "")
                    {
                        string s = ProcessNode(node);
                        ndText += s;
                        if (s.Length > 0)
                            sb.Append(s);
                    }
                }
                catch { }
            }
            // 23-03-2020
            if (string.IsNullOrEmpty(ndText) || orgLinks == 0)//08-04-2020
            {
                nodeCol = doc.DocumentNode.SelectNodes("//div[@class='xVtsMb i6u2Cc']|//div[@class='xVtsMb']/div/div");//swapped 08-04-2020
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@class='vC5Ym DhKAUb']/div");  // 03-04-2020
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='kp-wp-tab-overview']/div|.//div[@class='WvKfwe']/div");//14-10-2020 updated selector classic links //15-04-2020
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@class='WvKfwe a3spGf']/div");  // 15-04-2020
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@class='WvKfwe a3spGf']/div|//div[@class='WvKfwe a3spGf']/g-section-with-header");  // 15-04-2020    //01-05-2020");  // 15-04-2020//22-05-2020
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@class='a3spGf WvKfwe']/div|//div[@class='a3spGf WvKfwe']/g-section-with-header|.//div[@class='UDZeY OTFaAf']");  // 01-06-2020
                foreach (HtmlNode node in nodeCol)
                {
                    try
                    {
                        if (node.InnerHtml != "")
                        {
                            string s = ProcessNode(node);
                            ndText += s;
                            if (s.Length > 0)
                                sb.Append(s);
                        }
                    }
                    catch { }
                }
            }
            // 23-03-2020

            //if (orgLinks < count)
            //    return string.Empty;

            string bottomStuff = GetBottomStuff(doc);
            sb.Append(bottomStuff);
            sb.Append("</section>");

            sb.Append("<section col=\"right\">");
            string rightStuff = GetRightStuff(doc);
            sb.Append(rightStuff);
            sb.Append("</section>");
            sb.Append("</searchResult>");

            if (ndText.Length <= 0)
            {
                organicurls = 0;
                return string.Empty;
            }
            organicurls = orgLinks;
            return sb.ToString();


        }

        private string GetRightStuff(HtmlDocument doc)
        {
            StringBuilder s = new StringBuilder();

            HtmlNode rcNode = doc.DocumentNode.SelectSingleNode("//div[@id='rhs_block']");
            if (rcNode == null)
                rcNode = doc.DocumentNode.SelectSingleNode("//div[@id='rhs']"); // 18-11-2019
            if (rcNode == null)
                rcNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'rhscol col')]"); // 21-04-2020
            if (rcNode == null)
                return string.Empty;

            // product listed ads
            HtmlNode sNode = rcNode.SelectSingleNode(".//div[@class='cu-container']");
            if (sNode != null)
            {
                if (!sNode.InnerHtml.Contains("iKidV"))//22-10-2019
                {
                    s.Append("<block type=\"productListedAds\" url=\"\">");
                    HtmlNodeCollection cl = sNode.SelectNodes(".//a[@class='plantl pla-unit-title-link']");
                    if (cl == null)
                        cl = sNode.SelectNodes(".//a[@class='plantl pla-unit-single-clickable-target clickable-card']");
                    if (cl == null)
                        cl = sNode.SelectNodes(".//div[@class='mnr-c pla-unit']/a[2]");
                    if (cl == null)
                        cl = sNode.SelectNodes(".//div[@class='pla-unit-title']/a");
                    if (cl == null)
                        cl = sNode.SelectNodes(".//div[@class='twpSFc mnr-c']/a[2]");
                    if (cl != null)
                    {
                        foreach (HtmlNode nd in cl)
                        {
                            // 18-11-2019
                            string title = nd.InnerText;
                            var url = nd.Attributes["href"].Value.Trim();
                            url = GetRedirectedUrl(url);
                            if (string.IsNullOrEmpty(title.Trim()))
                            {
                                HtmlNode nd1 = nd.SelectSingleNode(".//span[@class='rhsl4']");
                                if (nd1 != null)
                                    title = SetTitle(nd1.InnerText);
                            }
                            if (string.IsNullOrEmpty(title.Trim()))
                            {
                                title = nd.Attributes["aria-label"]?.Value;
                            }
                            s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");

                        }
                    }
                    s.Append("</block>");
                }
            }

            // kp
            HtmlNode node = rcNode.SelectSingleNode(".//div[@class='kp-header']");
            if (node == null)
                node = rcNode.SelectSingleNode(".//div[@class='rhsvw explore-xpanels-desktop__xpanel-wrapper']");
            if (node == null)
                node = rcNode.SelectSingleNode(".//div[@class='iKidV']");//22-10-2019
            if (node == null)
                node = rcNode.SelectSingleNode(".//div[@class='kp-wholepage EyBRub kp-wholepage-osrp HSryR']");  // 06-11-2019
            if (node == null)
                node = rcNode.SelectSingleNode(".//div[contains(@class,'kp-wholepage kp-wholepage-osrp')]");  // 11-05-2020 //16-10-2020 kp block in contains functions
            if (node == null)
                node = rcNode.SelectSingleNode(".//div[@class='Y37F6d Nn2Stf']");  // 21-04-2020
            if (node == null)
                node = rcNode.SelectSingleNode(".//div[@class='UDZeY fAgajc OTFaAf']");  // 27-05-2020
            if (node == null)
                node = rcNode.SelectSingleNode(".//div[@class='NFQFxe mod']");  // 01-06-2020

            if (node != null)     //'kp-blk knowledge-panel _Rqb _RJe']") != null) //|.//div[@role='heading']/div[1]/span
            {
                s.Append("<block type=\"knowledgeGraph\" url=\"\" />");
            }

            return s.ToString();
        }

        private string GetBottomStuff(HtmlDocument doc)
        {
            StringBuilder s = new StringBuilder();

            // Ads
            HtmlNode colb = doc.DocumentNode.SelectSingleNode("//div[@id='bottomads']");   //20-01-2020
            if (colb != null)
            {
                HtmlNodeCollection col = colb.SelectNodes(".//div[@id='tadsb']/ol/li|.//div[@id='tads']/div[@class='uEierd']|.//div[@id='tadsb']/div[@class='uEierd']"); //24-09-2020 bottom adwords//21-09-2020 updated bottom adwords   //20-01-2020
                if (col == null)
                    col = colb.SelectNodes(".//div[@id='tadsb']/div/ol/li");   //16-04-2020
                if (col == null) return s.ToString();   //20-01-2020

                s.Append("<block type=\"adwords\" url=\"\">");
                foreach (HtmlNode nd in col)
                {
                    HtmlNode n = nd.SelectSingleNode(".//h3/a[2]|.//div[@class='ad_cclk']/a[2]|.//div[@class='d5oMvf']/a"); //27-06-2020
                    if (n != null)
                    {
                        HtmlNode tittlenode = n.SelectSingleNode(".//h3|.//div[@role='heading']");//27-06-2020
                                                                                                  //25-08-2020 commented
                                                                                                  /* if (!n.Attributes["href"].Value.StartsWith("/"))
                                                                                                       s.Append("<item url=\"" + SetUrl(n.Attributes["href"].Value) + "\" title=\"" + SetTitle(tittlenode.InnerText) + "\" />");
                                                                                                   else
                                                                                                   {
                                                                                                       string title = tittlenode.InnerText;
                                                                                                       n = nd.SelectSingleNode(".//div[@class='ads-visurl']/cite");
                                                                                                       if (n != null)
                                                                                                           s.Append("<item url=\"" + SetUrl(n.InnerText) + "\" title=\"" + SetTitle(title) + "\" />");
                                                                                                   }*/
                                                                                                  //25-08-2020

                        try  //28-09-2020  try catch.
                        {
                            string url = string.Empty;
                            string title = tittlenode.InnerText;

                            //27-08-2020
                            if (!string.IsNullOrEmpty(GetRedirectedUrl_TextAds(n.Attributes["href"]?.Value)))
                            {
                                url = GetRedirectedUrl_TextAds(n.Attributes["href"].Value);
                            }
                            else if (!string.IsNullOrEmpty(GetRedirectedUrl_TextAds(n.Attributes["data-rw"]?.Value)))
                            {
                                url = GetRedirectedUrl_TextAds(n.Attributes["data-rw"].Value);
                            }
                            else if (!string.IsNullOrEmpty(GetRedirectedUrl_TextAds(n.Attributes["data-pcu"]?.Value)))
                            {
                                url = GetRedirectedUrl_TextAds(n.Attributes["data-pcu"].Value);
                            }
                            else
                            {
                                HtmlNode n1 = nd.SelectSingleNode(".//div[@class='ads-visurl']/cite");
                                if (n1 != null)
                                    url = GetRedirectedUrl_TextAds(n1.InnerText);

                                if (string.IsNullOrEmpty(url))
                                    url = GetRedirectedUrl_TextAds(n.Attributes["href"]?.Value);
                            }
                            if (!string.IsNullOrEmpty(url))
                                s.Append("<item url=\"" + url + "\" title=\"" + SetTitle(title) + "\" />");
                            // end 27-08-2020
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                s.Append("</block>");
            }

            return s.ToString();
        }

        private string GetProductListedAds(HtmlDocument doc)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode pla = doc.DocumentNode.SelectSingleNode("//div[@class='cu-container']");
            if (pla != null)
            {
                HtmlNode h3 = pla.SelectSingleNode(".//div[@class='_tz']/h3");
                if (h3 != null)
                {
                    if (h3.InnerText.StartsWith("Shop for") || h3.InnerText.StartsWith("See ") || h3.InnerText.StartsWith("zwarte "))
                    {
                        s.Append("<block type=\"productListedAds\" url=\"\">");

                        HtmlNodeCollection cl = pla.SelectNodes(".//a[@class='plantl pla-unit-title-link']");
                        if (cl == null)
                            cl = pla.SelectNodes(".//a[@class='plantl pla-unit-single-clickable-target clickable-card']");
                        if (cl == null)
                            cl = pla.SelectNodes(".//div[@class='mnr-c pla-unit']/a[2]");
                        if (cl == null)
                            cl = pla.SelectNodes(".//div[@class='pla-unit-title']/a");
                        if (cl != null)
                        {
                            foreach (HtmlNode nd in cl)
                            {
                                var url = nd.Attributes["href"].Value.Trim();
                                url = GetRedirectedUrl(url);
                                s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(nd.InnerText) + "\" />");
                            }
                        }
                        s.Append("</block>");
                    }
                }
            }

            return s.ToString();
        }

        private string GetTopStuff(HtmlDocument doc)
        {
            StringBuilder s = new StringBuilder();

            // for carousel
            HtmlNode crNode = doc.DocumentNode.SelectSingleNode("//div[@id='extabar']|//div[@id='appbar']");  //29-06-2020
            if (crNode != null)
            {
                if (crNode.SelectSingleNode(".//div[@id='kx']") != null || crNode.SelectSingleNode(".//g-scrolling-carousel") != null //|| crNode.SelectSingleNode(".//div[@role='heading']") != null) //29-06-2020
                     || crNode.SelectSingleNode(".//div[@jscontroller='envtD']") != null) //29-06-2020
                {
                    s.Append("<block type=\"carousel\" url=\"\">");
                    //s.Append(GetCarousel(crNode));  // 23-10-2019
                    s.Append("</block>");
                }
            }

            // product listed ads
            HtmlNode pla = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'cu-container')]"); //25-09-2020 included contains for existing selector
            if (pla != null)
            {
                HtmlNode h3 = pla.SelectSingleNode(".//div[@class='dxR8gf']/h3");
                if (h3 != null)
                {
                    if (pla.SelectSingleNode(".//div[contains(@class, 'commercial-unit-desktop-top')]") != null
                        || pla.SelectSingleNode(".//div[contains(@class, 'top-pla-group-inner')]") != null)  // 13-02-2020 
                    {
                        s.Append("<block type=\"productListedAds\" url=\"\">");

                        HtmlNodeCollection cl = pla.SelectNodes(".//a[@class='plantl pla-unit-title-link']");
                        if (cl == null)
                            cl = pla.SelectNodes(".//a[@class='plantl pla-unit-single-clickable-target clickable-card']");
                        if (cl == null)
                            cl = pla.SelectNodes(".//div[@class='mnr-c pla-unit']/a[2]");
                        if (cl == null)
                            cl = pla.SelectNodes(".//div[@class='pla-unit-title']/a");
                        if (cl != null)
                        {
                            foreach (HtmlNode nd in cl)
                            {
                                var url = nd.Attributes["href"].Value;
                                url = GetRedirectedUrl(url);
                                s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(nd.InnerText) + "\" />");
                            }
                        }
                        s.Append("</block>");
                    }
                }
            }

            // text ads
            HtmlNode colt = doc.DocumentNode.SelectSingleNode("//div[@id='tvcap']");  //20-01-2020
            if (colt != null)
            {
                HtmlNodeCollection col = colt.SelectNodes(".//div[@id='tads']/ol/li|.//div[@id='tads']/div/ol/li|.//div[@id='tadsb']/ol/li|.//div[@id='tads']/div[@class='uEierd']"); //21-09-2020 adwords selector//20-01-2020 //08-04-2020

                if (col != null) //return s.ToString();  //20-01-2020
                {
                    s.Append("<block type=\"adwords\" url=\"\">");
                    foreach (HtmlNode nd in col)
                    {
                        //HtmlNode n = nd.SelectSingleNode(".//h3/a[2]");
                        HtmlNode n = nd.SelectSingleNode(".//div[@class='ad_cclk']/a[2]|.//div[contains(@class,'d5oMvf')]/a"); //29-08-2020 included contains fucntions //23-07-2020 included missing item urls selectors
                        if (n != null)
                        {
                            //25-08-2020 commented
                            /*if (!n.Attributes["href"].Value.StartsWith("/"))
                            {
                                HtmlNode title = n.SelectSingleNode(".//h3|.//div[@role='heading']");  //23-07-2020
                                s.Append("<item url=\"" + SetUrl(n.Attributes["href"].Value) + "\" title=\"" + SetTitle(title.InnerText) + "\" />");
                            }
                            else
                            {
                                string title = "";
                                HtmlNode titleNode = n.SelectSingleNode(".//h3");
                                if (titleNode != null)
                                    title = titleNode.InnerText;
                                else
                                    title = n.InnerText;
                                n = nd.SelectSingleNode(".//div[@class='ads-visurl']/cite");
                                if (n != null)
                                    s.Append("<item url=\"" + SetUrl(n.InnerText) + "\" title=\"" + SetTitle(title) + "\" />");
                            }*/
                            //25-08-2020
                            //25-08-2020
                            try  //28-09-2020  try catch.
                            {
                                string url = string.Empty;
                                //27-08-2020
                                HtmlNode titleNode = n.SelectSingleNode(".//h3|.//div[@role='heading']");
                                string title = titleNode != null ? titleNode.InnerText : n.InnerText;

                                if (!string.IsNullOrEmpty(GetRedirectedUrl_TextAds(n.Attributes["href"]?.Value)))
                                {
                                    url = GetRedirectedUrl_TextAds(n.Attributes["href"].Value);
                                }
                                else if (!string.IsNullOrEmpty(GetRedirectedUrl_TextAds(n.Attributes["data-rw"]?.Value)))
                                {
                                    url = GetRedirectedUrl_TextAds(n.Attributes["data-rw"].Value);
                                }
                                else if (!string.IsNullOrEmpty(GetRedirectedUrl_TextAds(n.Attributes["data-pcu"]?.Value)))
                                {
                                    url = GetRedirectedUrl_TextAds(n.Attributes["data-pcu"].Value);
                                }
                                else
                                {
                                    HtmlNode n1 = nd.SelectSingleNode(".//div[@class='ads-visurl']/cite");
                                    if (n1 != null)
                                        url = GetRedirectedUrl_TextAds(n1.InnerText);

                                    if (string.IsNullOrEmpty(url))
                                        url = GetRedirectedUrl_TextAds(n.Attributes["href"]?.Value);
                                }
                                if (!string.IsNullOrEmpty(url))
                                    s.Append("<item url=\"" + url + "\" title=\"" + SetTitle(title) + "\" />");
                                // end 27-08-2020
                            }
                            catch (Exception ex)
                            { throw ex; }
                        }
                    }
                    s.Append("</block>");
                }
            }

            //18-03-2020
            colt = doc.DocumentNode.SelectSingleNode("//div[@id='taw']");
            if (colt != null)
            {
                HtmlNode kg = colt.SelectSingleNode(".//div[@class='NFQFxe mod']");
                if (kg != null)
                {
                    s.Append("<block type=\"knowledgeGraph\" url=\"\" />");
                }

                HtmlNode ts = colt.SelectSingleNode(".//div[@class='rSr7Wd']");
                if (ts != null)
                {
                    s.Append("<block type=\"topStories\" url=\"\">");
                    s.Append(GetTopStories(ts));
                    s.Append("</block>");
                }

                HtmlNodeCollection map = colt.SelectNodes(".//g-tray-header[@class='XvZKZb ndEm3b']/div/span |.//div[@class='WuixVd']/a/div"); // 17-08-2020 included selector for map block
                if (map != null)
                {
                    foreach (var node in map)
                    {
                        if (node.InnerText == "Affected area" || node.SelectSingleNode(".//span/svg/path") != null) //17-08-2020 included selector for map block without innerText
                        {
                            s.Append("<block type=\"maps\" url=\"\"></block>");
                            break;
                        }
                    }
                }
            }
            //end of 18-03-2020

            return s.ToString();
        }

        private string ProcessNode(HtmlNode node)
        {
            try  //28-09-2020  try catch.
            {

                string sb = "";
                if (IsBlock(node))
                {
                    sb = ProcessBlock(node);
                }
                else if (IsOrganic(node))
                {
                    sb = ProcessOrganic(node);
                }
                return sb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string ProcessOrganic(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            if (node.HasClass("_NId") || node.HasClass("bkWMgd") || node.HasClass("srg")
                || node.HasClass("g") || node.SelectNodes(".//div[@class='g']") != null // 18-03-2020
                || node.SelectNodes(".//div[@class='g GjRtuc']") != null // 02-06-2020
                || node.SelectNodes(".//div[contains(@class,'g card-section')]") != null) //06-10-2020 classic link
            {
                HtmlNodeCollection nds = node.SelectNodes(".//div[@class='g']");
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='rc']");
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='gG0TJc']");  //29-05-2020
                if (nds != null)
                    foreach (HtmlNode nd in nds)
                    {
                        // 02-06-2020
                        if (nd.SelectSingleNode(".//table[@class='nrgt']") != null || node.SelectSingleNode(".//table[@class='jmjoTe']") != null) //22-08-2020 included dor site links
                        {
                            s.Append(GetSiteLinks(nd));
                            continue;
                        }

                        if (nd.SelectSingleNode(".//h3[@role='heading']") != null)
                        {
                            if (nd.SelectSingleNode(".//h3[@role='heading']").InnerText == "Videos")
                            {
                                s.Append("<block type=\"videos\" url=\"\">");
                                //get video urls;
                                s.Append(GetVideos(nd));
                                s.Append("</block>");
                                continue;
                            }
                        } // End 02-06-2020

                        HtmlNode title = null;  // 18-11-2019
                        HtmlNode n = nd.SelectSingleNode(".//h3[@class='r']/a");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='r']/a");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='r']/div/a"); //28-05-2020
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='yuRUbf']/a"); //02-10-2020
                        if (n != null)
                            title = n.SelectSingleNode(".//h3");


                        HtmlNode img = nd.SelectSingleNode(".//img");
                        if (img != null)
                        {
                            if (Regex.IsMatch(nd.OuterHtml, "id=\"vidthumb\\d*\"")) // 17-01-2020
                            {
                                var urls = n.Attributes["href"].Value;
                                if (urls.StartsWith("http") || urls.StartsWith("https") || urls.StartsWith("ftp")) //30-04-2020
                                {
                                    //25-09-2020 commented
                                    /*int indx = urls.IndexOf("http://");//25-09-2020 LastIndexOf changed to IndexOf
                                    if (indx < 0)
                                    {
                                        indx = urls.IndexOf("https://");//25-09-2020 LastIndexOf changed to IndexOf
                                    }
                                    if (indx < 0)
                                    {
                                        indx = urls.LastIndexOf("ftp://");  //30-04-2020
                                    }
                                    urls = urls.Remove(0, indx);*/
                                    //end 25-09-2020
                                    // video block.
                                    s.Append("<block type=\"video\" url=\"\">");
                                    s.Append("<item url=\"" + SetUrl(urls) + "\" title=\"" + SetTitle(title.InnerText) + "\" />");
                                    s.Append("</block>");
                                }
                            }

                            //Changes - Included else if condition which was missing.....
                            else if (orgLinks < 100)
                            {
                                if (n == null)
                                    n = nd.SelectSingleNode(".//div[@class='yuRUbf']/a"); //04-09-2020 included selector for classic links
                                if (n != null)
                                    title = n.SelectSingleNode(".//h3"); //04-09-2020 included selector for classic links
                                var urls = n.Attributes["href"].Value;
                                urls = SetUrl(urls);    // 20-12-2019
                                if (urls.StartsWith("http") || urls.StartsWith("https") || urls.StartsWith("ftp")) //30-04-2020
                                {
                                    //25-09-2020
                                    /*int indx = urls.IndexOf("http://");
                                    if (indx < 0)
                                    {
                                        indx = urls.IndexOf("https://");
                                    }
                                    if (indx < 0)
                                    {
                                        indx = urls.LastIndexOf("ftp://");  //30-04-2020
                                    }
                                    urls = urls.Remove(0, indx);*/
                                    //end 25-09-2020
                                    // string links1= HttpUtility.UrlDecode(urls);
                                    s.Append("<item url=\"" + SetUrl(urls) + "\"  title=\"" + SetTitle(title.InnerText) + "\"  />");
                                    orgLinks++;
                                }
                            }
                        }

                        else
                        {
                            if (orgLinks < 100)
                            {
                                if (n == null)
                                    n = nd.SelectSingleNode(".//g-link/a");
                                if (n == null)
                                    n = nd.SelectSingleNode(".//div[@class='r']/div/a");  // 28-05-2020 twitter class link included selector
                                if (n == null)
                                    n = nd.SelectSingleNode(".//h3[@class='r dO0Ag']/a");  //29-05-2020
                                if (n == null)
                                    n = nd.SelectSingleNode(".//div[@class='yuRUbf']/a"); //03-09-2020  included selector for classic links                         
                                if (n != null)
                                    title = n.SelectSingleNode(".//h3"); //03-09-2020 included selector for classic links
                                var urls = n.Attributes["href"].Value;
                                string t;
                                if (title != null)
                                    t = title.InnerText;
                                else
                                    t = n.InnerText;

                                urls = SetUrl(urls);    // 20-12-2019
                                if (urls.StartsWith("http") || urls.StartsWith("https") || urls.StartsWith("ftp")) //30-04-2020
                                {
                                    //25-09-2020 commented
                                    /*int indx = urls.IndexOf("http://");
                                    if (indx < 0)
                                    {
                                        indx = urls.IndexOf("https://");
                                    }
                                    if (indx < 0)
                                    {
                                        indx = urls.LastIndexOf("ftp://");  //30-04-2020
                                    }
                                    urls = urls.Remove(0, indx);*/
                                    // end 25-09-2020
                                    // string links1= HttpUtility.UrlDecode(urls);
                                    s.Append("<item url=\"" + SetUrl(urls) + "\"  title=\"" + SetTitle(t) + "\"  />");
                                    orgLinks++;
                                }
                            }
                        }
                    }
            }
            else
            {
                HtmlNodeCollection col = node.SelectNodes(".//h3[@class='r']/a");
                if (col == null)
                    col = node.SelectNodes(".//div[@class='r']/a"); // 02-06-2020
                if (col == null)
                    col = node.SelectNodes(".//h3[@class='r dO0Ag']/a");  //29-05-2020
                if (col == null)
                    col = node.SelectNodes(".//div[@class='zTpPx']/g-link/a");  //28-05-2020
                foreach (HtmlNode nd in col)
                {
                    string u = nd.Attributes["href"].Value.Replace("/url?q=", "").Replace("&amp;", "&").Replace("&", "&#38;");

                    //u = nd.Attributes["href"].Value.StartsWith("http").ToString();
                    if (u.Contains("&sa="))
                        u = u.Substring(0, u.IndexOf("&sa="));
                    if (orgLinks < 100)
                    {
                        if (u.StartsWith("http") || u.StartsWith("https") || u.StartsWith("ftp")) //30-04-2020
                        {
                            //25-09-2020 commented
                            /*int indx = u.IndexOf("http://");
                            if (indx < 0)
                            {
                                indx = u.IndexOf("https://");
                            }
                            if (indx < 0)
                            {
                                indx = u.LastIndexOf("ftp://");  //30-04-2020
                            }
                            u = u.Remove(0, indx);*/
                            //25-09-2020
                            // string links1 = HttpUtility.UrlDecode(u);
                            s.Append("<item url=\"" + SetUrl(u) + "\"  title=\"" + SetTitle(nd.InnerText) + "\"  />");
                            orgLinks++;
                        }
                    }
                }
            }
            return s.ToString();
        }

        private string ProcessBlock(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            string blockType = GetBlockType(node);
            switch (blockType.ToLower())
            {
                case "topstories":
                    s.Append("<block type=\"topStories\" url=\"\">");
                    //get top stories urls;
                    s.Append(GetTopStories(node));
                    s.Append("</block>");
                    break;
                case "sitelinks":
                    //get site links;
                    s.Append(GetSiteLinks(node));
                    break;
                case "images":
                    s.Append("<block type=\"images\" url=\"\">");
                    //get image urls;
                    s.Append(GetImages(node));
                    s.Append("</block>");
                    break;
                case "maps":
                    s.Append("<block type=\"maps\" url=\"\"></block>");
                    break;
                case "twitters":
                    //get twitter urls;
                    s.Append(GetTwitterCards(node));
                    break;
                case "answercard":
                    s.Append("<block type=\"answerCard\" url=\"\">");
                    //get answer card urls;
                    s.Append(GetAnswerCard(node));
                    s.Append("</block>");
                    break;
                case "peoplealsoask":
                    s.Append("<block type=\"peopleAlsoAsk\" url=\"\">");
                    //get people also ask urls;
                    s.Append(PeopleAlsoAsk(node));
                    s.Append("</block>");
                    break;
                case "carousel":
                    //s.Append("<block type=\"carousel\" url=\"\"></block>");
                    s.Append("<block type=\"carousel\" url=\"\">");
                    s.Append(GetCarousel(node)); //11-05-2020 removed uncommented
                    s.Append("</block>");
                    break;
                case "finance":
                    s.Append("<block type=\"finance\" url=\"\"></block>");
                    break;
                case "event":
                    s.Append("<block type=\"eventResults\" url=\"\"></block>");
                    break;
                case "videocard":
                    s.Append(GetVideoCard(node));
                    break;
                case "videos":
                    s.Append("<block type=\"videos\" url=\"\">");
                    //get video urls;
                    s.Append(GetVideos(node));
                    s.Append("</block>");
                    break;
                case "knowledgepanel":
                    s.Append("<block type=\"knowledgeGraph\" url=\"\" />"); //05-10-2020
                    break;
                case "jobs":  //20-11-2020
                    s.Append(GetJobs(node));
                    break;
                default:
                    break;
            }
            return s.ToString();

        }


        //20-11-2020
        private string GetJobs(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();

            s.Append("<block type=\"jobs\" url=\"\">");

            HtmlNodeCollection nodes = node.SelectNodes(".//ul/li/div[@class='PwjeAc']");
            if (nodes != null)
            {
                foreach (HtmlNode nd in nodes)
                {
                    string url = string.Empty;
                    HtmlNode link = nd.SelectSingleNode(".//g-link/a");
                    if (link != null)
                    {
                        url = link.Attributes["href"].Value;
                        if (url.StartsWith("https://www.google.")) url = string.Empty;
                        url = SetUrl(url);
                    }

                    string title = nd.SelectSingleNode(".//div[@role='heading']").InnerText;
                    if (!string.IsNullOrEmpty(url) || !string.IsNullOrEmpty(title))
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                }
            }

            s.Append("</block>");

            return s.ToString();
        }
        //end 20-11-2020
        private string GetVideoCard(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();

            HtmlNode nd = node.SelectSingleNode(".//div[@class='_ELb']/a");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='twQ0Be']/a");
            if (nd != null)
                s.Append("<block type=\"videoCard\" url=\"" + SetUrl(nd.Attributes["href"].Value) + "\"></block>");

            return s.ToString();
        }

        private string GetVideos(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//g-inner-card/div/a");
            if (nds == null)
                nds = node.SelectNodes(".//div[@jsname='ibnC6b']/div/a");   //17-07-2020
            if (nds != null)
                foreach (HtmlNode nd in nds)
                {
                    try
                    {
                        string title = "";
                        HtmlNode n = nd.SelectSingleNode(".//div[@class='Igo7ld mRnBbe QgUve xIqs0b']");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='KiGY3d mB12kf JRhSae ZyAH8d']");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='wCIBKb']/div");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='CwxNSe']/div"); // 02-06-2020

                        try
                        {
                            title = n.InnerText;
                        }
                        catch { title = ""; }

                        string url = nd.Attributes["href"].Value.Trim();
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                    }
                    catch { }
                }

            return s.ToString();
        }

        private string GetSiteLinks(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode n = node.SelectSingleNode(".//div[@class='r']/a");
            if (n == null)
                n = node.SelectSingleNode(".//div[@class='yuRUbf']/a"); //03-09-2020 included classic link selector
            if (n != null)
            {
                if (orgLinks < 100)
                {
                    HtmlNode t = n.SelectSingleNode(".//h3");
                    s.Append("<item url=\"" + SetUrl(n.Attributes["href"].Value) + "\" title=\"" + SetTitle(t.InnerText) + "\" />");
                    orgLinks++;
                }
            }

            HtmlNodeCollection nds = node.SelectNodes(".//table[@class='nrgt']/tr|.//table[@class='jmjoTe']/tr"); //22-08-2020 include selector for sitelinks
            if (nds != null)
            {
                s.Append("<block type=\"siteLinks\" url=\"\">");
                foreach (HtmlNode nd in nds)
                {
                    HtmlNodeCollection c = nd.SelectNodes(".//h3[@class='r']/a|.//h3[@class='r t9dkOd']/a|.//h3[@class='r X2oHVb t9dkOd']/a"); //03-06-2020 //01-06-2020 updated selector for sitelinks
                    if (c == null) continue;
                    foreach (HtmlNode a in c)
                    {
                        s.Append("<item url=\"" + SetUrl(a.Attributes["href"].Value) + "\" title=\"" + SetTitle(a.InnerText) + "\" />");
                    }
                }
                s.Append("</block>");
            }
            return s.ToString();
        }

        private string PeopleAlsoAsk(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[@class='_eHi']/div"); // (".//h3[@class='r']/a");
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='psDd8d']/div");
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='DUeSlb']/div");
            if (nds == null)
                nds = node.SelectNodes(".//div[@jsname='xXq91c']");
            if (nds == null)
                return string.Empty;
            foreach (HtmlNode nd in nds)
            {
                s.Append("<item url=\"\" title=\"" + SetTitle(nd.InnerText) + "\" />");
            }
            return s.ToString();
        }

        private string GetAnswerCard(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[@class='r']/a");
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='yuRUbf']/a");  //03-09-2020 included selector for missing classic links
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='WcS13d']"); //removed /a //05-10-2020 included selector for missing classic links
            if (nds == null)
                return string.Empty;
            foreach (HtmlNode nd in nds)
            {
                //05-10-2020
                string title = "";
                HtmlNodeCollection nds1 = nd.SelectNodes(".//h3");
                if (nds1 != null)
                {
                    title = nd.SelectSingleNode(".//h3").InnerText;
                    s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"" + SetTitle(title) + "\" />");
                }
                nds1 = nd.SelectNodes(".//a");
                if (nds1 != null)
                    foreach (HtmlNode nd1 in nds1)
                    {
                        s.Append("<item url=\"" + SetUrl(nd1.Attributes["href"].Value) + "\" title=\"" + SetTitle(title) + "\" />");
                    }
                //end 05-10-2020
            }
            //05-10-2020 commented
            /*foreach (HtmlNode nd in nds)
            {
                //05-10-2020
                string title;
                if (nd.SelectSingleNode(".//h3") != null)
                    title = nd.SelectSingleNode(".//h3").InnerText;
                else
                    title = nd.InnerText;
                //end 05-10-2020
                s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"" + SetTitle(title) + "\" />");
            }*/
            //end 05-10-2020
            return s.ToString();
        }

        private string GetTwitterCards(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode hn = node.SelectSingleNode(".//h3[@class='r']/div/g-link/a");
            if (hn == null)
                hn = node.SelectSingleNode(".//h3/g-link/a");
            if (hn == null)
                hn = node.SelectSingleNode(".//g-link/a"); //included on 2019-06-24
            if (hn != null)
            {
                s.Append("<block type=\"twitterCards\" url=\"" + SetUrl(hn.Attributes["href"].Value) + "\">");

                HtmlNodeCollection nds = node.SelectNodes(".//g-inner-card/div/div[2]/div/g-link/a");
                if (nds == null)
                    nds = node.SelectNodes(".//g-inner-card/div/div[1]/a");
                if (nds == null)
                    nds = node.SelectNodes(".//g-inner-card/div/div/div[1]/a[1]");  //02-06-2020
                if (nds == null)
                    nds = node.SelectNodes(".//a[@class='h4kbcd']"); //27-05-2020 twitterCard item URLs included selector
                if (nds != null)
                    foreach (HtmlNode nd in nds)
                    {
                        s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"\" />");
                    }
                s.Append("</block>");
            }
            return s.ToString();
        }

        private string GetImages(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//g-img/img");
            bool existed = false;
            if (nds != null)
                foreach (HtmlNode nd in nds)
                {
                    if (nd.Attributes.Contains("title"))    // 24-10-2019
                    {
                        string url = nd.Attributes["title"].Value.Trim();
                        if (url.StartsWith("//www.")) url = "http:" + url;
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"\" />");
                        existed = true;
                    }
                }
            if (!existed)
            {
                string imgItems = GetImageURLs();
                s.Append(imgItems);
            }
            return s.ToString();
        }

        private string GetImageURLs()
        {
            StringBuilder s = new StringBuilder();

            string matchPattern1 = @"]n,\[x22(.*?)x22";
            string matchPattern2 = @"]\n,\[x22(.*?)\?";
            string matchPattern3 = "\"ou\":\"(.*?)\",";
            string matchPattern5 = "px\\W><img data-src=\\W(.*?)(&amp;s)?\"\\s"; //06-11-2020 //24-06-2020
            Regex re = new Regex(matchPattern1, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = re.Matches(html);
            ArrayList alDup = new ArrayList();
            ArrayList myList = new ArrayList();
            foreach (Match m in mc)
            {
                string HtmlText = HttpUtility.HtmlDecode(m.Groups[1].Value);
                if (HtmlText.StartsWith("http") || HtmlText.StartsWith("https"))
                {
                    int n = HtmlText.IndexOf("?");
                    if (n > 0)
                        HtmlText = HtmlText.Remove(n);

                    alDup.Add(HtmlText);
                }
            }
            re = new Regex(matchPattern2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            mc = re.Matches(html);

            foreach (Match m in mc)
            {
                string HtmlText = HttpUtility.HtmlDecode(m.Groups[1].Value);
                if (HtmlText.StartsWith("http") || HtmlText.StartsWith("https"))
                {
                    int n = HtmlText.IndexOf("?");
                    if (n > 0)
                        HtmlText = HtmlText.Remove(n);

                    alDup.Add(HtmlText);
                }
            }
            re = new Regex(matchPattern3, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            mc = re.Matches(html);

            foreach (Match m in mc)
            {
                string HtmlText = HttpUtility.HtmlDecode(m.Groups[1].Value);
                if (HtmlText.StartsWith("http") || HtmlText.StartsWith("https"))
                {
                    int n = HtmlText.IndexOf("?");
                    if (n > 0)
                        HtmlText = HtmlText.Remove(n);

                    alDup.Add(HtmlText);
                }
            }

            //06-11-2020
            re = new Regex(matchPattern5, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            mc = re.Matches(html);

            foreach (Match m in mc)
            {
                string HtmlText = HttpUtility.HtmlDecode(m.Groups[1].Value);
                if (HtmlText.StartsWith("http") || HtmlText.StartsWith("https"))
                {
                    alDup.Add(HtmlText);
                }
            }
            //end 06-11-2020
            foreach (string s1 in alDup)
            {
                if (myList.Contains(s1) || string.IsNullOrEmpty(s1)) continue;
                myList.Add(s1);
            }

            foreach (string s1 in myList)
            {
                s.Append("<item url=\"" + SetUrl(s1) + "\" title=\"\" />");
            }

            return s.ToString();
        }

        private string GetTopStories(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//g-inner-card/a");

            if (nds == null)
                nds = node.SelectNodes(".//div[@class='dbsr']/a");
            if (nds == null)
                nds = node.SelectNodes(".//g-inner-card/div/a");   //01-05-2020
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='HCUNre dbsr']/a"); //21-09-2020 Top Stories block item urls selector updated

            if (nds != null)
                foreach (HtmlNode nd in nds)
                {
                    string title = "";
                    HtmlNode n = nd.SelectSingleNode(".//div[@class='y9oXvf rrBdId']");
                    if (n == null)
                        n = nd.SelectSingleNode(".//div[@class='y9oXvf']"); // 13-03-2020
                    if (n == null)
                        n = nd.SelectSingleNode(".//div[@class='mRnBbe QgUve jBgGLd']");
                    if (n == null)
                        n = nd.SelectSingleNode(".//div[@class='mRnBbe QgUve nDgy9d']");
                    if (n == null)
                        n = nd.SelectSingleNode(".//div[@class='nDgy9d']");     // changes on 02-07-2019
                    if (n == null)
                        //   n = nd.SelectSingleNode(".//div[@class='mCBkyc jBgGLd']|.//div[@class='mCBkyc nDgy9d']|.//div[@class='mCBkyc oz3cqf vH5Lmd nDgy9d']|.//div[@class='mCBkyc oz3cqf vH5Lmd jBgGLd']"); //08-08-2020 //04-06-2020   //01-05-2020
                        n = nd.SelectSingleNode(".//div[contains(@class, 'mCBkyc')]"); //08-08-2020 including contains function

                    if (n != null)
                        title = n.InnerText;
                    else
                        title = nd.InnerText;

                    s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"" + SetTitle(title) + "\" />");
                }
            else
            {
                nds = node.SelectNodes(".//g-card-section/a");

                if (nds != null)
                    foreach (HtmlNode nd in nds)
                    {
                        s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"" + SetTitle(nd.InnerText) + "\" />");
                    }
            }
            return s.ToString();
        }

        // 23-10-2019
        private string GetCarousel(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            //HtmlNodeCollection nd = node.SelectNodes(".//div[@jsmodel='uIhXXc']/div/g-scrolling-carousel/div/div/div/ul[@class='Kjd0sd']/div/div/g-inner-card/a");
            HtmlNodeCollection nd = node.SelectNodes(".//div[@class='nsEy4b']/div/g-inner-card/g-link/a");  //26-11-2019
            if (nd == null)
                nd = node.SelectNodes(".//div[@jsname='WUSFrc']/g-link/a");
            if (nd == null)
                nd = node.SelectNodes(".//div[@class='v1uiFd']/g-link/a");  // 11-05-2020
            string url = "";
            if (nd != null)
            {
                foreach (HtmlNode nd1 in nd)
                {
                    url = nd1.Attributes["href"].Value;
                    HtmlNode hn = nd1.SelectSingleNode(".//div[@class='mB12kf JRhSae nDgy9d']");
                    if (hn == null)
                        hn = nd1.SelectSingleNode(".//div[contains(@class, 'hfac6')]"); //28-07-2020
                    //if (hn == null)
                    //    hn = nd1.SelectSingleNode(".//div[@class='hfac6d oz3cqf vH5Lmd']");//04-06-2020 //28-07-2020
                    string title = hn.InnerText;
                    s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");

                }
            }
            if (nd == null)
            {
                nd = node.SelectNodes(".//a[@class='ttwCMe']");
                if (nd != null)
                    foreach (HtmlNode nd1 in nd)
                    {
                        if (url.Contains("http"))
                        {
                            url = nd1.Attributes["href"].Value;
                        }
                        HtmlNode hn = nd1.SelectSingleNode(".//div[@class='oyj2db']");

                        string title1 = hn.InnerText;
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title1) + "\" />");
                    }
            }
            // uncommented.
            //string caitems = GetCarouselURLs();
            //s.Append(caitems);
            return s.ToString();
        }


        public string GetCarouselURLs()
        {
            StringBuilder s = new StringBuilder();
            string matchPattern = "\\Wn,\\Wx222003\\Wx22:\\Wnull\\W\\Wx22(.*?)\\Wx22,\\Wx22(.*?)\\Wx22\\W\\Wx22(.*?)\\Wx22\\Wnull";
            Regex re = new Regex(matchPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = re.Matches(html);
            ArrayList alDup = new ArrayList();

            foreach (Match m in mc)
            {
                string url = HttpUtility.HtmlDecode(m.Groups[2].Value);
                string text = HttpUtility.HtmlDecode(m.Groups[3].Value);

                if (!url.Contains("youtube"))
                {
                    if (url.StartsWith("http") || url.StartsWith("https"))
                    {
                        int n = url.IndexOf("?");
                        if (n > 0)
                            url = url.Remove(n);
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(text) + "\" />");
                    }
                }
            }

            return s.ToString();
        }

        private string GetBlockType(HtmlNode node)
        {
            HtmlNode nd = node.SelectSingleNode(".//div[@class='_ELb']/a");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='twQ0Be']/a");
            if (nd != null)
            {
                return "VideoCard";
            }
            nd = node.SelectSingleNode(".//div[@class='KNcnob']/g-img|.//div[@class='YEMaTe']/g-img");   //01-05-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='fn6bCb']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='qmv19b']");    // 09-12-2019
            if (nd != null)
                return "topstories";
            if (nd == null)
                nd = node.SelectSingleNode(".//span[@class='qB1pae']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='BFJZOc']/div");
            if (nd == null)
                //nd = node.SelectSingleNode(".//div[@class='LMMXP i8lZMc']");  //23-07-2020 // 02-06-2020
                nd = node.SelectSingleNode(".//div[contains(@class, 'LMMXP')]");  //23-07-2020
            //if (nd == null)
            //    nd = node.SelectSingleNode(".//div[@class='LMMXP mfMhoc']");  //23-07-2020 //17-07-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='sQkmof']");//23-07-2020 included selector for videos

            //15-10-2020
            HtmlNode nd1 = null;
            if (nd != null)
                nd1 = node.SelectSingleNode(".//div[@class='g']");
            if (nd != null && nd1 == null)
                return "videos";
            //end 15-10-2020

            nd = node.SelectSingleNode(".//div[@class='_Zfh']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Brgz0 tw-res']");
            if (nd != null)
            {
                return "Twitters";
            }

            nd = node.SelectSingleNode(".//div[@class='kp-blk cUnQKe Wnoohf OJXvsb']"); //11-02-2020
            if (nd != null)
            {
                return "PeopleAlsoAsk"; //11-02-2020
            }
            if (node.SelectSingleNode(".//g-card[@class='cvoI5e']") != null || node.SelectSingleNode(".//g-card[@class='U8KfXc']") != null) //23-11-2020 //20-11-2020
            {
                return "Jobs";
            }

            nd = node.SelectSingleNode(".//div[@class='_OKe']");
            if (nd != null)
            {
                nd = node.SelectSingleNode(".//div[@id='imso-root']");
                if (nd != null)
                    return "Event";

                return "AnswerCard";
            }
            nd = node.SelectSingleNode(".//div[@class='pcCUmf vCOSGb']");//03-06-2020 includes below two lines
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='MHStgc']/span"); //05-10-2020 answer card selector
            if (nd != null)
                return "AnswerCard";

            if (node.SelectSingleNode(".//div[@id='imso-root']") != null || node.SelectSingleNode(".//div[@class='k9uN1c kfn9hb']") != null
                || node.SelectSingleNode(".//div[@class='HaXvv kfn9hb']") != null || node.SelectSingleNode(".//div[@class='tsp-view']") != null)//24-11-2020 selector for eventresults block//07-02-2020
                return "Event";

            if (node.SelectSingleNode(".//div[@id='cwmcwd']") != null || node.SelectSingleNode(".//div[@class='ifM9O']") != null
                || node.SelectSingleNode(".//div[@class='vk_ard']") != null || node.SelectSingleNode(".//div[@class='d7sCQ kp-header']") != null   //03-06-2020
                || node.SelectSingleNode(".//div[@class='pcCUmf vCOSGb']") != null
                || node.SelectSingleNode(".//div[@class='vkc_np kkww4d']") != null) //21-09-2020 updated answered card selectors  //03-06-2020
            {
                if (node.SelectSingleNode(".//div[@class='BET1rd']") == null) //25-09-2020
                    return "AnswerCard";
            }
            //05-10-2020 KP Block selectors updated
            nd = node.SelectSingleNode(".//div[@class='kp-wholepage EyBRub kp-wholepage-osrp HSryR']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kp-wholepage kp-wholepage-osrp HSryR EyBRub']");
            if (nd != null)
                return "KnowledgePanel";
            //end 05-10-2020

            nd = node.SelectSingleNode(".//div[@class='DUU6i']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='LnbJhc']");  //16-01-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@jscontroller='IkchZc']");//13-03-2020

            if (nd != null)
            {
                return "Images";
            }

            nd = node.SelectSingleNode(".//*[@id='lu_map']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='xERobd']");  //changed on 26-06-2019
            //if (nd == null)
            //    nd = node.SelectSingleNode(".//div[@class='MHStgc']/span");//05-10-2020 commented //02-10-2020 maps selectors
            if (nd != null)
            {
                return "Maps";
            }
            // changes in map block on 19-06-2019.
            else
            {
                nd = node.SelectSingleNode(".//g-img/img");
                if (nd != null)
                {
                    if (nd.Attributes["alt"].Value.StartsWith("Map of "))
                        return "Maps";
                }

                // changes on 08-07-2019
                nd = node.SelectSingleNode(".//img[@alt='map image']");
                if (nd != null)
                    return "Maps";

            }
            // end of map changes.

            nd = node.SelectSingleNode(".//div[@id='kx']|.//div[@id='rrg']|.//div[@class='oHJrJb']");//05-08-2020 included selector for carousel
            if (nd != null)
            {
                return "Carousel";
            }
            nd = node.SelectSingleNode(".//div[@id='fac-ut']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@id='knowledge-finance-wholepage__entity-summary']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@id='knowledge-currency__currency-v2-updatable']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='g obcontainer']");   // updated on 01-08-2019
            if (nd != null)
            {
                return "Finance";
            }
            nd = node.SelectSingleNode(".//table[@class='nrgt']|.//table[@class='jmjoTe']");   //22-08-2020 included selector for sitelinks
            if (nd != null)
            {
                return "SiteLinks";
            }

            return "";
        }

        private bool IsBlock(HtmlNode node)
        {
            bool bVal = (node.SelectSingleNode(".//h3[@class='zQlLed']") != null  // top stories       
                || node.SelectSingleNode(".//div[@class='wXlZre B03h3d V14nKc ptcLIOszQJu__wholepage-card wp-msss']") != null//topstories 08-04-2020
                || node.SelectSingleNode(".//div[@class='e2BEnf U7izfe']") != null//topstories 01-06-2020
                || node.SelectSingleNode(".//div[@class='e2BEnf U7izfe mfMhoc']") != null //21-09-2020 images selectors
                || node.SelectSingleNode(".//table[@class='nrgt']") != null || node.SelectSingleNode(".//table[@class='jmjoTe']") != null      // site links  22-08-2020 included block type selector
                || node.SelectSingleNode(".//img[@id='lu_map']") != null      // maps
                || node.SelectSingleNode(".//div[@class='xERobd']") != null //  maps    //changed on 26-06-2019
                || node.SelectSingleNode(".//div[@id='kx']") != null      // carousel
                || node.SelectSingleNode(".//div[@id='fac-ut']") != null      // finance
                || node.SelectSingleNode(".//div[@class='_Zfh']") != null   // twitters
                || node.SelectSingleNode(".//div[@class='Brgz0 tw-res']") != null   // twitters                
                || node.SelectSingleNode(".//div[@class='_OKe']") != null   // answer card / people also ask
                || node.SelectSingleNode(".//div[@class='vkc_np kkww4d']") != null   // 23-03-2020
                || node.SelectSingleNode(".//div[@class='k9uN1c kfn9hb']") != null//24-10-2019
                || node.SelectSingleNode(".//div[@class='HaXvv kfn9hb']") != null//07-02-2020
                || (node.SelectSingleNode(".//div[@class='ifM9O']") != null && node.SelectSingleNode(".//div[@class='Wnoohf OJXvsb']") == null)   // answer card  
                || node.SelectSingleNode(".//div[@id='cwmcwd']") != null  // answer card 
                                                                          //|| node.SelectSingleNode(".//div[@class='vk_c card-section']") != null // answer card    
                || node.SelectSingleNode(".//div[@class='d7sCQ kp-header']") != null  //03-06-2020                                                      //|| node.SelectSingleNode(".//div[@class='vk_c card-section']") != null // answer card                
                || node.SelectSingleNode(".//div[@class='pcCUmf vCOSGb']") != null  //03-06-2020
                || node.SelectSingleNode(".//div[@class='kp-blk cUnQKe Wnoohf OJXvsb']") != null  // people also ask // 11-02-2020
                                                                                                  //|| node.SelectSingleNode(".//span[@data-original-name='People also ask']") != null  // people also ask
                || node.SelectSingleNode(".//h3[@class='_DM']") != null || node.SelectSingleNode(".//div[@id='imagebox_bigimages']") != null || node.SelectSingleNode(".//div[@class='mR2gOd']") != null //27-06-2020   // images
                || node.SelectSingleNode(".//div[@class='e2BEnf']/h3") != null // videos
                || node.SelectSingleNode(".//div[@class='e2BEnf U7izfe']/h3") != null  // videos
                || node.SelectSingleNode(".//div[@class='mod NFQFxe oHglmf xzPb7d']") != null//images//05-08-2020
                || node.SelectSingleNode(".//div[@id='knowledge-finance-wholepage__entity-summary']") != null // 18-03-2020
                || node.SelectSingleNode(".//div[@class='I6TXqe osrp-blk']") != null //12-08-2020 included selector for video card
                || node.SelectSingleNode(".//div[@class='WcS13d']") != null //02-10-2020 maps selectors
                || node.SelectSingleNode(".//h3[@class='GmE3X']") != null); //16-10-2020 updated selector for videos
            if (bVal == true)//2019-09-11
            {
                try
                {
                    if (node.InnerText.Contains("Podcast") || node.InnerText.Contains("播客") || node.InnerText.Contains("Podcaster")
                        || node.InnerText.Contains("ملفات البودكاست") || node.InnerText.Contains("พอดแคสต์"))   // 19-09-2019
                    {
                        if (node.SelectSingleNode(".//div[@class='Brgz0 tw-res']") == null)
                            bVal = false;
                    }
                    // 02-06-2020
                    if (node.SelectSingleNode(".//div[@class='a3spGf WvKfwe']|.//div[@class='HnYYW i8lZMc']") != null
                        && node.SelectSingleNode(".//div[@class='Brgz0 tw-res']|.//div[@class='kp-blk cUnQKe Wnoohf OJXvsb']") == null)
                        bVal = false;
                }
                catch { }
            }

            if (!bVal)
            {
                HtmlNode nd = node.SelectSingleNode(".//h3|.//div[contains(@class,'HnYYW')]|.//div[@class='LMMXP i8lZMc']|.//div[@class='e2BEnf U7izfe']/div|.//div[@class='LMMXP mfMhoc']"); //05-08-2020 included contains function  //17-07-2020 //03-06-2020  // 02-06-2020    //01-05-2020
                if (nd != null)
                    if (nd.InnerText == "Top stories" || nd.InnerText == "Huvudnyheter" || nd.InnerText == "Videos" || nd.InnerText == "Video" || nd.InnerText == "Tin bài hàng đầu" || nd.InnerText == "Voorpaginanieuws" || nd.InnerText == "Vertaalresultaat" || nd.InnerText == "Recipes")//05-08-2020 //29-06-2020//03-06-2020 // 02-06-2020  // 08-04-2020
                        return true;

                // changes in map block on 19-06-2019.
                nd = node.SelectSingleNode(".//g-img/img");
                if (nd != null)
                {
                    if (nd.Attributes["alt"].Value.StartsWith("Map of "))
                        return true;
                }
                // changes on 08-07-2019
                if (node.SelectSingleNode(".//img[@alt='map image']") != null)
                    return true;

                HtmlNodeCollection nds = node.SelectNodes(".//div");
                if (nds != null)
                    foreach (HtmlNode n in nds)
                    {
                        try
                        {
                            if (n.Attributes["class"] != null)   //01-06-2020
                                if (n.Attributes["class"].Value.Contains("obcontainer"))  //node.SelectSingleNode(".//div[@id='cwmcwd']") != null)
                                {
                                    bVal = true;
                                    break;
                                }
                        }
                        catch
                        { }
                    }
            }
            return bVal;
        }

        private bool IsOrganic(HtmlNode node)
        {
            return (node.SelectSingleNode(".//h3[@class='r']") != null || node.SelectSingleNode(".//div[@class='r']") != null
                || node.SelectSingleNode(".//div[@class='zTpPx']") != null || node.SelectSingleNode(".//div[@class='zTpPx']/g-link/a") != null    // 28-05-2020   // 13-03-2020
                || node.SelectSingleNode(".//h3[@class='r dO0Ag']") != null || node.SelectSingleNode(".//div[@class='DOqJne']") != null //27-06-2020    //29-05-2020
                || node.SelectSingleNode(".//div[@class='rc']") != null); // 03-09-2020 missing classic links selector included
        }

        //07-11-2019
        private string GetRedirectedUrl(string url)
        {
            try //28-09-2020  try catch.
            {
                //21-11-2019
                url = url.Replace("HTTPS://", "https://").Replace("HTTP://", "http://");
                if (string.IsNullOrEmpty(url.Trim())) return string.Empty;
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
                    url = url.Replace("////", "//"); //18-09-2020

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

                if ((url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("ftp://")) && !url.Contains("/aclk?"))  // 30-04-2020 
                    return url;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return string.Empty;

        }

        // There are chances method was used for title contains in case any issues in xml applied decode/encode.
        public string SetTitle(string unicodestring)
        {
            //return WebUtility.HtmlEncode(WebUtility.HtmlDecode(unicodestring));
            return WebUtility.HtmlEncode(WebUtility.HtmlDecode(unicodestring)).Replace("\\x27", "'").Replace("\\\\u0026", "&amp;").Replace("\\\\\\x22", "&quot;");
        }

        public string SetUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty; //25-08-2020
            try  //28-09-2020  try catch.
            {
                //21-11-2019
                url = GetRedirectedUrl(WebUtility.HtmlDecode(url).Trim());
                //if (url.ToLower().Contains("%2f") || url.ToLower().Contains("%2e"))//18-09-2020 commented
                if (url.Contains("%")) //18-09-2020
                    url = GetRedirectedUrl(WebUtility.UrlDecode(WebUtility.HtmlDecode(url)).Trim());
                ////SanitizeXmlString(url);
                return WebUtility.HtmlEncode(SanitizeXmlString(url).Replace("\x00", "%00")).Replace("\\\\u003d", "=").Replace('\u0002', ' ').Replace('\u0018', ' ').Replace('\f', ' ').Trim();//23-09-2020 applied method to URL  //26-03-2020 updated converting hexadecimal codes
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //27-08-2020
        private string GetRedirectedUrl_TextAds(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            try  //28-09-2020  try catch.
            {
                url = url.Replace("HTTPS://", "https://").Replace("HTTP://", "http://");
                if (string.IsNullOrEmpty(url.Trim())) return string.Empty;

                //18-09-2020 commented
                //if (url.LastIndexOf("https://") > 0)
                //    url = url.Remove(0, url.LastIndexOf("https://"));
                //if (url.LastIndexOf("http://") > 0)
                //    url = url.Remove(0, url.LastIndexOf("http://"));
                //end 18-09-2020


                Regex rx = new Regex("http[\\w]?://(.*)", RegexOptions.Singleline);
                if (!rx.Match(url).Success && !url.Contains("/aclk?"))
                    if (!url.Contains("://"))
                        url = "http://" + url;

                if (url.StartsWith("http:////") || url.StartsWith("https:////")) //18-09-2020 condition applied if appears http:////
                    url = url.Replace("////", "//"); //18-09-2020

                if (url.Contains("&amp;grqid="))
                    url = url.Remove(url.IndexOf("&amp;grqid="));

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

                if ((url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("ftp://")) && (!url.StartsWith("/aclk?") && !url.Contains("search?num=100")))
                {
                    //if (url.ToLower().Contains("%2f") || url.ToLower().Contains("%2e")) //commented 18-09-2020
                    //01-10-2020 commented
                    //if (url.Contains("%")) //18-09-2020
                    // url = GetRedirectedUrl(WebUtility.UrlDecode(WebUtility.HtmlDecode(url)).Trim());
                    //end 01-10-2020
                    url = WebUtility.UrlDecode(WebUtility.HtmlDecode(url)).Trim();//01-10-2020
                    return WebUtility.HtmlEncode(SanitizeXmlString(url).Replace("\x00", "%00")).Replace("\\\\u003d", "=").Replace('\u0002', ' ').Replace('\u0018', ' ').Replace('\f', ' ').Trim(); //23-09-2020 applied method to URL
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return string.Empty;
        }
        //23-09-2020 included method to find and fix hexdecimal chars
        public string SanitizeXmlString(string xml)
        {

            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            StringBuilder buffer = new StringBuilder(xml.Length);

            foreach (char c in xml)
            {
                if (XmlSanitizingStream.IsLegalXmlChar(c))
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }
        //end 23-09-2020

        public class XmlSanitizingStream : StreamReader
        {
            public XmlSanitizingStream(Stream streamToSanitize)
            : base(streamToSanitize, true)
            { }

            /// <summary>
            /// Whether a given character is allowed by XML 1.0.
            /// </summary>
            public static bool IsLegalXmlChar(int character)
            {
                return
                (
                     character == 0x9 /* == '\t' == 9   */          ||
                     character == 0xA /* == '\n' == 10  */          ||
                     character == 0xD /* == '\r' == 13  */          ||
                    (character >= 0x20 && character <= 0xD7FF) ||
                    (character >= 0xE000 && character < 0xFFFD) ||  //02-11-2020 changed <= 0xFFFD to < 0xFFFD
                    (character >= 0x10000 && character <= 0x10FFFF)
                );
            }
            private const int EOF = -1;

            public override int Read()
            {
                // Read each char, skipping ones XML has prohibited

                int nextCharacter;

                do
                {
                    // Read a character

                    if ((nextCharacter = base.Read()) == EOF)
                    {
                        // If the char denotes end of file, stop
                        break;
                    }
                }

                // Skip char if it's illegal, and try the next

                while (!XmlSanitizingStream.
                        IsLegalXmlChar(nextCharacter));

                return nextCharacter;
            }

            public override int Peek()
            {
                // Return next legal XML char w/o reading it 

                int nextCharacter;

                do
                {
                    // See what the next character is 
                    nextCharacter = base.Peek();
                }
                while
                (
                    // If it's illegal, skip over 
                    // and try the next.

                    !XmlSanitizingStream.IsLegalXmlChar(nextCharacter) &&
                    (nextCharacter = base.Read()) != EOF
                );

                return nextCharacter;

            }
        }

    }

}
