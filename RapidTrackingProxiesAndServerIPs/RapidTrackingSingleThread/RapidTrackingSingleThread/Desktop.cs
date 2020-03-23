using HtmlAgilityPack;
using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace RapidTrackingSingleThread
{
    public class Desktop
    {
        public int orgLinks;
        string html;
        public int count;
        public string ProcessDocument(string seid, string keyword, HtmlDocument doc)
        {
            count = 0;
            //if (doc == null) throw new Exception("No source found.");

            HtmlNode htmlNode = doc.DocumentNode.SelectSingleNode("//table[@id='mn']");
            if (htmlNode != null)
            {
                throw new Exception("Old page found.");
            }
            orgLinks = 0;
            string ndText = "";

            html = doc.DocumentNode.OuterHtml;
            StringBuilder sb = new StringBuilder();
            //sb.Append("<searchResult searchEngine=\"" + seid + "\" keyword=\"" + WebUtility.HtmlEncode(keyword) + "\" date=\"2020-02-06\" >");
            sb.Append("<searchResult searchEngine=\"" + seid + "\" keyword=\"" + WebUtility.HtmlEncode(keyword) + "\" date=\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\" >");
            sb.Append("<section col=\"main\">");
            string topStuff = GetTopStuff(doc);
            ndText = topStuff;
            sb.Append(topStuff);

            HtmlNodeCollection nodeCol = doc.DocumentNode.SelectNodes("//div[@class='_NId']");
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@class='bkWMgd']");
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@id='ires']/ol/div");
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div"); //13-03-2020

            //if (nodeCol == null) throw new Exception("No block found.");
            if (nodeCol == null) return string.Empty;
            //if (nodeCol == null) goto BOTTOMSTUFF; 

            try
            {
                foreach (HtmlNode node in nodeCol)
                {

                    if (node.InnerHtml != "")
                    {
                        string s = ProcessNode(node);
                        ndText += s;
                        if (s.Length > 0)
                            sb.Append(s);
                    }
                }
            }
            catch { }
            

            //if (orgLinks < count)
            //    return string.Empty;

            //BOTTOMSTUFF:
            string bottomStuff = GetBottomStuff(doc);
            ndText += bottomStuff;
            sb.Append(bottomStuff);
            sb.Append("</section>");

            sb.Append("<section col=\"right\">");
            string rightStuff = GetRightStuff(doc);
            ndText += rightStuff;
            sb.Append(rightStuff);
            sb.Append("</section>");
            sb.Append("</searchResult>");

            if (ndText.Length > 0)
            {
                count = orgLinks;
                return sb.ToString();
            }

            return string.Empty;

        }

        private string GetRightStuff(HtmlDocument doc)
        {
            StringBuilder s = new StringBuilder();

            HtmlNode rcNode = doc.DocumentNode.SelectSingleNode("//div[@id='rhs_block']");
            if (rcNode == null)
                rcNode = doc.DocumentNode.SelectSingleNode("//div[@id='rhs']"); // 18-11-2019
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
            HtmlNode colb = doc.DocumentNode.SelectSingleNode("//div[@id='bottomads']");   //20-01-2020 included selectors for Ads
            if (colb != null)
            {
                HtmlNodeCollection col = colb.SelectNodes(".//div[@id='tadsb']/ol/li");   //20-01-2020 included selectors for Ads
                if (col == null) return s.ToString();   //20-01-2020
                s.Append("<block type=\"adwords\" url=\"\">");
                foreach (HtmlNode nd in col)
                {
                    HtmlNode n = nd.SelectSingleNode(".//h3/a[2]|.//div[@class='ad_cclk']/a[2]");
                    if (n != null)
                    {
                        //if (!n.Attributes["href"].Value.StartsWith("/"))
                        //    s.Append("<item url=\"" + SetUrl(n.Attributes["href"].Value) + "\" title=\"" + SetTitle(n.InnerText) + "\" />");
                        //else
                        //{
                        //    string title = n.InnerText;
                        //    n = nd.SelectSingleNode(".//div[@class='ads-visurl']/cite");
                        //    if (n != null)
                        //        s.Append("<item url=\"" + SetUrl(n.InnerText) + "\" title=\"" + SetTitle(title) + "\" />");
                        //}

                        HtmlNode tittlenode = n.SelectSingleNode(".//h3");
                        if (!n.Attributes["href"].Value.StartsWith("/"))
                            s.Append("<item url=\"" + SetUrl(n.Attributes["href"].Value) + "\" title=\"" + SetTitle(tittlenode.InnerText) + "\" />");
                        else
                        {
                            string title = tittlenode.InnerText;
                            n = nd.SelectSingleNode(".//div[@class='ads-visurl']/cite");
                            if (n != null)
                                s.Append("<item url=\"" + SetUrl(n.InnerText) + "\" title=\"" + SetTitle(title) + "\" />");
                        }
                    }
                }
                s.Append("</block>");
            }

            return s.ToString();
        }

        private string GetAds(HtmlDocument doc)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//div[@id='tads']/ol/li");
            if (col != null)
            {
                s.Append("<block type=\"adwords\" url=\"\">");
                foreach (HtmlNode nd in col)
                {
                    HtmlNode n = nd.SelectSingleNode(".//h3/a[2]");
                    if (n != null)
                    {
                        if (!n.Attributes["href"].Value.StartsWith("/"))
                            s.Append("<item url=\"" + SetUrl(n.Attributes["href"].Value) + "\" title=\"" + SetTitle(n.InnerText) + "\" />");
                        else
                        {
                            string title = n.InnerText;
                            n = nd.SelectSingleNode(".//div[@class='ads-visurl']/cite");
                            if (n != null)
                                s.Append("<item url=\"" + SetUrl(n.InnerText) + "\" title=\"" + SetTitle(title) + "\" />");
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
            HtmlNode crNode = doc.DocumentNode.SelectSingleNode("//div[@id='extabar']");
            if (crNode != null)
            {
                if (crNode.SelectSingleNode(".//div[@id='kx']") != null || crNode.SelectSingleNode(".//g-scrolling-carousel") != null)
                {
                    s.Append("<block type=\"carousel\" url=\"\">");
                    //s.Append(GetCarousel(crNode));  // 23-10-2019
                    s.Append("</block>");
                }
            }

            // product listed ads
            HtmlNode pla = doc.DocumentNode.SelectSingleNode("//div[@class='cu-container']");
            if (pla != null)
            {
                HtmlNode h3 = pla.SelectSingleNode(".//div[@class='dxR8gf']/h3");
                if (h3 != null)
                {
                    if (pla.SelectSingleNode(".//div[contains(@class, 'commercial-unit-desktop-top')]") != null)
                    //if (h3.InnerText.StartsWith("Shop for") || h3.InnerText.StartsWith("See ") || h3.InnerText.StartsWith("zwarte "))
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
            HtmlNode colt = doc.DocumentNode.SelectSingleNode("//div[@id='tvcap']");  //20-01-2020 included select text Ads
            if (colt != null)
            {
                HtmlNodeCollection col = colt.SelectNodes(".//div[@id='tads']/ol/li|.//div[@id='tadsb']/ol/li");  //20-01-2020 included select text Ads
                if (col == null) return s.ToString();  //20-01-2020
                s.Append("<block type=\"adwords\" url=\"\">");
                foreach (HtmlNode nd in col)
                {
                    //HtmlNode n = nd.SelectSingleNode(".//h3/a[2]");
                    HtmlNode n = nd.SelectSingleNode(".//div[@class='ad_cclk']/a[2]");
                    if (n != null)
                    {
                        if (!n.Attributes["href"].Value.StartsWith("/"))
                        {
                            HtmlNode title = n.SelectSingleNode(".//h3");
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
                        }
                    }
                }
                s.Append("</block>");
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

                HtmlNodeCollection map = colt.SelectNodes(".//g-tray-header[@class='XvZKZb ndEm3b']/div/span");
                if (map != null)
                {
                    foreach (var node in map)
                    {
                        if (node.InnerText == "Affected area")
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

        private string ProcessOrganic(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            if (node.HasClass("_NId") || node.HasClass("bkWMgd") || node.HasClass("srg")
               || node.HasClass("g") || node.SelectNodes(".//div[@class='g']") != null) // 18-03-2020 // 13-03-2020
            {
                HtmlNodeCollection nds = node.SelectNodes(".//div[@class='g']");
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='rc']");
                if (nds != null)
                    foreach (HtmlNode nd in nds)
                    {
                        HtmlNode title = null;  // 18-11-2019
                        HtmlNode n = nd.SelectSingleNode(".//h3[@class='r']/a");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='r']/a");
                        if (n != null)
                            title = n.SelectSingleNode(".//h3");

                        HtmlNode img = nd.SelectSingleNode(".//img");
                        if (img != null)
                        {
                            if (Regex.IsMatch(nd.OuterHtml, "id=\"vidthumb\\d*\"")) // 17-01-2020 //Replaced selector for video block
                            {
                                var urls = n.Attributes["href"].Value;
                                if (urls.StartsWith("http") || urls.StartsWith("https"))
                                {
                                    int indx = urls.LastIndexOf("http://");
                                    if (indx < 0)
                                    {
                                        indx = urls.LastIndexOf("https://");
                                    }
                                    urls = urls.Remove(0, indx);
                                    // video block.
                                    s.Append("<block type=\"video\" url=\"\">");
                                    s.Append("<item url=\"" + SetUrl(urls) + "\" title=\"" + SetTitle(title.InnerText) + "\" />");
                                    s.Append("</block>");
                                }
                            }

                            //Changes - Included else if condition which was missing.....
                            else if (orgLinks < 100)
                            {
                                var urls = n.Attributes["href"].Value;
                                if (urls.StartsWith("http") || urls.StartsWith("https"))
                                {
                                    int indx = urls.LastIndexOf("http://");
                                    if (indx < 0)
                                    {
                                        indx = urls.LastIndexOf("https://");
                                    }
                                    urls = urls.Remove(0, indx);
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
                                var urls = n.Attributes["href"].Value;
                                string t;
                                if (title != null)
                                    t = title.InnerText;
                                else
                                    t = n.InnerText;
                                if (urls.StartsWith("http") || urls.StartsWith("https"))
                                {
                                    int indx = urls.LastIndexOf("http://");
                                    if (indx < 0)
                                    {
                                        indx = urls.LastIndexOf("https://");
                                    }
                                    urls = urls.Remove(0, indx);
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
                foreach (HtmlNode nd in col)
                {
                    string u = nd.Attributes["href"].Value.Replace("/url?q=", "").Replace("&amp;", "&").Replace("&", "&#38;");

                    //u = nd.Attributes["href"].Value.StartsWith("http").ToString();
                    if (u.Contains("&sa="))
                        u = u.Substring(0, u.IndexOf("&sa="));
                    if (orgLinks < 100)
                    {
                        if (u.StartsWith("http") || u.StartsWith("https"))
                        {
                            int indx = u.LastIndexOf("http://");
                            if (indx < 0)
                            {
                                indx = u.LastIndexOf("https://");
                            }
                            u = u.Remove(0, indx);
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
                    //s.Append(GetCarousel(node));
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
                default:
                    break;
            }
            return s.ToString();

        }

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
            if (n != null)
            {
                if (orgLinks < 100)
                {
                    HtmlNode t = n.SelectSingleNode(".//h3");
                    s.Append("<item url=\"" + SetUrl(n.Attributes["href"].Value) + "\" title=\"" + SetTitle(t.InnerText) + "\" />");
                    orgLinks++;
                }
            }

            HtmlNodeCollection nds = node.SelectNodes(".//table[@class='nrgt']/tr");
            if (nds != null)
            {
                s.Append("<block type=\"siteLinks\" url=\"\">");
                foreach (HtmlNode nd in nds)
                {
                    HtmlNodeCollection c = nd.SelectNodes(".//h3[@class='r']/a");
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
                return string.Empty;
            foreach (HtmlNode nd in nds)
            {
                string title = nd.SelectSingleNode(".//h3").InnerText;
                s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"" + SetTitle(title) + "\" />");
            }
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
            string url = "";
            if (nd != null)
            {
                foreach (HtmlNode nd1 in nd)
                {
                    url = nd1.Attributes["href"].Value;
                    HtmlNode hn = nd1.SelectSingleNode(".//div[@class='mB12kf JRhSae nDgy9d']");
                    if (hn == null)
                        hn = nd1.SelectSingleNode(".//div[@class='hfac6d']");
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
            nd = node.SelectSingleNode(".//div[@class='KNcnob']/g-img");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='fn6bCb']");
            if (nd != null)
                return "topstories";
            if (nd == null)
                nd = node.SelectSingleNode(".//span[@class='qB1pae']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='BFJZOc']/div");
            if (nd != null)
                return "videos";

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
            nd = node.SelectSingleNode(".//div[@class='_OKe']");
            if (nd != null)
            {
                nd = node.SelectSingleNode(".//div[@id='imso-root']");
                if (nd != null)
                    return "Event";

                return "AnswerCard";     //updated code 11-02-2020
            }

            if (node.SelectSingleNode(".//div[@id='imso-root']") != null || node.SelectSingleNode(".//div[@class='k9uN1c kfn9hb']") != null //24-10-2019
                || node.SelectSingleNode(".//div[@class='HaXvv kfn9hb']") != null)//07-02-2020 included selector for Event Block
                return "Event";

            if (node.SelectSingleNode(".//div[@id='cwmcwd']") != null || node.SelectSingleNode(".//div[@class='ifM9O']") != null
                || node.SelectSingleNode(".//div[@class='vk_ard']") != null)    // changes on 09-07-2019
            {

                return "AnswerCard";   //updated code 11-02-2020
            }

            nd = node.SelectSingleNode(".//div[@class='DUU6i']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='LnbJhc']");  //16-01-2020  //Included selector for Image block
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@jscontroller='IkchZc']");//13-03-2020
            if (nd != null)
            {
                return "Images";
            }

            nd = node.SelectSingleNode(".//*[@id='lu_map']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='xERobd']");  //changed on 26-06-2019
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

            nd = node.SelectSingleNode(".//div[@id='kx']|.//div[@id='rrg']");
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
            nd = node.SelectSingleNode(".//table[@class='nrgt']");
            if (nd != null)
            {
                return "SiteLinks";
            }

            return "";
        }

        private bool IsBlock(HtmlNode node)
        {
            bool bVal = (node.SelectSingleNode(".//h3[@class='zQlLed']") != null  // top stories                
                || node.SelectSingleNode(".//table[@class='nrgt']") != null      // site links
                || node.SelectSingleNode(".//img[@id='lu_map']") != null      // maps
                || node.SelectSingleNode(".//div[@class='xERobd']") != null //  maps    //changed on 26-06-2019
                || node.SelectSingleNode(".//div[@id='kx']") != null      // carousel
                || node.SelectSingleNode(".//div[@id='fac-ut']") != null      // finance
                || node.SelectSingleNode(".//div[@class='_Zfh']") != null   // twitters
                || node.SelectSingleNode(".//div[@class='Brgz0 tw-res']") != null   // twitters                
                || node.SelectSingleNode(".//div[@class='_OKe']") != null   // answer card / people also ask
                || node.SelectSingleNode(".//div[@class='k9uN1c kfn9hb']") != null//24-10-2019
                 || node.SelectSingleNode(".//div[@class='HaXvv kfn9hb']") != null//07-02-2020 included selector people also ask block
                || (node.SelectSingleNode(".//div[@class='ifM9O']") != null && node.SelectSingleNode(".//div[@class='Wnoohf OJXvsb']") == null)   // answer card  
                || node.SelectSingleNode(".//div[@id='cwmcwd']") != null  // answer card 
                 || node.SelectSingleNode(".//div[@class='kp-blk cUnQKe Wnoohf OJXvsb']") != null  // people also ask // 11-02-2020 included selector
                                                                                                   //|| node.SelectSingleNode(".//span[@data-original-name='People also ask']") != null  // people also ask
                || node.SelectSingleNode(".//h3[@class='_DM']") != null || node.SelectSingleNode(".//div[@id='imagebox_bigimages']") != null   // images
                || node.SelectSingleNode(".//div[@class='e2BEnf']/h3") != null // videos
                || node.SelectSingleNode(".//div[@class='e2BEnf U7izfe']/h3") != null // videos
                || node.SelectSingleNode(".//div[@id='knowledge-finance-wholepage__entity-summary']") != null); // 18-03-2020

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
                }
                catch { }
            }

            if (!bVal)
            {
                HtmlNode nd = node.SelectSingleNode(".//h3");
                if (nd != null)
                    if (nd.InnerText == "Top stories" || nd.InnerText == "Videos")  // 18-03-2020
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
               || node.SelectSingleNode(".//div[@class='zTpPx']") != null);    // 13-03-2020
        }


        internal object GetOxylabsWebDataSources_Nws_Images(string kw, string v1, string v2, string v3, string v4, string v5)
        {
            throw new NotImplementedException();
        }

        //07-11-2019
        private string GetRedirectedUrl(string url)
        {
            //21-11-2019
            url = url.Replace("HTTPS://", "https://").Replace("HTTP://", "http://");
            if (string.IsNullOrEmpty(url.Trim())) return string.Empty;

            if (url.LastIndexOf("https://") > 0)
                url = url.Remove(0, url.LastIndexOf("https://"));
            if (url.LastIndexOf("http://") > 0)
                url = url.Remove(0, url.LastIndexOf("http://"));

            Regex rx = new Regex("http[\\w]?://(.*)", RegexOptions.Singleline);
            if (!rx.Match(url).Success && !url.Contains("/aclk?"))
                url = "http://" + url;

            if (url.Contains("&amp;grqid="))
                url = url.Remove(url.IndexOf("&amp;grqid="));

            if (url.Contains("\0&"))
                url = url.Replace("\0&", "%00&");

            if ((url.StartsWith("https://") || url.StartsWith("http://")) && !url.Contains("/aclk?"))
                return url;

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
            //21-11-2019
            url = GetRedirectedUrl(WebUtility.HtmlDecode(url).Trim());
            if (url.ToLower().Contains("%2f") || url.ToLower().Contains("%2e"))
                url = GetRedirectedUrl(WebUtility.UrlDecode(WebUtility.HtmlDecode(url)).Trim());

            return WebUtility.HtmlEncode(url); //.Replace("&", "&amp;").Replace("&nbsp", "");
        }

    }
}
