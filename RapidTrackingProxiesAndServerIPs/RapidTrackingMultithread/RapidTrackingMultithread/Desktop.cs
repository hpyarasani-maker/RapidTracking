using HtmlAgilityPack;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace RapidTrackingMultithread
{
    public class Desktop 
    {
        public int orgLinks;
        string html;
        public string ProcessDocument(string seid, string keyword, HtmlDocument doc)
        {
            //count = 0;
            if (doc == null) throw new Exception("No source found.");
            HtmlNode htmlNode = doc.DocumentNode.SelectSingleNode("//table[@id='mn']");
            if (htmlNode != null)
            {
                throw new Exception("Old page found.");
            }
            orgLinks = 0;
            string ndText = "";

            html = doc.DocumentNode.OuterHtml;
            StringBuilder sb = new StringBuilder();            
            sb.Append("<searchResult searchEngine=\"" + seid + "\" keyword=\"" + WebUtility.HtmlEncode(keyword) + "\" date=\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\" >");
            sb.Append("<section col=\"main\">");
            string topStuff = GetTopStuff(doc);
            ndText = topStuff;
            sb.Append(topStuff);

            HtmlNodeCollection nodeCol = doc.DocumentNode.SelectNodes("//div[@class='_NId']");
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@class='bkWMgd']");
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@id='ires']/ol/div");//09-12-2020
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div|//div[@id='rso']/g-section-with-header|//div[@class='Hpbsqe']|//div[@id='Odp5De']");//23-12-2021//08-10-2021 images //03-12-2020  //01-05-2020 
            if (nodeCol == null || (nodeCol.Count > 1 && nodeCol.Count <= 3))//25-07-2022
                nodeCol = doc.DocumentNode.SelectNodes(".//div[contains(@class,'WvKfwe')]/div|.//div[@class='UDZeY OTFaAf']/div|.//div[@class='ULSxyf']") ?? nodeCol;//19-07-2022//15-07-2022
            if (nodeCol == null || nodeCol.Count <= 1) //01-08-2022 swapped lines
            {
                nodeCol = doc.DocumentNode.SelectNodes("//div[contains(@class,'WvKfwe')]/div|.//div[@class='UDZeY OTFaAf']/div") ?? nodeCol;
                if (nodeCol == null || nodeCol.Count <= 3)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='kp-wp-tab-overview']/div|//div[@class='hlcw0c']/div") ?? nodeCol;
            }
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
                    nodeCol = doc.DocumentNode.SelectNodes(".//div[contains(@class,'WvKfwe')]/div|.//div[contains(@class,'WvKfwe')]/g-section-with-header|.//div[@class='UDZeY OTFaAf']");//09-12-2020
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

            if (nodeCol == null) throw new Exception("No block found.");

            //if (nodeCol == null) return string.Empty;                      
            //if (nodeCol == null) goto BOTTOMSTUFF; 

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
                //count = orgLinks;
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
            if (node == null)
                node = rcNode.SelectSingleNode(".//div[contains(@class, 'knowledge-panel')]");//03-03-2022
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
                HtmlNodeCollection col = colb.SelectNodes(".//div[@id='tadsb']/ol/li|.//div[@id='tads']/div[@class='uEierd']|.//div[@id='tadsb']/div[@class='uEierd']|.//div[@id='tadsb']/div[@class='uEierd']|.//div[@id='tadsb']/div/div[@class='uEierd']");//28-03-2022 "/div" included //24-09-2020 bottom adwords//21-09-2020 updated bottom adwords   //20-01-2020
                if (col == null)
                    col = colb.SelectNodes(".//div[@id='tadsb']/div/ol/li");   //16-04-2020
                if (col == null) return s.ToString();   //20-01-2020

                s.Append("<block type=\"adwords\" url=\"\">");
                foreach (HtmlNode nd in col)
                {
                    HtmlNode n = nd.SelectSingleNode(".//h3/a[2]|.//div[@class='ad_cclk']/a[2]|.//div[@class='d5oMvf']/a|.//div[contains(@class,'v5yQqb')]/a");//12-11-2021 //27-06-2020
                    if (n != null)
                    {
                        HtmlNode tittlenode = n.SelectSingleNode(".//h3|.//div[@role='heading']");//27-06-2020
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
                HtmlNodeCollection col = colt.SelectNodes(".//div[@id='tads']/ol/li|.//div[@id='tads']/div/ol/li|.//div[@id='tadsb']/ol/li|.//div[@id='tads']/div[@class='uEierd']|.//div[@id='tads']/div/div[@class='uEierd']");//28-03-2022 "/div/"included //21-09-2020 adwords selector//20-01-2020 //08-04-2020

                if (col != null) //return s.ToString();  //20-01-2020
                {
                    s.Append("<block type=\"adwords\" url=\"\">");
                    foreach (HtmlNode nd in col)
                    {
                        //HtmlNode n = nd.SelectSingleNode(".//h3/a[2]");
                        HtmlNode n = nd.SelectSingleNode(".//div[@class='ad_cclk']/a[2]|.//div[contains(@class,'d5oMvf')]/a|.//div[contains(@class,'v5yQqb')]/a");//12-11-2021 //29-08-2020 included contains fucntions //23-07-2020 included missing item urls selectors
                        if (n != null)
                        {
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
                //|| node.HasClass("g") || node.SelectNodes(".//div[@class='g']") != null //c// 18-03-2020 
                || node.SelectNodes(".//div[@class='g GjRtuc']") != null // 02-06-2020
                || node.SelectNodes(".//div[contains(@class,'g card-section')]|.//div[@class='N3nEGc']") != null //03-03-2022 //06-10-2020 classic link
                || node.SelectNodes(".//div[@class='g tF2Cxc']|.//div[contains(@class,'g dFd2Tb')]|.//div[@class='g ZYT4Gf']") != null//13-07-2022 07-04-2022 //24-08-2021 video block //01-06-2021
                || node.SelectNodes(".//div[@class='d3zsgb']/div[@class='yuRUbf']") != null) //13-04-2022 missing CLinks
            {
                //HtmlNodeCollection nds = node.SelectNodes(".//div[contains(@class,'tF2Cxc')]"); //25-01-2022
                HtmlNodeCollection nds = node.SelectNodes(".//div[contains(@class,'g tF2Cxc')]|.//div[@class='g']");//20-04-2022 //04-04-2022
                if (nds == null && node.Attributes["class"]?.Value == "g tF2Cxc")//20-04-2022
                    nds = node.SelectNodes(".");//20-04-2022 
                if (nds == null)
                    nds = node.SelectNodes(".//div[contains(@class,'tF2Cxc')]");
                if (nds == null) //25-01-2022
                    nds = node.SelectNodes(".//div[@class='g']|.//div[@class='HD8Pae luh4tb cUezCb xpd O9g5cc uUPGi']|.//div[contains(@class,'g card-section')]");//25-01-2022
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='rc']");
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='gG0TJc']");  //29-05-2020
                //if (nds == null)
                //    nds = node.SelectNodes(".//div[contains(@class,'tF2Cxc')]");//25-01-2022 commented//07-01-2021 //18-02-2021 included contains function
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='yuRUbf']"); //31-05-2021
                if (nds == null)
                    nds = node.SelectNodes(".//div[contains(@class,'dFd2Tb')]|.//div[@class='g ZYT4Gf']");//07-04-2022 //24-08-2021 video block
                if (node.SelectNodes(".//div/div[@class='g jNVrwc Y4pkMc']") != null) //24-08-2021 collecting sub classic links
                    nds = node.SelectNodes(".//div/div[@class='g jNVrwc Y4pkMc']"); //24-08-2021 collecting sub classic links
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
                            if ((Regex.IsMatch(nd.OuterHtml, "id=\"vidthumb\\d*\"") && (nd.SelectSingleNode(".//div[@class='ij69rd UHe5G']") != null || nd.SelectSingleNode(".//div[@class='ij69rd TUOsUe UHe5G']") != null)) || nd.SelectSingleNode(".//div[contains(@class,'U1TUId')]|.//div[@class='J1mWY']") != null)//07-04-2022 //18-10-2021 video block selector
                            {
                                //24-08-2021 video item urls
                                var urls = string.Empty;
                                if (n != null)
                                    urls = n.Attributes["href"].Value;
                                else
                                {
                                    var a = nd.SelectSingleNode(".//div[@class='ct3b9e']/a|.//div[@class='IAZbGe']/a"); //07-04-2022
                                    urls = a.Attributes["href"].Value;
                                    title = a.SelectSingleNode(".//h3");
                                } //24-08-2021 video block item urls
                                if (urls.StartsWith("http") || urls.StartsWith("https") || urls.StartsWith("ftp")) //30-04-2020
                                {
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
                                if (n == null)
                                    n = nd.SelectSingleNode(".//a");//31-05-2021
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
                if (node.SelectSingleNode(".//div[@id='rhs']") != null || node.Attributes["id"]?.Value == "rhs")
                {
                    node = node.SelectSingleNode(".//div[@class='g']|.//div[@class='g Ww4FFb tF2Cxc']|.//div[@class='g eejeod up9jud']"); //19-07-2022
                    if (node == null) return string.Empty;
                }//06-04-2022 //01-04-2022
                HtmlNodeCollection col = node.SelectNodes(".//h3[@class='r']/a");
                if (col == null)
                    col = node.SelectNodes(".//div[@class='r']/a"); // 02-06-2020
                if (col == null)
                    col = node.SelectNodes(".//h3[@class='r dO0Ag']/a");  //29-05-2020
                if (col == null)
                    col = node.SelectNodes(".//div[@class='zTpPx']/g-link/a");  //28-05-2020
                if (col == null)
                    col = node.SelectNodes(".//div[@class='DOqJne']/g-link/a|.//div[@class='M42dy']/g-link/a");//02-02-2022 //07-12-2020 twitter classic link selector
                if (col == null)
                    col = node.SelectNodes(".//div[@class='yuRUbf']/a"); //08-10-2021 for missing classic links
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
                            // string links1 = HttpUtility.UrlDecode(u);
                            //s.Append("<item url=\"" + SetUrl(u) + "\"  title=\"" + SetTitle(nd.InnerText) + "\"  />");//18-01-2022 commented
                            var title = nd.SelectSingleNode(".//h3")?.InnerText ?? nd.InnerText; //18-01-2022
                            s.Append("<item url=\"" + SetUrl(u) + "\"  title=\"" + SetTitle(title) + "\"  />"); //18-01-2022
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
                /*case "maps":
                    s.Append("<block type=\"maps\" url=\"\">");//24-03-2022
                    s.Append(GetMaps(node));
                    s.Append("</block>");//24-03-2022
                    break;*/
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
                /*case "topsights": //23-03-2022
                    s.Append("<block type=\"topSights\" url=\"\">");
                    s.Append(GetTopSights(node));
                    s.Append("</block>");
                    break;
                case "flights":
                    s.Append("<block type=\"flights\" url=\"\">");
                    s.Append(GetFlights(node));
                    s.Append("</block>");
                    break; //23-02-2022*/
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

            HtmlNodeCollection nodes = node.SelectNodes(".//li/div[@class='PwjeAc']"); //15-07-2022
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
            HtmlNodeCollection nds = node.SelectNodes(".//g-inner-card/div/a|.//a[@class='X5OiLe']"); //08-12-2021 videos item urls sel
            if (nds == null)
                nds = node.SelectNodes(".//div[@jsname='ibnC6b']/div/a");   //17-07-2020
            if (nds == null)
                //nds = node.SelectNodes(".//div[@class='LYyupc']/div/a|.//a[@class='X5OiLe']|.//div[@class='XpiUte']/a"); //30-08-2021 videos item url//07-07-2021 //23-07-2021
                nds = node.SelectNodes(".//div[@class='LYyupc']/div/a|.//div[@class='XpiUte']/a"); //08-12-2021 videos item urls //30-08-2021 videos item url//07-07-2021 //23-07-2021
            if (nds != null)
                foreach (HtmlNode nd in nds)
                {
                    try
                    {
                        string title = "";
                        if (nd.Attributes["href"].Value.Contains("/search?num=100")) continue; //08-12-2021 avoid wrong urls
                        HtmlNode n = nd.SelectSingleNode(".//div[@class='Igo7ld mRnBbe QgUve xIqs0b']");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='KiGY3d mB12kf JRhSae ZyAH8d']");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='wCIBKb']/div");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='CwxNSe']/div"); // 02-06-2020
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[contains(@class,'oz3cqf p5AXld')]");//23-07-2021
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='lSegpf']"); //30-08-2021 vides item title
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='fc9yUc tNxQIb ynAwRc OSrXXb']"); //08-12-2021 titles
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
                    HtmlNodeCollection c = nd.SelectNodes(".//h3[@class='r']/a|.//h3[@class='r t9dkOd']/a|.//h3[@class='r X2oHVb t9dkOd']/a|.//h3[@class='r yTjVDd']/a|.//h3/a");//10-07-2021 //04-12-2020//03-06-2020 //01-06-2020 updated selector for sitelinks
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
        private string PeopleAlsoAsk(HtmlNode node) //included item urls code 31-01-2022
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
                nds = node.SelectNodes(".//div[@jsname='jIA8B']"); //08-07-2021
            if (nds == null)
                nds = node.SelectNodes(".//div[@jsname='Cpkphb']"); //30-11-2021 people also ask titles
            if (nds == null)
                return string.Empty;
            string[] titles = new string[nds.Count];//31-01-2022
            int x = 0;
            foreach (HtmlNode nd in nds)
            {
                //s.Append("<item url=\"\" title=\"" + SetTitle(nd.InnerText) + "\" />");
                titles[x++] = nd.InnerText;
            }
            var res = GetPeopleAlsoAskUrls(titles);
            if (string.IsNullOrEmpty(res))
                foreach (var t in titles)
                    s.Append("<item url=\"\" title=\"" + SetTitle(t) + "\" />");
            s.Append(res);//31-01-2022
            return s.ToString();
        }//end of item urls code 31-01-2022*/
        private string GetPeopleAlsoAskUrls(string[] titles) //People also method 31-01-2022
        {
            StringBuilder s = new StringBuilder();
            //string pattern = @"WEB_ANSWERS_STANDARD_RESULT_(.*?)div class\\x3d\\x22tF2Cxc\\x22\\x3e\\x3cdiv class\\x3d\\x22yuRUbf\\x22[ style\\x3d\\x22white-space\Wnowrap\\x22]*\\x3e\\x3ca href\\x3d\\x22(.*?)\\x22"; //29-03-2022
            //string pattern = @"WEB_ANSWERS_STANDARD_RESULT_(.*?)div class\\x3d\\x22tF2Cxc\\x22\\x3e\\x3cdiv class\\x3d\\x22yuRUbf\\x22[ style\\x3d\\x22(white-space\Wnowrap|position:relative)\\x22]*\\x3e\\x3ca href\\x3d\\x22(.*?)\\x22"; //06-05-2022
            string pattern = @"div class\\x3d\\x22tF2Cxc\\x22\\x3e\\x3cdiv class\\x3d\\x22yuRUbf\\x22[ style\\x3d\\x22(white-space\Wnowrap|position:relative)\\x22]*\\x3e\\x3ca href\\x3d\\x22(.*?)\\x22";//07-06-2022 //06-05-2022
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = re.Matches(html);
            ArrayList myList = new ArrayList();
            int x = 0;
            char[] yt = { '\\', '2', '6' };
            foreach (Match m in mc)
            {
                string url = HttpUtility.HtmlDecode(HttpUtility.HtmlEncode(m.Groups[1].Value)); //07-06-2022
                if (url.StartsWith("http") || url.StartsWith("https"))
                {
                    url = SetYTUrl(url, yt); //12-03-2022
                    if (x < titles.Length)//22-02-2022
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(titles[x++]) + "\" />");//22-02-2022
                }
            }
            for (; x < titles.Length; x++)//18-02-2022
                s.Append("<item url=\"\" title=\"" + SetTitle(titles[x]) + "\" />");//18-02-2022
            return s.ToString();
        }
        /*private string GetPeopleAlsoAskUrls(string[] titles) //People also method 02-03-2022
        {
            try
            {
                StringBuilder s = new StringBuilder();
                string pattern = @"WEB_ANSWERS_STANDARD_RESULT_(.*?)div class\\x3d\\x22tF2Cxc\\x22\\x3e\\x3cdiv class\\x3d\\x22yuRUbf\\x22\\x3e\\x3ca href\\x3d\\x22(.*?)\\x22";
                Regex re = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mc = re.Matches(html);
                ArrayList myList = new ArrayList();
                int x = 0;
                char[] yt = { '\\', '2', '6' };
                foreach (Match m in mc)
                {
                    string url = HttpUtility.HtmlDecode(m.Groups[2].Value);
                    if (url.StartsWith("http") || url.StartsWith("https"))
                    {
                        url = SetYTUrl(url, yt); //12-03-2022
                    }
                    //text
                    string textPattern = @"\\x3cspan class\\x3d\\x22hgKElc\\x22\\x3e(.*?)\\x3c/span\\x3e";
                    string imagePattern = @"\\x3cimg data-src\\x3d\\x22(.*?)\\x22";
                    Regex _rx = new Regex(textPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match _m = _rx.Match(m.Groups[0].Value);
                    string txt, text = string.Empty;
                    string imag, img = string.Empty;
                    if (_m.Success)
                    {
                        txt = HttpUtility.HtmlDecode(_m.Groups[1].Value);
                        text = SetYTUrl(txt, yt); //15-03-2022
                    }
                    //image
                    Regex rimage = new Regex(imagePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match mi = rimage.Match(m.Groups[0].Value);
                    if (mi.Success)
                    {
                        imag = HttpUtility.HtmlDecode(mi.Groups[1].Value);
                        img = SetYTUrl(imag, yt); //15-03-2022
                    }
                    //18-03-2022
                    //table
                    string tblPattern = @"\\x3ctable\\x3e\\x3ctbody\\x3e(.*?)\\x3c/tbody\\x3e\\x3c/table\\x3e";
                    _rx = new Regex(tblPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    _m = _rx.Match(m.Groups[0].Value);
                    string tbl = string.Empty;
                    string tblValues = string.Empty;
                    if (_m.Success)
                    {
                        tbl = HttpUtility.HtmlDecode(_m.Groups[1].Value);
                        //rows
                        string rowPattern = @"\\x3ctr(.*?)\\x3c/tr\\x3e";
                        _rx = new Regex(rowPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        MatchCollection _mcrows = _rx.Matches(tbl);
                        tblValues = "<table>";
                        foreach (Match mrow in _mcrows)
                        {
                            string row = HttpUtility.HtmlDecode(mrow.Groups[1].Value);
                            //th
                            string thPattern = @"\\x3cth(.*?)\\x3e(.*?)\\x3c/th\\x3e";
                            _rx = new Regex(thPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection _mcth = _rx.Matches(row);
                            tblValues += "<tr>";
                            foreach (Match mth in _mcth)
                            {
                                string th = HttpUtility.HtmlDecode(mth.Groups[2].Value);
                                tblValues += "<th>";
                                //tblValues += th.Replace(@"\x3cb\x3e", "").Replace(@"\x3c/b\x3e", "");
                                tblValues += SetYTUrl(th, yt);//22-03-2022
                                tblValues += "</th>";
                            }
                            //td
                            string tdPattern = @"\\x3ctd(.*?)\\x3e(.*?)\\x3c/td\\x3e";
                            _rx = new Regex(tdPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            MatchCollection _mctd = _rx.Matches(row);
                            foreach (Match mtd in _mctd)
                            {
                                string td = HttpUtility.HtmlDecode(mtd.Groups[2].Value);
                                tblValues += "<td>";
                                //tblValues += td.Replace(@"\x3cb\x3e", "").Replace(@"\x3c/b\x3e", "");
                                tblValues += SetYTUrl(td, yt); //22-03-2022
                                tblValues += "</td>";
                            }
                            tblValues += "</tr>";
                        }
                        tblValues += "</table>";
                    }

                    if (x < titles.Length)
                    {
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(titles[x++]) + "\" text=\"" + SetTitle(text) + "\" image=\"" + SetUrl(img) + "\" >");
                        if (!string.IsNullOrEmpty(tblValues))
                        {
                            s.Append(tblValues);
                        }
                        s.Append("</item>");
                    }
                    //end 18-03-2022
                }
                for (; x < titles.Length; x++)
                    s.Append("<item url=\"\" title=\"" + SetTitle(titles[x]) + "\" text=\"\" image=\"\" />"); //17-03-2022
                return s.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }*/
        private string GetAnswerCard(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[@class='r']/a");
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='yuRUbf']/a");  //03-09-2020 included selector for missing classic links
            if (nds == null)//23-12-2021
                nds = node.SelectNodes(".//a[@class='GBgvb']");//23-12-2021
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='WcS13d']"); //removed /a //05-10-2020 included selector for missing classic links
            if (nds == null)
                return string.Empty;

            foreach (HtmlNode nd in nds)
            {
                //05-10-2020
                string title = "";

                HtmlNodeCollection nds1 = nd.SelectNodes(".//h3|.//div[@class='wKZW5d']"); //23-12-2021
                if (nds1 != null)
                {
                    title = nd.SelectSingleNode(".//h3|.//div[@class='wKZW5d']").InnerText; //23-12-2021
                    //string url = nd.Attributes["href"].Value; //10-01-2022
                    //if (!url.Contains("/search?num=100")) //10-01-2022
                    s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"" + SetTitle(title) + "\" />");
                }
                nds1 = nd.SelectNodes(".//a");
                if (nds1 != null)
                    foreach (HtmlNode nd1 in nds1)
                    {
                        //string url = nd1.Attributes["href"].Value; //10-01-2022
                        //if (!url.Contains("/search?num=100")) //10-01-2022
                        s.Append("<item url=\"" + SetUrl(nd1.Attributes["href"].Value) + "\" title=\"" + SetTitle(title) + "\" />");
                    }
                //end 05-10-2020
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
                string url = hn.Attributes["href"].Value; //10-01-2022
                if (url.Contains("/search?num=100")) url = string.Empty; //10-01-2022
                s.Append("<block type=\"twitterCards\" url=\"" + SetUrl(url) + "\">"); //10-01-2022
                HtmlNodeCollection nds = node.SelectNodes(".//g-inner-card/div/div[2]/div/g-link/a");
                if (nds == null)
                    nds = node.SelectNodes(".//g-inner-card/div/a"); //21-10-2021 twitter item urls
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
        //09-08-2021 update images item urls
        private string GetImages(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[contains(@class,'eA0Zlc PZPZlf JX86yc ivg-i')]|.//div[@jsname='dTDiAc']"); //11-08-2021 //09-08-2021
            if (nds == null)
                nds = node.SelectNodes(".//g-img/img");
            bool existed = false;
            if (nds != null)
                foreach (HtmlNode nd in nds)
                {
                    //09-08-2021
                    string url = string.Empty;
                    if (nd.Attributes.Contains("data-lpage"))
                    {
                        url = nd.Attributes["data-lpage"].Value.Trim();
                        if (url.StartsWith("//www.")) url = "http:" + url;
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"\" />");
                        existed = true;
                    }//end 09-08-2021
                    else if (nd.Attributes.Contains("title"))    // 24-10-2019
                    {
                        url = nd.Attributes["title"].Value.Trim();
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
            string matchPattern6 = @"\\x22,\W\\x22(.*?)\\\\u0026s\\x22,"; //05-07-2021
            string matchPattern1 = @"]n,\[x22(.*?)x22"; //05-07-2021
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
            //05-07-2021
            re = new Regex(matchPattern6, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            mc = re.Matches(html);
            foreach (Match m in mc)
            {
                string HtmlText = HttpUtility.HtmlDecode(m.Groups[1].Value);
                if (HtmlText.StartsWith("http") || HtmlText.StartsWith("https"))
                {
                    alDup.Add(HtmlText);
                }
            }
            //end 05-07-2021
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
                nds = node.SelectNodes(".//g-inner-card/div/div/a");//03-11-2021 TS item urls
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='HCUNre dbsr']/a"); //21-09-2020 Top Stories block item urls selector updated
            if (nds == null && node.SelectNodes(".//div/a/div[@class='TIh7vf']|.//div/a[@class='WlydOe']") != null) //15-09-2021 selector missing TS item urls
            {    //07-12-2021                                                                                                    //nds = node.SelectNodes(".//div/a"); //10-12-2020 
                nds = node.SelectNodes(".//div/a[@class='WlydOe']");
                if (nds == null)
                    nds = node.SelectNodes(".//div/a");
            }//07-12-2021

            if (nds != null)
                foreach (HtmlNode nd in nds)
                {
                    if (nd.SelectSingleNode(".//div[@class='EUjJDc mtqGb nlNnsd VDgVie']") != null) continue; //25-09-2021 for top stories wrong item urls
                    string title = "";
                    if (nd.Attributes["href"].Value.Contains("/search?num=100")) continue;//08-12-2021 avoid wrong urls
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
                    string itemURL = nd.Attributes["href"].Value; //04-10-2021
                                                                  // if (!itemURL.Contains("/search?num=100"))//04-10-2021
                    s.Append("<item url=\"" + SetUrl(itemURL) + "\" title=\"" + SetTitle(title) + "\" />");//04-10-2021
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


        private string GetCarouselURLs()
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
        private string GetTopSights(HtmlNode node)//23-03-2022 new element top sights
        {
            StringBuilder s = new StringBuilder();
            //top
            HtmlNodeCollection nds = node.SelectNodes(".//div[contains(@class,'EDblX DAVP1')]/a");
            s.Append("<top>");
            foreach (HtmlNode nd in nds)
            {
                string url = nd.Attributes["href"].Value;
                string title = nd.InnerText;
                if (url.StartsWith("/"))
                    url = "https://www.googole.com" + url;
                if (!string.IsNullOrEmpty(url) || !string.IsNullOrEmpty(title))
                    s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
            }
            s.Append("</top>");
            //bottom
            nds = node.SelectNodes(".//div[@class='rqTuzc']/a");
            s.Append("<bottom>");
            foreach (HtmlNode nd in nds)
            {
                string url = nd.Attributes["href"].Value;
                string title = nd.SelectSingleNode(".//span[@class='aVSTQd tNxQIb OSrXXb']").InnerText;
                if (url.StartsWith("/"))
                    url = "https://www.googole.com" + url;
                if (!string.IsNullOrEmpty(url) || !string.IsNullOrEmpty(title))
                    s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
            }
            s.Append("</bottom>");
            return s.ToString();
        }//23-03-2022
        private string GetFlights(HtmlNode node)//23-03-2022 new element flights
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[@class='aieQre']/div/a");
            foreach (HtmlNode nd in nds)
            {
                try
                {
                    string airline = nd.SelectSingleNode(".//span[@class='ps0VMc']")?.InnerText.Trim() ?? "";
                    string hours = nd.SelectSingleNode(".//span[@class='sRcB8']")?.InnerText.Trim() ?? "";
                    string connecting = nd.SelectSingleNode(".//span[@class='u85UCd']")?.InnerText.Trim() ?? "";
                    string price = nd.SelectSingleNode(".//span[@class='xqqLDd']")?.InnerText.Trim() ?? "";
                    s.Append("<item airline=\"" + SetTitle(airline) + "\" hours=\"" + SetTitle(hours) + "\" connecting=\"" + SetTitle(connecting) + "\" price=\"" + price + "\" />");
                }
                catch { }
            }
            return s.ToString();
        }//23-03-2022 
        private string GetMaps(HtmlNode node)//24-03-2022 new element Maps
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[@class='hmHBZd jvOlt']");
            foreach (HtmlNode nd in nds)
            {
                try
                {
                    string title = nd.SelectSingleNode(".//div[@class='BTPx6e bOYhNc']")?.InnerText.Trim() ?? "";
                    string rating = nd.SelectSingleNode(".//span[@class='YDIN4c YrbPuc']")?.InnerText.Trim() ?? "";
                    string price = nd.SelectSingleNode(".//span[@class='dv1Q3e']")?.InnerText.Trim() ?? "";
                    s.Append("<item price=\"" + SetTitle(price) + "\" rating=\"" + SetTitle(rating) + "\" title=\"" + SetTitle(title) + "\" />");
                }
                catch { }
            }
            return s.ToString();
        }//24-03-2022
        private string GetBlockType(HtmlNode node)
        {
            HtmlNode nd = node.SelectSingleNode(".//div[@class='_ELb']/a");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='twQ0Be']/a");
            if (nd != null)
            {
                return "VideoCard";
            }
            nd = node.SelectSingleNode(".//div[@class='KNcnob']/g-img|.//div[contains(@class,'YEMaTe')]/g-img");//10-12-2020 applied contains   //01-05-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='fn6bCb']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='qmv19b']");    // 09-12-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div/a/div[@class='TIh7vf']"); // 10-12-2019 topstories block type selector
            if (nd == null)
                nd = node.SelectSingleNode(".//div/a/div[@class='vJOb1e']"); //20-10-2021 top stories block
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='CEMjEf NUnG9d']/g-img"); //06-04-2022 TS
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
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@jsname='wRSfy']"); //02-12-2020 included for videos block
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
            /*nd = node.SelectSingleNode(".//g-tray-header[@class='kno-fb-ctx gsrt AX8YBc']");//23-03-2022 TopSights and Flights
            if (nd != null)
                return "TopSights";//23-03-2022

            nd = node.SelectSingleNode(".//div[@class='WlTAzf mnr-c']");//23-03-2022
            if (nd != null)
                return "Flights";//23-03-2022*/

            //nd = node.SelectSingleNode(".//div[@class='kp-blk cUnQKe']|.//div[@class='kp-blk cUnQKe Wnoohf OJXvsb']|.//div[@jsname='N760b']");//08-07-2021//04-12-2020 //11-02-2020
            nd = node.SelectSingleNode(".//div[contains(@class,'cUnQKe')]|.//div[@jsname='N760b']");//02-08-2021//08-07-2021
            if (nd != null)
            {
                if (node.SelectSingleNode(".//div[@class='AuVD KJ7Tg cUnQKe']") == null) //20-07-2022
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
                || node.SelectSingleNode(".//div[@class='HaXvv kfn9hb']") != null || node.SelectSingleNode(".//div[@class='tsp-view']") != null //24-11-2020 selector for eventresults block//07-02-2020
                || node.SelectSingleNode(".//div[@class='tsp-fvcfc']") != null) //02-08-2021 event block selector
                return "Event";

            if (node.SelectSingleNode(".//div[@id='cwmcwd']") != null || node.SelectSingleNode(".//div[@class='ifM9O']") != null
                || node.SelectSingleNode(".//div[@class='vk_ard']") != null || node.SelectSingleNode(".//div[@class='d7sCQ kp-header']") != null   //03-06-2020
                || node.SelectSingleNode(".//div[@class='pcCUmf vCOSGb']") != null
                || node.SelectSingleNode(".//div[@class='vkc_np kkww4d']") != null //21-09-2020 updated answered card selectors  //03-06-2020
                || node.SelectSingleNode(".//div[@class='setTDc']") != null //07-12-2020 answered card selector
                || node.SelectSingleNode(".//div[@class='kp-blk ouUsKb G45kvd']") != null //24-08-2021 answer card selector
                || node.SelectSingleNode(".//div[@class='M0XuFe mnr-c vk_c']") != null //23-12-2021
                || node.SelectSingleNode(".//div[@id='wob_wc']") != null //06-04-2022//31-03-2022 
                || node.SelectSingleNode(".//div[@class='YXEKBb']") != null //06-06-2022 Answered Card
                || node.SelectSingleNode(".//div[@class='ellip vk_h lxZILe']|.//div[contains(@class, 'vk_c')]|.//div[contains(@class,'lr_container')]") != null)//17-06-2022 //16-06-2022
            {
                if (node.SelectSingleNode(".//div[@class='BET1rd']|.//div[@class='EfDVh wDYxhc NFQFxe']") == null
                    && node.SelectSingleNode(".//div[@class='tpa-cc']") == null && node.SelectSingleNode(".//g-scrolling-carousel[@class='VXdDm']") == null //22-06-2022//23-12-2021 //21-08-2021 //25-09-2020
                    && node.SelectSingleNode(".//*[@id='lu_map']|.//img[contains(@alt,'Map of')]|.//a[contains(@data-url,'/maps/')]") == null) //22-07-2022 //13-10-2021
                    return "AnswerCard";
                else if (node.SelectSingleNode(".//div[@class='WcS13d']") != null && node.SelectSingleNode(".//div[@class='EfDVh wDYxhc NFQFxe']") != null //27-12-2021
                    || node.SelectSingleNode(".//div[@class='nmVgI3FLyE0__answer']") != null
                    || node.SelectSingleNode(".//div[@class='N6Sb2c i29hTd']") != null)//16-06-2022 //23-03-2022
                    return "AnswerCard";//27-12-2021
            }
            //05-10-2020 KP Block selectors updated
            nd = node.SelectSingleNode(".//div[@class='kp-wholepage EyBRub kp-wholepage-osrp HSryR']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kp-wholepage kp-wholepage-osrp HSryR EyBRub']");
            if (nd != null && node.SelectSingleNode(".//div[@class='q6PGbe']") == null && node.SelectSingleNode(".//div[@class='l44Vof']") == null && node.SelectSingleNode(".//div[@class='P9Jfrb']") == null && node.SelectSingleNode(".//div[@class='LnbJhc']") == null && node.SelectSingleNode(".//div[@class='H93uF']") == null) //17-05-2022 //18-03-2022
                return "KnowledgePanel";
            //end 05-10-2020

            nd = node.SelectSingleNode(".//div[@class='DUU6i']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='LnbJhc']");  //16-01-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@jscontroller='IkchZc']");//13-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@jscontroller='hFvNdd']");//08-10-2021 images
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kno-fiu kno-liu']"); //07-12-2021 for images block
            //if (nd != null && node.Attributes["id"]?.Value != "Odp5De" && node.SelectSingleNode(".//div[@class='q6PGbe']") == null && node.SelectSingleNode(".//div[@class='l44Vof']") == null && node.SelectSingleNode(".//div[@class='P9Jfrb']") == null)//09-03-2022//31-12-2021
            if (nd != null && node.Attributes["id"]?.Value != "Odp5De" && node.SelectSingleNode(".//div[@class='q6PGbe']|.//div[@class='l44Vof']|.//div[@class='P9Jfrb']|.//div[@class='o8ebK']") == null) //02-06-2022
            {
                return "Images";
            }

            nd = node.SelectSingleNode(".//*[@id='lu_map']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='xERobd']|.//div[@class='H93uF']|.//div[@class='CH6Bmd']|.//div[@class='vs2hJf']|.//div[@class='o8ebK']");//02-06-2022//27-05-2022//05-03-2022//22-01-2022  //changed on 26-06-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//a[contains(@data-url,'/maps/')]");//23-08-2021 map selector
            if (nd == null)
                nd = node.SelectSingleNode(".//img[contains(@alt,'Map of')]|.//div[@jscontroller='TVzfQb']");//21-02-2022//27-12-2021 maps
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

            nd = node.SelectSingleNode(".//div[@id='kx']|.//div[@id='rrg']|.//div[@class='oHJrJb']|.//div[@class='q6PGbe']");//03-11-2021 CS Block//05-08-2020 included selector for carousel
            if (nd != null)
            {
                return "Carousel";
            }
            nd = node.SelectSingleNode(".//div[@id='fac-ut']|.//div[@class='dzpFPb']");//06-04-2022
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@id='knowledge-finance-wholepage__entity-summary']");
            if (nd == null)
                //nd = node.SelectSingleNode(".//div[@id='knowledge-currency__currency-v2-updatable']");
                nd = node.SelectSingleNode(".//div[contains(@id,'knowledge-currency__')]"); // contains 31-07-2021
            if (nd == null)
                //nd = node.SelectSingleNode(".//div[@class='g obcontainer']");   // updated on 01-08-2019
                nd = node.SelectSingleNode(".//div[contains(@class,'obcontainer')]"); //02-11-2021 finance block
            if (nd == null)
                nd = node.SelectSingleNode(".//div[contains(@class,'kno-fb-ctx')]"); //13-10-2021
            if (nd != null)
            {
                if (node.SelectSingleNode(".//div[@class='NhRr3b']") != null)//17-06-2022
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
                || node.SelectSingleNode(".//div[contains(@class, 'e2BEnf U7izfe')]") != null //28-07-2021 images selectors
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
                || node.SelectSingleNode(".//div[@class='d7sCQ kp-header']") != null  //03-06-2020                                                      
                                                                                      //|| node.SelectSingleNode(".//div[@class='vk_c card-section']") != null // answer card                
                || node.SelectSingleNode(".//div[@class='pcCUmf vCOSGb']") != null  //03-06-2020
                                                                                    //|| node.SelectSingleNode(".//div[@class='kp-blk cUnQKe Wnoohf OJXvsb']") != null  // people also ask // 11-02-2020
                || node.SelectSingleNode(".//div[contains(@class,'cUnQKe')]") != null //08-07-2021
                                                                                      //|| node.SelectSingleNode(".//span[@data-original-name='People also ask']") != null  // people also ask
                || node.SelectSingleNode(".//h3[@class='_DM']") != null || node.SelectSingleNode(".//div[@id='imagebox_bigimages']") != null || node.SelectSingleNode(".//div[@class='mR2gOd']") != null //27-06-2020   // images
                || node.SelectSingleNode(".//div[@class='e2BEnf']/h3") != null // videos
                || node.SelectSingleNode(".//div[@class='e2BEnf U7izfe']/h3") != null  // videos
                || node.SelectSingleNode(".//div[@class='mod NFQFxe oHglmf xzPb7d']") != null//images//05-08-2020
                || node.SelectSingleNode(".//div[@id='knowledge-finance-wholepage__entity-summary']") != null // 18-03-2020
                || node.SelectSingleNode(".//div[@class='I6TXqe osrp-blk']") != null //12-08-2020 included selector for video card
                || node.SelectSingleNode(".//div[@class='WcS13d']") != null //02-10-2020 maps selectors
                || node.SelectSingleNode(".//h3[@class='GmE3X']") != null //16-10-2020 updated selector for videos
                || node.SelectSingleNode(".//div[@class='twQ0Be']") != null //03-12-2020 updated selector for videocard
                || node.SelectSingleNode(".//div[@class='vwfsqc']") != null //07-12-2020
                || node.SelectSingleNode(".//div[@class='setTDc']") != null //07-12-2020
                || node.SelectSingleNode(".//div[@class='HnYYW']/div") != null //23-07-2021
                || node.SelectSingleNode(".//div[@class='e2BEnf mfMhoc']") != null //25-09-2021 missing top stories
                || node.SelectSingleNode(".//div[@class='e2BEnf']") != null //04-10-2021 top stories
                || node.SelectSingleNode(".//div[@jscontroller='hFvNdd']") != null//13-10-2021
                || node.SelectSingleNode(".//div[@class='g jNVrwc Y4pkMc']") != null //07-12-2021
                || node.SelectSingleNode(".//div[@class='e2BEnf q8U8x']") != null //07-12-2021
                || node.SelectSingleNode(".//div[@jsname='wRSfy']") != null) //07-12-2021
                || node.SelectSingleNode(".//div[@class='e2BEnf axf3qc q8U8x']") != null//29-12-2021 top stories
                || node.SelectSingleNode(".//div[@class='WlTAzf mnr-c']") != null; //23-03-2022
                                                                                   //&& node.SelectSingleNode(".//div[@class='yuRUbf']") == null; //11-10-2021 //08-10-2021 images
            if (bVal == true)//2019-09-11
            {
                try
                {
                    if (node.SelectSingleNode(".//div[@class='g jNVrwc Y4pkMc']|.//div[@class='g tF2Cxc']|.//div[@class='g eejeod up9jud']" +
                    "|.//div[@class='g Ww4FFb tF2Cxc']") != null) return false; //21-07-2022//15-02-2022//02-02-2022//31-12-2021 missing CLinks
                    if (node.SelectSingleNode(".//div[@class='twQ0Be']|.//div[@jsname='N760b']|.//div[@jsname='wRSfy']|.//div[contains(@class,'e2BEnf U7izfe')]|.//div[@jsname='A6RGif']|.//div[@class='P9Jfrb']") != null) return true;//08-03-2022//07-03-2022//28-12-2021//10-12-2021//09-12-2021 //08-12-2021 PAlsoB   //30-08-2021 video card
                    if (node.SelectSingleNode(".//div[@class='osrp-blk']|.//div[@class='tpa-cc']") != null && node.SelectSingleNode(".//div[@class='l44Vof']") == null && node.SelectSingleNode(".//div[@class='H93uF']") == null) //17-05-2022//31-12-2021
                        return false; //20-08-2021
                    if (node.Attributes["id"]?.Value == "rhs") return false;//03-03-2022
                    //02-12-2020
                    HtmlNode nd = node.SelectSingleNode(".//div[@role='heading']|.//div[@class='UDZeY OTFaAf']"); //02-07-2021

                    if (nd != null && (nd.InnerText == "More results" || nd.InnerText == "Top results" || nd.InnerText.Contains("Web results"))) //02-07-2021 //03-12-2020
                        return false;
                    //end 02-12-2020

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

                    if (node.SelectSingleNode(".//div[contains(@class,'kp-blk')]|.//div[contains(@class,'c2xzTb')]") != null || node.Attributes["class"]?.Value == "kp-blk c2xzTb")//16-02-2022//23-12-2021//07-12-2020 for answercard block 
                        return true;//22-12-2021 commented
                    //if (node.SelectSingleNode(".//div[@class='kp-blk Wnoohf OJXvsb']") != null)//22-12-2021
                    //    return false;
                    //else
                    //    return true;//22-12-2021
                    //28-05-2021
                    if (node.SelectSingleNode(".//div[@class='g']") != null)
                        if (node.SelectSingleNode(".//table[@class='nrgt']") != null || node.SelectSingleNode(".//table[@class='jmjoTe']") != null)  // 28-05-2021
                            return true;
                    //28-05-2021 ends

                    if (node.SelectSingleNode(".//div[@class='g']") != null) //04-12-2020 select for class links
                        return false;
                    if (node.SelectSingleNode(".//div[@class='d3zsgb']|.//div[@class='lMMUFc']") != null) //18-07-2022 wrong answer card //13-10-2021
                        return false;
                }
                catch { }
            }

            if (!bVal)
            {
                HtmlNode nd = node.SelectSingleNode(".//h3|.//div[contains(@class,'HnYYW')]|.//div[@class='LMMXP i8lZMc']|.//div[@class='e2BEnf U7izfe']/div|.//div[@class='LMMXP mfMhoc']"); //05-08-2020 included contains function  //17-07-2020 //03-06-2020  // 02-06-2020    //01-05-2020
                if (nd != null)
                    if (nd.InnerText == "Top stories" || nd.InnerText == "Huvudnyheter" || nd.InnerText == "Videos" || nd.InnerText == "Video" || nd.InnerText == "Tin bài hàng đầu" || nd.InnerText == "Voorpaginanieuws" || nd.InnerText == "Vertaalresultaat" || nd.InnerText == "Recipes" || nd.InnerText == "Vidéos")//02-12-2020 videos//05-08-2020 //29-06-2020//03-06-2020 // 02-06-2020  // 08-04-2020
                        return true;

                if (node.SelectSingleNode(".//div[contains(@class,'kp-blk')]") != null || node.SelectSingleNode(".//div[@class='dzpFPb']") != null)//28-05-2022//06-04-2022 //13-10-2021
                    return true;

                // changes in map block on 19-06-2019.
                nd = node.SelectSingleNode(".//g-img/img");
                if (nd != null)
                {
                    if (nd.Attributes["alt"].Value.StartsWith("Map of ") || node.SelectSingleNode(".//div[@class='H93uF']") != null) //21-04-2022
                        return true;
                    if (node.SelectSingleNode(".//div[@class='U1TUId LYh3vc']") != null) //16-12-2021
                        return false; //16-12-2021
                }
                // changes on 08-07-2019
                if (node.SelectSingleNode(".//img[@alt='map image']") != null || node.SelectSingleNode(".//div[@jsname='N760b']|.//div[@class='kno-mrg kno-swp']|.//div[@class='e4xoPb']|.//div[@class='H93uF']") != null || node.SelectSingleNode(".//img[contains(@data-bsrc,'/maps/')]") != null)//22-01-2022//23-08-2021 map selector//02-08-2021
                    if (node.SelectSingleNode(".//div[@class='tF2Cxc']|.//div[@class='jtfYYd']") != null || node.Attributes["id"]?.Value == "rhs" || node.SelectSingleNode(".//div[contains(@class, 'rhsg')]") != null)//07-04-2022 //17-02-2022//16-12-2021 //10-12-2021
                        return false;//10-12-2021
                    else //10-12-2021
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
                || node.SelectSingleNode(".//div[@class='rc']") != null // 03-09-2020 missing classic links selector included
                || node.SelectSingleNode(".//div[@class='DOqJne']/g-link/a") != null //twitter classic link selector
                || node.SelectSingleNode(".//div[contains(@class,'tF2Cxc')]/div/a") != null //07-01-2021 missing classic link //18-02-2021 included contains fucntions
                || node.SelectSingleNode(".//div[@class='yuRUbf']") != null //31-05-2021
                || node.SelectSingleNode(".//div/div[@class='g tF2Cxc']|.//div[contains(@class,'g dFd2Tb')]|.//div[@class='g ZYT4Gf']") != null//13-07-2022 //07-04-2022//24-08-2021 video block //01-06-2021
                || node.SelectSingleNode(".//div[@class='M42dy']/g-link/a") != null); //02-02-2022 twitter link
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
                    // if (!url.StartsWith("/")) //11-09-2021 ignore url start with "/"
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

                if ((url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("ftp://")) && !url.Contains("/aclk?"))// 30-04-2020 
                    return url;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return string.Empty;
        }
        /// <summary>
        /// peoplealsoask block url and youtube urls cleaning method
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string SetYTUrl(string url, char[] yt)//12-03-202
        {

            if (string.IsNullOrEmpty(url)) return string.Empty;
            if (url.Contains(@"\x3d"))
                url = url.Replace(@"\x3d", "="); //10-03-2022
            if (url.Contains("youtube.com"))
            {
                int n = url.IndexOfAny(yt);
                if (n > 0)
                    url = url.Replace(url.Remove(0, n), "");//11-03-2022
            }
            if (url.Contains(@"\u003d"))
                url = url.Replace(@"\u003d", "="); //22-03-2022 \u003d

            if (url.Contains(@"\u0026"))
                url = url.Replace(@"\u0026", "&"); //22-03-2022

            if (url.Contains(@"\x26"))
                url = url.Replace(@"\x26amp;", "&"); //10-03-2022\x26#39;

            if (url.Contains(@"\x26#39;"))
                url = url.Replace(@"\x26#39;", "'"); //17-03-2022

            if (url.Contains(@"\x27"))
                url = url.Replace(@"\x27", "'"); //11-03-2022

            if (url.Contains(@"\x3cb\x3e"))
                url = url.Replace(@"\x3cb\x3e", ""); //15-03-2022 for text

            if (url.Contains(@"\x3cbr\x3e"))
                url = url.Replace(@"\x3cbr\x3e", ""); //21-03-2022 for text

            if (url.Contains(@"\x3c/b\x3e"))
                url = url.Replace(@"\x3c/b\x3e", ""); //17-03-2022 for text
            if (url.Contains(@"\x26quot;"))
                url = url.Replace(@"\x26quot;", "\"");//21-03-2022
            if (url.Contains(@"\u201c"))
                url = url.Replace(@"\u201c", "“"); //22-03-2022
            if (url.Contains(@"\u201d"))
                url = url.Replace(@"\u201d", "”"); //22-03-2022

            return WebUtility.HtmlEncode(WebUtility.HtmlDecode(url));
        }//12-03-2022 end

        // There are chances method was used for title contains in case any issues in xml applied decode/encode.
        public string SetTitle(string unicodestring)
        {
            //return WebUtility.HtmlEncode(WebUtility.HtmlDecode(unicodestring));
            return WebUtility.HtmlEncode(WebUtility.HtmlDecode(unicodestring)).Replace("\\x27", "'").Replace("\\\\u0026", "&amp;").Replace("\\\\\\x22", "&quot;").Replace("\\u2013", "–"); //17-03-2022
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