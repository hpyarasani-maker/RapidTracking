using HtmlAgilityPack;
using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TrackingTrending
{
    class iOS 
    {
        int orgLinks;
        string html;

        public string ProcessDocument(string seid, string keyword, HtmlDocument doc, out int count)
        {
            count = 0;

            if (doc == null)  return string.Empty;           

            orgLinks = 0;
            html = doc.DocumentNode.OuterHtml;
            StringBuilder sb = new StringBuilder();
            sb.Append("<searchResult searchEngine=\"" + seid + "\" keyword=\"" + WebUtility.HtmlEncode(keyword) + "\" date=\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\" >");
            sb.Append("<section col=\"main\">");
            string topStuff = GetTopStuff(doc);
            sb.Append(topStuff);

            HtmlNodeCollection nodeCol = doc.DocumentNode.SelectNodes("//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']/div[@class='MUxGbd v0nnCb lyLwlc']|//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']/div/div[@class='MUxGbd v0nnCb lyLwlc']");   //29-04-2020
            if (nodeCol != null)
                nodeCol = nodeCol[nodeCol.Count - 1].SelectNodes("a/div");  //28-04-2020
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div|//div[@id='rso']/g-card|//div[@id='taw']/div[@class='med']/div[2]/div|//div[@id='rso']/nav");   //28-04-2020
            if (nodeCol != null && nodeCol.Count == 1)
                nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div|//div[@class='vC5Ym DhKAUb']/div");    //17-09-2019
            if (nodeCol == null)
                nodeCol = doc.DocumentNode.SelectNodes("//*[@id='tscffb']");
            //if (nodeCol == null)
            //if (nodeCol == null)
            //    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='ires']/ol/div");

            if (nodeCol == null) return string.Empty; 

            string ndText = "";

            foreach (HtmlNode node in nodeCol)
            {
                HtmlNode fsh = node.SelectSingleNode(".//*[@id='knowledge-finance-wholepage__fw-sticky-header']");
                if (fsh != null)
                {
                    HtmlNodeCollection nc = node.SelectNodes(".//*[@class='knowledge-finance-wholepage__section wp-ms']"); // /div[1]
                    if (node.HasClass("kp-wholepage"))
                    {
                        continue;
                    }
                    try
                    {
                        if (nc != null)
                        {
                            foreach (HtmlNode nd in nc)
                            {
                                string s = ProcessNode(nd);
                                ndText += s;
                                if (s.Length > 0)
                                    sb.Append(s);
                            }
                            break;
                        }
                    }
                    catch { }

                }
                //changes on 06-08-2019
                if ((node.SelectSingleNode(".//div[@id='knowledge-finance-wholepage__entity-summary']") != null
                    || node.InnerText.Contains("Finance results")) && node.SelectSingleNode(".//div[@class='srg']") != null)
                {
                    sb.Append("<block type=\"finance\" url=\"\"></block>");
                }

                if (node.HasClass("kp-wholepage") || node.SelectNodes(".//div[contains(@class, 'kp-wholepage')]") != null)
                {
                    // changed on 05-07-2019
                    HtmlNode n = node.SelectSingleNode(".//div[@class='PyJv1b kno-fb-ctx gsmt PZPZlf']/span[@role='heading']");
                    if (n == null)
                        n = node.SelectSingleNode(".//div[@class='PyJv1b kno-fb-ctx gsmt PZPZlf lV8Nyd']/span[@role='heading']");
                    if (n == null)
                        n = node.SelectSingleNode(".//div[@class='PyJv1b gsmt PZPZlf']/span[@role='heading']"); // 06-11-2019
                    if (n == null)
                        n = node.SelectSingleNode(".//div[@class='PyJv1b gsmt PZPZlf lV8Nyd']/span[@role='heading']"); // 11-11-2019
                    if (n == null)
                        n = node.SelectSingleNode(".//div[@class='Ftghae iirjIb']");//16-09-2019 //
                    if (n != null)
                    {
                        string heading = n.InnerText;
                        sb.Append("<block type=\"knowledgeGraph\" url=\"\" title=\"" + SetTitle(heading) + "\" />");
                    }

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
                catch
                { }
            }

            if (string.IsNullOrEmpty(ndText.Trim()) || orgLinks == 0)
            {
                foreach (HtmlNode node in nodeCol)
                {
                    try
                    {
                        if (node.HasClass("kp-wholepage") || node.SelectNodes(".//div[contains(@class, 'kp-wholepage')]") != null)
                        {
                            HtmlNodeCollection nc = node.SelectNodes(".//div[@id='kp-wp-tab-overview']/div"); //22-01-2020
                            if (nc == null)
                                nc = node.SelectNodes(".//div[@class='WvKfwe']/div|.//div[@class='WvKfwe a3spGf']/div|.//div[@class='ChlgHf']|.//div[@class='UDZeY mf8UVb']|.//div[@class='uxUO1b g0S8Ze mnr-c']"); //17-06-2020 answer card  //01-06-2020");  //15-04-2020     
                            if (nc == null) //|.//div[@class='a3spGf WvKfwe']/div //23-05-2020
                                nc = node.SelectNodes(".//div[@class='Kot7x eXEBMb Znsfnf']/div[@class='GhpATe pttBJc']|.//div[@class='a3spGf WvKfwe']/div"); //15-04-2020
                            if (nc == null)//|.//div[@class='kp-blk c2xzTb OJXvsb']//23-05-2020
                                nc = node.SelectNodes(".//div[@class='UDZeY']/div|.//div[@class='vC5Ym']/div|.//div[@class='kp-blk cUnQKe Wnoohf OJXvsb']|.//div[@class='kp-blk c2xzTb OJXvsb']");       //23-05-2020
                            if (nc == null)
                                nc = node.SelectNodes(".//div[@class='MRWHue']");
                            if (nc == null)
                                nc = node.SelectNodes(".//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']/div"); //22-01-2020
                            if (nc == null)
                                nc = node.SelectNodes(".//div[@class='a3spGf WvKfwe']");
                            foreach (HtmlNode nd in nc)
                            {
                                if (nd.InnerHtml != "")
                                {
                                    string s = string.Empty;
                                    try
                                    {
                                        s = ProcessNode(nd);
                                    }
                                    catch { }
                                    ndText += s;
                                    if (s.Length > 0)
                                        sb.Append(s);
                                }
                            }
                            break;
                        }
                    }
                    catch(WebException ex) { return ex.Message.ToString(); }
                }
            }

            string bottomStuff = GetBottomStuff(doc);
            sb.Append(bottomStuff);
            sb.Append("</section>");

            sb.Append("<section col=\"right\">");
            string rightStuff = GetRightStuff(doc);
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
            // product listed ads
            HtmlNode rcNode = doc.DocumentNode.SelectSingleNode("//div[@id='rhs_block']");
            if (rcNode == null)
                return string.Empty;

            HtmlNode sNode = rcNode.SelectSingleNode(".//div[@class='cu-container']");
            if (sNode != null)
            {
                s.Append("<block type=\"productListedAds\" url=\"\">");
                //HtmlNodeCollection col = sNode.SelectNodes(".//a[@class='plantl pla-unit-title-link']|.//div[@class='mnr-c pla-unit']/a[2]|.//a[@class='plantl pla-unit-single-clickable-target clickable-card']");
                //if (col == null)
                HtmlNodeCollection col = sNode.SelectNodes(".//div[@class='_Ead']/a[2]");
                if (col != null)
                {
                    foreach (HtmlNode nd in col)
                    {
                        var url = nd.Attributes["href"].Value.Trim();
                        url = GetRedirectedUrl(url);
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(nd.InnerText) + "\" />");
                    }
                }
                s.Append("</block>");
            }

            // kp

            //if (rcNode.SelectSingleNode(".//div[@class='kp-header']") != null)     //'kp-blk knowledge-panel _Rqb _RJe']") != null)
            //{
            //    s.Append("<block type=\"knowledgeGraph\" url=\"\" />");
            //}


            return s.ToString();
        }

        private string GetBottomStuff(HtmlDocument doc)
        {
            StringBuilder s = new StringBuilder();
            //25-09-2019                        //swaped productlistedads 14-05-2020
            if (doc.DocumentNode.SelectSingleNode("//div[@class='mnr-c IGtt6d imgac']") != null)
            {
                HtmlNode pla = doc.DocumentNode.SelectSingleNode(".//div[contains(@class, 'commercial-unit-mobile-top')]");
                if (pla == null)
                    pla = doc.DocumentNode.SelectSingleNode(".//div[contains(@class, 'commercial-unit-mobile-bottom')]");   // 18-09-2018
                if (pla != null)
                {
                    HtmlNode h3 = pla.SelectSingleNode(".//div[@class='dxR8gf']/h3");
                    if (h3 == null)
                        h3 = pla.SelectSingleNode(".//div[@class='richlist-top-shopping-title']/h3");
                    if (h3 == null)
                        h3 = pla.SelectSingleNode(".//div[@class='gsrt richlist-top-shopping-title']/h3");
                    if (h3 == null)
                        h3 = pla.SelectSingleNode(".//div[@class='gsrt dxR8gf']");

                    if (h3 != null)
                    {
                        if ((h3.SelectSingleNode(".//h3[contains(@class, 'r')]") != null && h3.SelectSingleNode(".//h3[@role='heading']") != null) ||   //12-09-2019 21-05-2020 included productlist ads
                         (h3.InnerText.StartsWith("Shop for") || h3.InnerText.StartsWith("See ") || h3.InnerText.StartsWith("Ver ")))
                        {
                            s.Append("<block type=\"productListedAds\" url=\"\">");

                            HtmlNodeCollection cl = pla.SelectNodes(".//a[@class='pla-unit eUPzHb']|.//div[@class='mnr-c pla-unit']/a[2]|.//a[@class='plantl pla-unit-single-clickable-target clickable-card']");
                            if (cl == null)
                                cl = pla.SelectNodes(".//a[@class='pla-unit']");
                            if (cl != null)
                            {
                                foreach (HtmlNode nd in cl)
                                {
                                    var url = nd.Attributes["href"].Value;
                                    url = GetRedirectedUrl(url);
                                    if (!string.IsNullOrEmpty(nd.SelectSingleNode(".//h4").InnerText) && !string.IsNullOrEmpty(url.Trim()))    //13-11-2019
                                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(nd.SelectSingleNode(".//h4").InnerText) + "\" />");
                                }
                            }
                            //09-09-2019
                            else if (doc.DocumentNode.SelectNodes(".//div[@class='RL6uuc gws-product_ads-showcase_immersive__immersive-tile']//div[@class='PhX95']") != null)
                            {
                                HtmlNodeCollection hidedNodes = doc.DocumentNode.SelectNodes(".//div[@class='RL6uuc gws-product_ads-showcase_immersive__immersive-tile']");
                                HtmlNode urlnode, innertextNode;
                                if (hidedNodes != null)
                                {
                                    foreach (HtmlNode planode in hidedNodes)
                                    {
                                        urlnode = planode.SelectSingleNode(".//div[@class='PhX95']");
                                        innertextNode = planode.SelectSingleNode(".//div[@class='Ved4gc']");
                                        //11-11-2019
                                        string url = GetProductListedUrls(urlnode.InnerText.ToString());
                                        if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(innertextNode.InnerText))    //13-11-2019
                                            s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(innertextNode.InnerText) + "\" />");
                                    }
                                }
                            }
                            //else
                            //{
                            //    cl = pla.SelectNodes(".//div[@class='zgXJce']");
                            //    if (cl == null)
                            //        cl = pla.SelectNodes(".//div[@class='JsMaPc']");
                            //    if (cl != null)
                            //    {
                            //        foreach (HtmlNode nd in cl)
                            //        {
                            //            try
                            //            {
                            //                s.Append("<item url=\"\" title=\"" + SetTitle(nd.InnerText) + "\" />");
                            //            }
                            //            catch { }
                            //        }
                            //    }
                            //}
                            s.Append("</block>");
                        }
                    }
                }
            }//25-09-2019
            // Ads  added or condition
            // HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//div[@id='tadsb']/ol/li|//div[@id='tads']/ol/li");
            HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//div[@id='tadsb']/div[@class='C4eCVc c']/ol/li");  // 24-04-2020   included class selector
            if (col != null)
            {
                s.Append("<block type=\"adwords\" url=\"\">");
                foreach (HtmlNode nd in col)
                {
                    HtmlNode n = nd.SelectSingleNode(".//div[@class='d5oMvf KJDcUb']/a");
                    if (n == null)
                        n = nd.SelectSingleNode(".//div[@class='d5oMvf KJDcUb WzRKRb']/a"); // 17-12-2019
                    if (n == null)
                        n = nd.SelectSingleNode(".//div/a[2]|.//div/div/div[@class='d5oMvf']/a");
                    if (n != null)
                    {
                        string title = n.SelectSingleNode(".//h3|.//div[@role='heading']").InnerText;
                        if (!string.IsNullOrEmpty(SetUrl(n.Attributes["href"].Value))) // 12-06-2020
                        {
                            var url = n.Attributes["href"].Value.Trim();
                            //url = GetRedirectedUrl(url);  // 12-06-2020
                            s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                        }
                        else
                        {
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


        private string GetTopStuff(HtmlDocument doc)
        {
            StringBuilder s = new StringBuilder();

            // carousel
            HtmlNode crNode = doc.DocumentNode.SelectSingleNode("//div[@id='appbar']");
            if (crNode != null)
            {
                // if (crNode.SelectSingleNode(".//div[@id='sh_uid_1']") != null)
                if (crNode.SelectSingleNode(".//sticky-header[@class='pA48Db']") != null || crNode.SelectSingleNode(".//div[@id='sh_uid_1']") != null || crNode.SelectSingleNode(".//div[@class='K1fSEd']") != null || crNode.SelectSingleNode(".//div[@class='klbar']") != null) //03-06-2020 //23-05-2020 commented this line getting object reference|| crNode.SelectSingleNode(".//div[@class='KkEU2']") != null)    //21-05-2020 carousel block
                {
                    s.Append("<block type=\"carousel\" url=\"\">");
                    //s.Append(GetCarousel(crNode)); //23-05-2020 commented because if item urls are empty then display empty block
                    s.Append("</block>");
                }

            }

            crNode = doc.DocumentNode.SelectSingleNode("//div[@id='taw']");
            if (crNode != null)
            {
                //// apps
                HtmlNode App = crNode.SelectSingleNode(".//div[@class='kvsF2b']|.//div[@class='i5ai3 gsmt']"); //d5oMvf
                if (App != null)
                {
                    //HtmlNode App1 = App.SelectSingleNode(".//h3[@class='header-title yovt']");
                    s.Append("<block type=\"apps\" url=\"\">");

                    HtmlNodeCollection App2 = App.SelectNodes(".//grid-list-tile-indexed");
                    if (App2 != null)
                    {
                        foreach (HtmlNode nd in App2)
                        {
                            string title = "";
                            string url = "";
                            try
                            {
                                HtmlNode n = nd.SelectSingleNode(".//div[@class='dJiFAd']");
                                title = SetTitle(n.InnerText);
                                url = n.Attributes["href"].Value;
                            }
                            catch { }
                            s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                        }
                    }
                    s.Append("</block>");
                }
                // mnr-c IGtt6d imgac qs-ic fp-w cTMkTb
                // product listed ads
                if (doc.DocumentNode.SelectSingleNode("//div[@class='mnr-c IGtt6d imgac cTMkTb']") != null
                    || doc.DocumentNode.SelectSingleNode("//div[@class='mnr-c IGtt6d imgac qs-ic fp-w cTMkTb']") != null)
                {
                    HtmlNode pla = crNode.SelectSingleNode(".//div[contains(@class, 'commercial-unit-mobile-top')]");
                    if (pla == null)
                        pla = doc.DocumentNode.SelectSingleNode(".//div[contains(@class, 'commercial-unit-mobile-bottom')]");   // 18-09-2018
                    if (pla != null)
                    {
                        HtmlNode h3 = pla.SelectSingleNode(".//div[@class='dxR8gf']/h3");
                        if (h3 == null)
                            h3 = pla.SelectSingleNode(".//div[@class='richlist-top-shopping-title']/h3");
                        if (h3 == null)
                            h3 = pla.SelectSingleNode(".//div[@class='gsrt richlist-top-shopping-title']/h3");
                        if (h3 == null)
                            h3 = pla.SelectSingleNode(".//div[@class='gsrt dxR8gf']");
                        if (h3 == null)
                            h3 = pla.SelectSingleNode(".//div[@class='qgYQZb']/div");   // 29-11-2019
                        if (h3 != null)
                        {
                            if ((pla.SelectSingleNode(".//h3[@class='r']") != null && pla.SelectSingleNode(".//h3[@role='heading']") != null) //13-11-2019
                                || h3.InnerText.StartsWith("Shop for") || h3.InnerText.StartsWith("See ") || WebUtility.HtmlDecode(h3.InnerText).StartsWith("Ads·See ")
                                || h3.InnerText.StartsWith("Ver ")
                                )
                            {
                                s.Append("<block type=\"productListedAds\" url=\"\">");

                                HtmlNodeCollection cl = pla.SelectNodes(".//a[@class='pla-unit eUPzHb']|.//div[@class='mnr-c pla-unit']/a[2]|.//a[@class='plantl pla-unit-single-clickable-target clickable-card']");
                                if (cl == null)
                                    cl = pla.SelectNodes(".//a[@class='pla-unit']");
                                if (cl != null)
                                {
                                    foreach (HtmlNode nd in cl)
                                    {
                                        var url = nd.Attributes["href"].Value;
                                        url = GetRedirectedUrl(url);
                                        if (!string.IsNullOrEmpty(nd.SelectSingleNode(".//h4").InnerText) && !string.IsNullOrEmpty(url.Trim()))    //13-11-2019
                                            s.Append("<item url=\"" + SetUrl(url.Replace("&nbsp;", "")) + "\" title=\"" + SetTitle(nd.SelectSingleNode(".//h4").InnerText) + "\" />"); // 04-11-2019
                                    }
                                }
                                //09-09-2019
                                else if (doc.DocumentNode.SelectNodes(".//div[@class='RL6uuc gws-product_ads-showcase_immersive__immersive-tile']//div[@class='PhX95']") != null)
                                {
                                    HtmlNodeCollection hidedNodes = doc.DocumentNode.SelectNodes(".//div[@class='RL6uuc gws-product_ads-showcase_immersive__immersive-tile']"); //01-11-2019
                                    if (hidedNodes == null)
                                        hidedNodes = doc.DocumentNode.SelectNodes(".//div[@class='OkuxMe']");   // 13-11-2019
                                    HtmlNode urlnode, innertextNode;
                                    if (hidedNodes != null)
                                    {
                                        foreach (HtmlNode planode in hidedNodes)
                                        {
                                            //urlnode = planode.SelectSingleNode(".//div[@class='PhX95']");
                                            //innertextNode = planode.SelectSingleNode(".//div[@class='Ved4gc']");

                                            //01-11-2019
                                            urlnode = planode.SelectSingleNode(".//div[@class='PhX95']|.//div[@class='UBq0ab']");
                                            //if (urlnode == null)
                                            //{
                                            //    s.Append("<item url=\"\" title=\"" + SetTitle(urlnode.InnerText) + "\" />");
                                            //}
                                            innertextNode = planode.SelectSingleNode(".//div[@class='Ved4gc']|.//div[@class='UBq0ab']");
                                            //11-11-2019
                                            string url = GetProductListedUrls(urlnode.InnerText.ToString().Replace("&nbsp;", ""));
                                            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(innertextNode.InnerText))    //13-11-2019
                                                s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(innertextNode.InnerText) + "\" />");  // 04-11-2019
                                        }
                                    }
                                }
                                //else
                                //{
                                //    cl = pla.SelectNodes(".//div[@class='zgXJce']");
                                //    if (cl == null)
                                //        cl = pla.SelectNodes(".//div[@class='JsMaPc']");
                                //    if (cl != null)
                                //    {
                                //        foreach (HtmlNode nd in cl)
                                //        {
                                //            try
                                //            {
                                //                s.Append("<item url=\"\" title=\"" + SetTitle(nd.InnerText) + "\" />");
                                //            }
                                //            catch { }
                                //        }
                                //    }
                                //}
                                s.Append("</block>");
                            }
                        }
                    }
                }

                // text ads
                HtmlNodeCollection col = crNode.SelectNodes(".//div[@id='tads']/ol/li");
                if (col == null)
                    col = crNode.SelectNodes(".//div[@id='tadsb']/ol/li"); // 21-02-2020 included selector for the text ads block
                if (col == null)
                    col = doc.DocumentNode.SelectNodes("//div[@id='tads']/div/ol/li"); //08-04-2020
                //if (col == null)
                //    col = doc.DocumentNode.SelectNodes("//div[@class='WcsE3 dJMePd']/div");  // 01-04-2020 commented on 16-06-2020
                if (col == null)
                    col = doc.DocumentNode.SelectNodes("//div[@jsname='hWE2jd']");//16-06-2020
                // 12-06-2020
                //if (col != null)
                //{
                //    col = doc.DocumentNode.SelectNodes("//div[@jsname='xBqLkd']");
                //    if (col != null) col = null;
                //}//end 12-06-2020

                if (col != null)
                {
                    s.Append("<block type=\"adwords\" url=\"\">");
                    foreach (HtmlNode nd in col)
                    {
                        HtmlNode n = nd.SelectSingleNode(".//div[@class='ad_cclk']/a[2]");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='d5oMvf']/a");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='d5oMvf KJDcUb']/a");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='d5oMvf KJDcUb WzRKRb']/a");  // 29-11-2019
                        if (n == null)
                            n = nd.SelectSingleNode(".//div/a[@class='V0MxL']");    // changes on 28-06-2019
                        if (n == null)
                            n = nd.SelectSingleNode(".//a[@jsname='wOJZib']");  // 01-04-2020
                        if (n != null)
                        {
                            string title = (n.SelectSingleNode(".//h3") != null) ? n.SelectSingleNode(".//h3").InnerText
                                : (n.SelectSingleNode(".//div[@role='heading']") != null) ? n.SelectSingleNode(".//div[@role='heading']").InnerText
                                : (n.SelectSingleNode(".//div[@class='mdzVfb gAWudd']") != null) ? n.SelectSingleNode(".//div[@class='mdzVfb gAWudd']").InnerText  // 01-04-2020
                                : n.InnerText;

                            if (!n.Attributes["href"].Value.StartsWith("/"))
                            {
                                var url = n.Attributes["href"].Value;
                                url = GetRedirectedUrl(url);
                                s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                            }
                            else if (n.Attributes["data-rw"] != null && !n.Attributes["data-rw"].Value.StartsWith("/"))  // changes on 28-06-2019
                            {
                                var url = n.Attributes["data-rw"].Value;
                                url = GetRedirectedUrl(url);
                                s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                            }
                            else
                            {
                                n = nd.SelectSingleNode(".//div[@class='ads-visurl']/cite|.//div[@class='QNz0M ellip GsCRYb']/cite");   // changes on 28-06-2019
                                if (n != null)
                                    s.Append("<item url=\"" + SetUrl(n.InnerText) + "\" title=\"" + SetTitle(title) + "\" />");
                            }
                        }
                    }
                    s.Append("</block>");
                }

                //// answercards
                HtmlNode answernode = crNode.SelectSingleNode(".//div[@class='xpdopen']|.//div[@class='xpdopen rYczAc']");
                if (answernode != null)
                {
                    bool video = false;
                    HtmlNode ndv = answernode.SelectSingleNode(".//div[@class='srg']");
                    if (ndv != null)
                    {
                        var vlnk = ndv.SelectSingleNode(".//a");
                        if (vlnk != null)
                            if (vlnk.Attributes["href"].Value.Contains("youtube."))
                            {
                                s.Append("<block type=\"videoCard\" url=\"" + SetUrl(vlnk.Attributes["href"].Value) + "\"></block>");

                                video = true;
                            }
                    }

                    if (!video)
                    {
                        //HtmlNode App1 = App.SelectSingleNode(".//h3[@class='header-title yovt']");                    

                        HtmlNodeCollection App2 = answernode.SelectNodes(".//div[@class='ytwLQd']/h3/a");
                        if (App2 != null)
                        {
                            s.Append("<block type=\"answerCard\" url=\"\">");
                            foreach (HtmlNode nd in App2)
                            {
                                string title = "";
                                string url = "";
                                try
                                {
                                    title = SetTitle(nd.InnerText);
                                    url = nd.Attributes["href"].Value;
                                }
                                catch { }
                                s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                            }

                            s.Append("</block>");
                        }
                    }
                }

                //Knowledge graph new 
                HtmlNode kgNode = crNode.SelectSingleNode(".//div[@class='c ptJHdc commercial-unit-mobile-top']|.//div[@class='SPZz6b']");

                if (kgNode != null)
                {
                    if (kgNode.SelectSingleNode(".//div[@class='navigation']|.//div[@class='kno-ecr-pt kno-fb-ctx HOpgu gsmt']") != null)     //'kp-blk knowledge-panel _Rqb _RJe']") != null)
                    {
                        HtmlNode kgNode1 = crNode.SelectSingleNode(".//div[@class='l1GGUc kCNel gsrt N49EUd']"); //start 07-08-2019
                        string title = kgNode1.InnerText; //start 07-08-2019

                        s.Append("<block type=\"knowledgeGraph\" url=\"\" title=\"" + SetTitle(title) + "\" />");//start 07-08-2019
                    }
                }
            }

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
            HtmlNodeCollection nds;

            if (node.SelectSingleNode(".//div[@jscontroller='iht5n']") != null)
                nds = node.SelectNodes(".//div[@jscontroller='iht5n']/div");
            else
            {
                //nds = node.SelectNodes(".//div[@class='mnr-c waTp2e xpd O9g5cc uUPGi']");  19-05-2020   //20-01-2020 selector changed for two classic links
                //if (nds == null)
                nds = node.SelectNodes(".//div[@class='mnr-c O9g5cc uUPGi']|.//div[@class='mnr-c xpd O9g5cc uUPGi']|.//div[@class='HD8Pae mnr-c xpd O9g5cc uUPGi']|.//div/g-card[@class='XqIXXe']|.//g-card[@id='tscffb']|.//g-card[@class='g F6CFcc']|.//div[@class='khgTR lWEpfd']|.//div[@class='mnr-c fp-w qs-ic aig-grd']|.//g-card[@class='URhAHe']"); //20-05-2020 missing classic link //05-06-2020
                if (nds == null)
                    if (node.Attributes["class"].Value == "mnr-c xpd O9g5cc uUPGi")
                        nds = node.SelectNodes(".//div[@class='KJDcUb']");
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='KJDcUb WzRKRb']");//29-11-2019
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='setTDc']");   // 25-10-2019
            }
            if (nds != null)
            {
                foreach (HtmlNode nd in nds)
                {
                    try
                    {
                        //14-11-2019
                        if (nd.SelectSingleNode(".//div[@jscontroller='i5z2Rc']") != null
                            || nd.SelectSingleNode(".//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']") != null)  //13-12-2019
                        {
                            s.Append(GetSiteLinks(nd));
                            continue;
                        }
                        //21-02-2020  included selector for the Apps Block
                        if (nd.SelectSingleNode(".//div[@class='ki5rnd']|.//div[@class='yR4jwc']") != null) //20-05-2020 included selector for app block
                        {
                            s.Append(GetApps(nd));
                            continue;
                        }

                        //27-09-2019
                        if (nd.HasClass("F6CFcc"))  // twitter block    
                        {
                            if (nd.SelectSingleNode(".//div[@class='qdrjAc Dwsemf']") != null)  //11-11-2019
                            {
                                s.Append(GetTwitterCards(nd));
                                continue;
                            }
                        }

                        if (nd.SelectSingleNode(".//g-inner-card[@class='zf84ud THG0oc VoEfsd']") != null)
                            continue;
                        //end 27-09-2019

                        try
                        {
                            // 17-10-2019
                            if (nd.Name == "g-card" || nd.Attributes["class"].Value == "HD8Pae mnr-c xpd O9g5cc uUPGi")
                            {
                                if (nd.SelectNodes(".//div[@class='g card-section jiwmWe']") == null)  //27-12-2019
                                {
                                    string vdos = string.Empty;
                                    vdos = SetVideos(nd);
                                    if (!string.IsNullOrEmpty(vdos))
                                    {
                                        s.Append(vdos);
                                        continue;
                                    }
                                }
                            }
                            // end 17-10-2019
                        }
                        catch { }

                        if (nd.Attributes["data-hveid"] != null)    // || nd.SelectSingleNode(".//div[@class='U3THc']") != null)
                        {
                            if (!Regex.IsMatch(nd.Attributes["data-hveid"].Value, "C..QAA") && (!Regex.IsMatch(nd.Attributes["data-hveid"].Value, "C....")))

                                continue;


                            HtmlNode img = nd.SelectSingleNode(".//div[@class='G5NbBd']/div");

                            if (img != null)
                            {
                                try
                                {
                                    if (Regex.IsMatch(img.OuterHtml, "id =\"vidthumb\\d*\"") || img.Attributes["class"].Value.Contains("__video-result"))
                                    {
                                        HtmlNode vdo = nd.SelectSingleNode(".//a[@class='C8nzq BmP5tf']");
                                        // video block.
                                        if (vdo == null)
                                            vdo = nd.SelectSingleNode(".//div[@class='th N3nEGc']/a");
                                        if (vdo != null)
                                        {
                                            string url = vdo.Attributes["href"].Value;
                                            //string title = vdo.SelectSingleNode(".//div[1]").InnerText; // vdo.InnerText.Replace("'", "").Replace("\"", "");
                                            string title = vdo.SelectSingleNode(".//div[@class='MUxGbd v0nnCb']|.//div[@class='zlBHuf MUxGbd v0nnCb']").InnerText;  //24-09-2019
                                            if (url.StartsWith("http") || url.StartsWith("https") || url.StartsWith("ftp")) //30-04-2020
                                            {
                                                int indx = url.LastIndexOf("http://");
                                                if (indx < 0)
                                                {
                                                    indx = url.LastIndexOf("https://");
                                                }
                                                if (indx < 0)
                                                {
                                                    indx = url.LastIndexOf("ftp://");  //30-04-2020
                                                }
                                                url = url.Remove(0, indx);
                                                s.Append("<block type=\"video\" url=\"\">");
                                                s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                                                s.Append("</block>");
                                                continue;
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }


                            HtmlNode n = nd.SelectSingleNode(".//div[@class='ZINbbc xpd']/div/a");
                            if (n == null)
                                n = nd.SelectSingleNode(".//div[@class='ZINbbc xpd']/div[1]/a");
                            if (n == null)
                                n = nd.SelectSingleNode(".//a[@class='C8nzq JTuIPc amp_r']");
                            if (n == null)
                                n = nd.SelectSingleNode(".//a[@class='C8nzq JTuIPc']");
                            if (n == null)
                                n = nd.SelectSingleNode(".//a[@class='C8nzq BmP5tf amp_r']");  // 10-06-2020 swapped from below
                            if (n == null)
                                n = nd.SelectSingleNode(".//a[@class='C8nzq Tj0U2 BmP5tf']");  // 27-11-2019
                            if (n == null)
                                n = nd.SelectSingleNode(".//a[@class='C8nzq BmP5tf']");   // 10-06-2020 swapped from above
                            if (n == null)
                                n = nd.SelectSingleNode(".//a[@class='C8nzq Tj0U2 BmP5tf amp_r']"); // 29-11-2019
                            if (n == null)
                                n = nd.SelectSingleNode(".//a[@class='sXtWJb amp_r']"); // 20-05-2020
                            if (n == null)
                                n = nd.SelectSingleNode(".//g-link/a");
                            if (n == null)
                            {
                                n = nd.SelectSingleNode(".//div[@class='rc']");
                                if (n != null)
                                    n = nd.SelectSingleNode(".//h3[@class='r']/a|.//h3[@class='r']/div/a"); //09-06-2020
                            }
                            if (n != null)
                            {
                                string u = n.Attributes["href"].Value;
                                HtmlNode d = n.SelectSingleNode(".//div[@role='heading']");
                                string t = "";
                                if (d != null)
                                    t = d.InnerText;
                                else
                                    t = n.InnerText;

                                if (orgLinks < 100)
                                {
                                    if (u.StartsWith("http") || u.StartsWith("https") || u.StartsWith("ftp")) //30-04-2020
                                    {
                                        int indx = u.LastIndexOf("http://");
                                        if (indx < 0)
                                        {
                                            indx = u.LastIndexOf("https://");
                                        }
                                        if (indx < 0)
                                        {
                                            indx = u.LastIndexOf("ftp://");  //30-04-2020
                                        }
                                        //string links1 = HttpUtility.UrlDecode(u);

                                        s.Append("<item url=\"" + SetUrl(u) + "\"  title=\"" + SetTitle(t) + "\" />");   // 
                                        orgLinks++;
                                    }
                                }

                            }
                        }
                        else if (nd.Attributes["class"] != null || nd.Attributes.Count == 0)
                        {
                            if (nd.Attributes.Count == 0 || nd.Attributes["class"].Value == "mnr-c" || nd.Attributes["class"].Value == "mnr-c xpd O9g5cc uUPGi"
                             || nd.Attributes["class"].Value == "mnr-c O9g5cc uUPGi" || nd.Attributes["class"].Value == "mnr-c waTp2e xpd O9g5cc uUPGi"
                             || nd.Attributes["class"].Value == "MGqjK" || nd.Attributes["class"].Value == "setTDc" || nd.Attributes["class"].Value == "khgTR lWEpfd"   // 20-05-2020
                             || node.Attributes["class"]?.Value == "mnr-c xpd O9g5cc uUPGi") //20-01-2020 // selectors for two classic links block
                            {
                                //17-10-2019
                                string vdos = string.Empty;
                                vdos = SetVideos(nd);
                                if (!string.IsNullOrEmpty(vdos))
                                {
                                    s.Append(vdos);
                                    continue;
                                }
                                // end 17-10-2019




                                HtmlNode img = nd.SelectSingleNode(".//img");

                                if (img != null)
                                {
                                    try
                                    {
                                        //28-10-2019
                                        if (Regex.IsMatch(img.OuterHtml, "id=\"vidthumb\\d*\"") || nd.SelectSingleNode(".//div[@class='YgXj7b Qc4Zr']") != null)
                                        {
                                            HtmlNode n = nd.SelectSingleNode(".//h3[@class='r']/a");
                                            // video block.
                                            if (n == null)
                                                n = nd.SelectSingleNode(".//div[@class='th N3nEGc']/a");
                                            if (n == null)
                                                n = nd.SelectSingleNode(".//a[@class='C8nzq BmP5tf amp_r']");  // 10-06-2020 swapped from below //08-01-2020 //included selector for video block
                                            if (n == null)
                                                n = nd.SelectSingleNode(".//a[@class='C8nzq BmP5tf']");  // 10-06-2020 swapped from above
                                            if (n == null)
                                                n = nd.SelectSingleNode(".//a[@class='C8nzq Tj0U2 BmP5tf']");    // 29-11-2019
                                            string url = n.Attributes["href"].Value;

                                            string title = n.InnerText.Replace("'", "").Replace("\"", "");
                                            //28-10-2019
                                            if (n.SelectSingleNode(".//div[@role='heading']") != null)
                                                title = n.SelectSingleNode(".//div[@role='heading']").InnerText;
                                            if (url.StartsWith("http") || url.StartsWith("https") || url.StartsWith("ftp")) //30-04-2020
                                            {
                                                int indx = url.LastIndexOf("http://");
                                                if (indx < 0)
                                                {
                                                    indx = url.LastIndexOf("https://");
                                                }
                                                if (indx < 0)
                                                {
                                                    indx = url.LastIndexOf("ftp://");  //30-04-2020
                                                }
                                                url = url.Remove(0, indx);
                                                s.Append("<block type=\"video\" url=\"\">");
                                                s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                                                s.Append("</block>");
                                                continue;
                                            }
                                        }
                                    }
                                    catch { }

                                }

                                HtmlNode nv = nd.SelectSingleNode(".//div[@class='ZINbbc xpd']/div/a");
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//div[@class='ZINbbc xpd']/div[1]/a");
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//a[@class='C8nzq JTuIPc amp_r']");
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//a[@class='C8nzq JTuIPc']");
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//a[@class='C8nzq BmP5tf amp_r']");  // 10-06-2020 swapped from below
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//a[@class='C8nzq Tj0U2 BmP5tf']");  // 27-11-2019
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//a[@class='C8nzq BmP5tf']");  // 10-06-2020 swapped from above
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//a[@class='C8nzq Tj0U2 BmP5tf amp_r']"); // 29-11-2019
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//a[@class='sXtWJb amp_r']"); // 20-05-2020
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//g-link/a");
                                if (nv == null)
                                {
                                    nv = nd.SelectSingleNode(".//div[@class='rc']|.//div[@class='ytwLQd']");//05-06-2020 missing classic links
                                    if (nv != null)
                                        nv = nd.SelectSingleNode(".//h3[@class='r']/a|.//h3[@class='r']/div/a"); // 09-06-2020
                                }
                                if (nv != null)
                                {
                                    string u = nv.Attributes["href"].Value;
                                    HtmlNode d = nv.SelectSingleNode(".//div[@role='heading']");
                                    string t = "";
                                    if (d != null)
                                        t = d.InnerText;
                                    else
                                        t = nv.InnerText;


                                    if (orgLinks < 100)
                                    {
                                        u = SetUrl(u);
                                        if (u.StartsWith("http") || u.StartsWith("https") || u.StartsWith("ftp")) //30-04-2020
                                        {
                                            int indx = u.LastIndexOf("http://");
                                            if (indx < 0)
                                            {
                                                indx = u.LastIndexOf("https://");
                                            }
                                            if (indx < 0)
                                            {
                                                indx = u.LastIndexOf("ftp://");  //30-04-2020
                                            }
                                            //string links1 = HttpUtility.UrlDecode(u);

                                            s.Append("<item url=\"" + SetUrl(u) + "\"  title=\"" + SetTitle(t) + "\" />");   // 
                                            orgLinks++;
                                        }
                                    }
                                }

                            }
                            else if (nd.Attributes["class"].Value == "g mnr-c srg")
                            {
                                // if it is app link
                                HtmlNode n = node.SelectSingleNode(".//div[@class='g mnr-c srg']/a");
                                if (n != null)
                                {
                                    if (orgLinks < 100)
                                    {
                                        var u = n.Attributes["href"].Value;
                                        if (u.StartsWith("http") || u.StartsWith("https") || u.StartsWith("ftp")) //30-04-2020
                                        {
                                            int indx = u.LastIndexOf("http://");
                                            if (indx < 0)
                                            {
                                                indx = u.LastIndexOf("https://");
                                            }
                                            if (indx < 0)
                                            {
                                                indx = u.LastIndexOf("ftp://");  //30-04-2020
                                            }
                                            u = u.Remove(0, indx);
                                            // string links1 = HttpUtility.UrlDecode(u);
                                            s.Append("<item url=\"" + SetUrl(u) + "\"  title=\"" + SetTitle(n.InnerText) + "\" />");    //   
                                            orgLinks++;
                                        }

                                    }
                                }
                            }
                        }
                    }

                    catch { }
                }
            }

            //}

            return s.ToString();
        }

        //17-10-2019
        private string SetVideos(HtmlNode nd)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode videos = nd.SelectSingleNode(".//div[@class='TyzpY']");
            if (videos == null)
                videos = nd.SelectSingleNode(".//div[@class='SRYuRe']");
            if (videos == null)
                videos = nd.SelectSingleNode(".//div[@class='TvV1fe']");
            if (videos == null)
                videos = nd.SelectSingleNode(".//div[@class='zK9jzc B3JUpd']");
            if (videos == null)
                videos = nd.SelectSingleNode(".//jsname[@class='ibnC6b']/a|.//div[@jsname='ibnC6b']/a");

            if (videos != null)
            {
                s.Append("<block type=\"videos\" url=\"\">");
                //get video urls;
                s.Append(GetVideos(nd));
                s.Append("</block>");
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
                case "videos":
                    s.Append("<block type=\"videos\" url=\"\">");
                    //get video urls;
                    s.Append(GetVideos(node));
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
                case "productlistedads"://start 13-08-2019
                    s.Append("<block type=\"ProductListedAds\" url=\"\">");
                    s.Append(ProductListedAds(node));
                    s.Append("</block>");
                    break;//end 13-08-2019
                case "carousel"://16-08-2019
                    s.Append("<block type=\"carousel\" url=\"\">");
                    s.Append(GetCarousel(node));
                    s.Append("</block>");//16-08-2019
                    break;
                case "finance":
                    s.Append("<block type=\"finance\" url=\"\"></block>");
                    break;
                case "event":
                    s.Append("<block type=\"eventResults\" url=\"\"></block>");
                    break;
                case "knowledgepanel":
                    s.Append(GetKnowledgePanel(node));  //"<block type=\"knowledgeGraph\" url=\"\" />");
                    break;
                case "videocard":
                    s.Append(GetVideoCard(node));
                    break;
                case "apps":
                    s.Append(GetApps(node));    // changes on 15-07-2019
                    break;
                default:
                    break;
            }
            return s.ToString();

        }

        // 18-10-2019
        private string GetCarousel(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nd = node.SelectNodes(".//div[@jsmodel='uIhXXc']/div/g-scrolling-carousel/div/div/div/ul[@class='Kjd0sd']/div/div/g-inner-card/a");
            if (nd == null)
                nd = node.SelectNodes(".//div[@jsname='WUSFrc']/g-link/a|.//div[@jsname='WUSFrc']/div/g-link/a"); // 06-01-2020 Included new selector for carousel
            string url = "";
            if (nd != null)
            {
                foreach (HtmlNode nd1 in nd)
                {
                    url = nd1.Attributes["href"].Value;
                    HtmlNode hn = nd1.SelectSingleNode(".//div[@class='mB12kf JRhSae nDgy9d']");
                    if (hn == null)
                        hn = nd1.SelectSingleNode(".//div[@class='hfac6d']");
                    if (hn == null)
                        hn = nd1.SelectSingleNode(".//div[@class='hfac6d oz3cqf vH5Lmd']"); //04-06-2020
                    string title = hn.InnerText;
                    s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                }
            }
            if (nd == null)
            {
                nd = node.SelectNodes(".//a[@class='ttwCMe']");
                if (nd == null)
                    nd = node.SelectNodes(".//a[@class='ttwCMe hide-focus-ring']");
                if (nd == null)
                    nd = node.SelectNodes(".//div[@class='uais2d']/a");  //21-05-2020
                if (nd == null)
                    nd = node.SelectNodes(".//div[@class='Z8r5Gb']/a"); //23-05-2020
                if (nd == null)
                    return string.Empty;
                foreach (HtmlNode nd1 in nd)
                {
                    url = nd1.Attributes["href"].Value;
                    HtmlNode hn = nd1.SelectSingleNode(".//div[@class='oyj2db']"); //S20Xzc
                    if (hn == null)
                        hn = nd1.SelectSingleNode(".//div[@class='wfg6Pb']");    //21-05-2020
                    if (hn == null)
                        hn = nd1.SelectSingleNode(".//div[@class='S20Xzc']");    //23-05-2020
                    if (hn == null)
                        hn = nd1.SelectSingleNode(".//div[@class='JjtOHd Bgg9M']");  //23-05-2020 
                    string title1 = hn.InnerText;
                    s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title1) + "\" />");
                }
            }
            string caitems = GetCarouselURLs();
            s.Append(caitems);
            return s.ToString();
        }

        // 18-10-2019
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
                    if (!url.Contains("google.com"))
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
            }

            return s.ToString();
        }

        private string GetProductListedUrls(string url)
        {
            if (url.Contains("www") && url.Contains("http") != true)
            {
                url = "http://" + url;
            }
            else if (url.Contains("www") != true && url.Contains("http") != true)
            {
                url = "http://www." + url;
            }
            else if (url.Contains("www") != true && url.Contains("http") != true)
            {
                Uri uri = new Uri(url);
                if (uri.HostNameType == UriHostNameType.Dns)
                {
                    string host = uri.Host;
                    if (host.Split('.').Length > 2)
                        url = "http://" + url;
                    else
                        url = "http://www." + url;

                }
                else
                    url = "http://www." + url;
            }

            if (url.Contains(" ") || (url.Contains("www.") && url.Split('.').Length == 1))
                url = "";

            return url;
        }

        //start 13-08-2019
        private string ProductListedAds(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();

            HtmlNodeCollection nds = node.SelectNodes(".//div[@class='Lt4Ktd']/div");
            if (nds != null)
            {
                HtmlNodeCollection nd = node.SelectNodes(".//g-inner-card[@class='zj3nWc Nplhsf wOt4nf VoEfsd']/a");
                foreach (HtmlNode nd1 in nd)
                {
                    string title = nd1.InnerText;
                    string url = "";
                    if (url.Contains("http"))
                    {
                        url = nd1.Attributes["href"].Value;
                    }
                    s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                }
            }

            return s.ToString();
        }//start 13-08-2019

        private string GetVideoCard(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();

            HtmlNode nd = node.SelectSingleNode(".//div[@class='_ELb']/a");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='twQ0Be']/a");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='NFQFxe XbtRGb qxsd mod']/div/a"); //23-01-2020  // 21-02-2020 included selector for video cards block
            if (nd != null)
                s.Append("<block type=\"videoCard\" url=\"" + SetUrl(nd.Attributes["href"].Value) + "\"></block>");

            return s.ToString();
        }

        private string GetSiteLinks(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode n = node.SelectSingleNode(".//h3[@class='r']/a");
            if (n == null)
                n = node.SelectSingleNode(".//a[@class='C8nzq JTuIPc']");
            if (n == null)
                n = node.SelectSingleNode(".//a[@class='C8nzq BmP5tf']");
            if (n != null)
            {
                if (orgLinks < 100)
                {
                    HtmlNode t = n.SelectSingleNode(".//div[@role='heading']");
                    s.Append("<item url=\"" + SetUrl(n.Attributes["href"].Value) + "\" title=\"" + SetTitle(t.InnerText) + "\" />");
                    orgLinks++;
                }
            }

            HtmlNodeCollection nds = node.SelectNodes(".//table[@class='nrg']/tr");

            if (nds != null)
            {
                s.Append("<block type=\"siteLinks\" url=\"\">");
                foreach (HtmlNode nd in nds)
                {
                    HtmlNodeCollection c = nd.SelectNodes(".//span[@class='cNifBc']/h3/a");
                    if (c == null) continue;
                    foreach (HtmlNode a in c)
                    {
                        s.Append("<item url=\"" + SetUrl(a.Attributes["href"].Value) + "\" title=\"" + SetTitle(a.InnerText) + "\" />");
                    }
                }
                s.Append("</block>");
            }

            //28-10-2019
            nds = node.SelectNodes(".//div[@class='Lgnr0e J88qA BmP5tf']");
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='pIpgAc KKgUze XO51F OAX6kd']/a");
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='pIpgAc KKgUze XO51F']/a");
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='MUxGbd v0nnCb lyLwlc']/a");
            if (nds != null)
            {
                s.Append("<block type=\"siteLinks\" url=\"\">");
                foreach (HtmlNode nd in nds)
                {
                    //HtmlNodeCollection c = nd.SelectNodes(".//span[@class='cNifBc']/h3/a");
                    //if (c == null) continue;
                    //foreach (HtmlNode a in c)
                    //{
                    if (!nd.InnerHtml.Contains("span id"))//25-04-2020   
                        s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"" + SetTitle(nd.InnerText) + "\" />");
                    //    }
                }
                s.Append("</block>");
            }

            if (orgLinks < 100)//13-12-2019
            {
                nds = node.SelectNodes(".//div[@class='srg']");
                if (nds != null)
                    foreach (HtmlNode nd in nds)
                    {
                        n = nd.SelectSingleNode(".//div[@class='KJDcUb']/a");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='d5oMvf KJDcUb']/a");

                        if (n != null)
                        {
                            HtmlNode t = nd.SelectSingleNode(".//div[@role='heading']");
                            s.Append("<item url=\"" + SetUrl(n.Attributes["href"].Value) + "\" title=\"" + SetTitle(t.InnerText) + "\" />");
                            orgLinks++;
                        }
                    }

            }//13-12-2019

            return s.ToString();
        }

        private string GetKnowledgePanel(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode nd = node.SelectSingleNode(".//div[@class='kp-header']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kp-blk knowledge-panel OJXvsb']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kp-blk fm06If knowledge-panel OJXvsb']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='oLO3I']");
            if (nd == null)//16-09-2019
                nd = node.SelectSingleNode(".//div[@class='kp-wholepage EyBRub ss6qqb mnr-c kp-wholepage-osrp']");//16-09-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='NFQFxe viOShc LKPcQc mod']");  //20-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='NFQFxe XbtRGb qxsd a84NUc CQKTwc mod']");  //20-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='c94Vsf Y1mqLe kp-rgc']");  // 20-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Uyhxfe ZdjxGf']");  // 30-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Y37F6d Nn2Stf']");  // 21-04-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='HnYYW FIdh1']");  // 01-06-2020
            if (nd != null)
            {
                string hdr = "";
                //string hdr = nd.SelectSingleNode(".//div[@role='heading']/div[1]/span").InnerText;
                HtmlNode hdrNode = nd.SelectSingleNode(".//div[@role='heading']/div[1]/span");
                if (hdrNode == null)
                    hdrNode = nd.SelectSingleNode(".//div[@class='kno-ecr-pt kno-fb-ctx HOpgu gsmt']/span");
                if (hdrNode == null)
                    hdrNode = nd.SelectSingleNode(".//div[@class='gsrt TCmnBf']");
                if (hdrNode == null)//26-09-2019
                    hdrNode = nd.SelectSingleNode(".//div[@class='SPZz6b']/div");//26-09-2019
                if (hdrNode == null)
                    hdrNode = nd.SelectSingleNode(".//div[@class='cX4Std B7U7kd']"); //30-03-2020
                if (hdrNode == null)
                    hdrNode = node.SelectSingleNode(".//div[@class='HnYYW FIdh1']"); //21-04-2020  
                if (hdrNode != null)
                    hdr = hdrNode.InnerText;

                s.Append("<block type=\"knowledgeGraph\" url=\"\" title=\"" + SetTitle(hdr) + "\" />");
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
            HtmlNodeCollection nds = node.SelectNodes(".//h3[@class='r']/a");
            if (nds == null)
                nds = node.SelectNodes(".//a[@class='sXtWJb']");      //26-11-2019
            if (nds == null)
                return string.Empty;
            foreach (HtmlNode nd in nds)
            {
                s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"" + SetTitle(nd.InnerText) + "\" />");
            }
            return s.ToString();
        }

        private string GetTwitterCards(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode hn = node.SelectSingleNode(".//div[@class='JVrfPc']/a");
            if (hn == null)
                hn = node.SelectSingleNode(".//g-link//a"); //included on 2019-06-24
            if (hn != null)
            {
                s.Append("<block type=\"twitterCards\" url=\"" + SetUrl(hn.Attributes["href"].Value) + "\">");

                HtmlNodeCollection nds = node.SelectNodes(".//div[@class='uR34qf oIY2kd JTuIPc']/a");

                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='uR34qf dJMePd JTuIPc']/a");
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='uR34qf dJMePd BmP5tf']/a");
                if (nds == null)
                    nds = node.SelectNodes(".//g-scrolling-carousel//g-card-section[@class='a02Mp jDsVJf']/a"); //included on 2019-06-24

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
            bool existed = false;
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[@class='GNxIwf']/div[@jscontroller='xc1DSd']/div/a/g-inner-card/g-img[@class='BA0A6c']/img");  //30-10-2019
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='GNxIwf']/div[@jscontroller='xc1DSd']/a/g-inner-card/g-img[@class='BA0A6c']/img|.//div[@class='eA0Zlc JX86yc ivg-i']/g-inner-card/g-img[@class='BA0A6c']/img"); //05-06-2020 included selector for images //13-01-2020 included selector for images
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='eR2XS']/g-inner-card/div/a/g-img[@class='SeXxHf']/img"); //08-06-2020
            if (nds != null)

                foreach (HtmlNode nd in nds)
                {
                    if (nd.Attributes.Contains("data-src"))
                    {
                        string url = nd.Attributes["data-src"].Value.Trim();
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
            //string matchPattern4 = "\\W\\W\\Wx22http[s]*://(.*?)\\Wx22";   // 17-02-2020 included pattern
            string matchPattern4 = @"\[0,\\x22[\w-\d]*:\\x22,\[\\x22(.*?)\\x22,"; //18-02-2020 replaced pattern for above 17-02-2020
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

            // 17-02-2020 
            re = new Regex(matchPattern4, RegexOptions.IgnoreCase);
            mc = re.Matches(html);

            foreach (Match m in mc)
            {
                string HtmlText = HttpUtility.UrlDecode(m.Groups[1].Value);

                HtmlText = Regex.Unescape(HtmlText.Replace("\\\\", "\\"));  //18-02-2020

                //if (!HtmlText.Contains("encrypted") && !HtmlText.Contains(".google.") && !HtmlText.Contains("cdn-img.") && !HtmlText.Contains("image.") && !HtmlText.Contains("t0.gstatic.") && !HtmlText.Contains("img.") && !HtmlText.Contains("cdn.tobi.") && !HtmlText.Contains("ae01."))
                alDup.Add(HtmlText);
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
            HtmlNodeCollection nds = node.SelectNodes(".//g-card-section/a");
            //if (nds != null)
            //    foreach (HtmlNode nd in nds)
            //    {
            //        s.Append("<item url=\"" + nd.Attributes["href"].Value + "\" title=\"" + nd.SelectSingleNode(".//div/span").InnerText + "\" />");
            //    }
            //nds = node.SelectNodes(".//g-inner-card/a");
            //if (nds != null)
            //    foreach (HtmlNode nd in nds)
            //    {
            //        s.Append("<item url=\"" + nd.Attributes["href"].Value + "\" title=\"" + nd.InnerText + "\" />");
            //    }
            if (nds != null)
            {
                foreach (HtmlNode nd in nds)
                {
                    //changes on 02-07-2019
                    HtmlNode title = nd.SelectSingleNode(".//div[@role='heading']");
                    if (title == null)
                        title = nd.SelectSingleNode(".//div[@class='d4FON']");
                    s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"" + SetTitle(title.InnerText) + "\" />");
                }
            }

            nds = node.SelectNodes(".//g-inner-card/a");
            if (nds == null)
                nds = node.SelectNodes(".//g-inner-card/div/a");    // 30-10-2019
            if (nds == null)
                nds = node.SelectNodes(".//g-inner-card/div/div/a");    // 13-12-2019
            if (nds == null)
                nds = node.SelectNodes(".//lazy-load-item/div/a");
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='amp_re dbsr']/a|.//div[@class='dbsr']/a|.//div[@class='Fq8eSd']/a|.//div[@class='y1Boce']/div/a");   // 18-12-2019
            if (nds != null)
                foreach (HtmlNode nd in nds)
                {
                    //changes on 28-06-2019
                    HtmlNode title = nd.SelectSingleNode(".//div[@class='d4FON']"); // 03-06-2020 swapped from below line.
                    if (title == null)
                        title = nd.SelectSingleNode(".//div[@role='heading']");
                    if (title == null)
                        title = nd.SelectSingleNode(".//div[@class='nDgy9d']");   //changes on 05-07-2019

                    string url = nd.Attributes["href"].Value;
                    s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title.InnerText) + "\" />");
                }
            else
            {
                nds = node.SelectNodes(".//a");
                if (nds != null)
                {
                    foreach (HtmlNode nd in nds)
                    {
                        if (nd.SelectSingleNode(".//div[@class='poMUXd']") != null || nd.SelectSingleNode(".//div[@class='mCBkyc nDgy9d']") != null || nd.SelectSingleNode(".//div[@class='poMUXd oz3cqf vH5Lmd']") != null || nd.SelectSingleNode(".//div[@class='mCBkyc oz3cqf vH5Lmd nDgy9d']") != null) //10-06-2020 //04-06-2020  //14-05-2020
                        {
                            HtmlNode title = nd.SelectSingleNode(".//div[@class='poMUXd']");
                            if (title == null)
                                title = nd.SelectSingleNode(".//div[@class='mCBkyc nDgy9d']");    //14-05-2020
                            if (title == null)
                                title = nd.SelectSingleNode(".//div[@class='poMUXd oz3cqf vH5Lmd']");//04-06-2020
                            if (title == null)
                                title = nd.SelectSingleNode(".//div[@class='mCBkyc oz3cqf vH5Lmd nDgy9d']");//10-06-2020
                            string url = nd.Attributes["href"].Value;
                            s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title.InnerText) + "\" />");
                        }
                    }
                }
            }

            return s.ToString();
        }

        private string GetVideos(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//g-inner-card/a");
            if (nds == null)
                nds = node.SelectNodes(".//div[@data-attrid='OsrpVideos']/a|.//div[@class='ALzVK']/div/div/a");
            if (nds == null)
                nds = node.SelectNodes(".//div[@jsname='ibnC6b']/a");
            if (nds != null)   // 16-09-2019
                foreach (HtmlNode nd in nds)
                {
                    try
                    {
                        string title = "";
                        HtmlNode n = nd.SelectSingleNode(".//div[@class='Igo7ld mRnBbe QgUve xIqs0b']");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='KiGY3d mB12kf JRhSae ZyAH8d']");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='oyj2db']");
                        //if (n == null)
                        //    n = nd.SelectSingleNode(".//div[@class='VibNM WGnkfe']|.//div[@jsname='ibnC6b']");
                        try
                        {
                            // Changes in Videos block on 25-06-2019
                            HtmlNode t = nd.SelectSingleNode(".//div[@role='heading']");
                            if (t != null)
                                title = t.InnerText;
                            else
                                title = n.InnerText;
                        }
                        catch
                        {
                            // changes in videos block on 19-06-2019.
                            HtmlNode t = nd.SelectSingleNode(".//div[@class='fJiQld']");
                            if (t == null)
                                t = nd.SelectSingleNode(".//div[@class='fJiQld oz3cqf vH5Lmd']");//10-06-2020
                            if (t != null)
                                title = t.InnerText;
                            // end of changes in videos.
                        }

                        string url = nd.Attributes["href"].Value.Trim();
                        if (url.Contains("/search?")) url = "";
                        //if(!string.IsNullOrEmpty(url.Trim()) || !string.IsNullOrEmpty(title.Trim()))
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                    }
                    catch { }
                }
            else
            {
                // 16-12-2019
                nds = node.SelectNodes(".//div[@jscontroller='OmmTPc']");
                if (nds != null)
                {
                    foreach (HtmlNode nd in nds)
                    {
                        string url = nd.Attributes["data-url"].Value;
                        string title = nd.SelectSingleNode(".//div[@class='fJiQld']|.//div[@class='fJiQld oz3cqf vH5Lmd']").InnerText;//12-06-2020
                        if (url.Contains("/search?")) url = "";
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                    }
                }
                // 16-12-2019
            }

            return s.ToString();
        }

        // changes on 15-07-2019
        private string GetApps(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode App = node.SelectSingleNode(".//div[@class='qs-io aig-lst']"); //d5oMvf
            if (App != null)
            {
                //HtmlNode App1 = App.SelectSingleNode(".//h3[@class='header-title yovt']");
                s.Append("<block type=\"apps\" url=\"\">");

                //06-08-2019
                HtmlNodeCollection App2 = App.SelectNodes(".//div[@class='aig-i aew2ab']");
                if (App2 != null)
                {
                    foreach (HtmlNode nd in App2)
                    {
                        HtmlNode App3 = nd.ChildNodes[0];

                        s.Append("<item url=\"" + SetUrl(App3.Attributes["href"].Value) + "\" title=\"" + SetTitle(nd.InnerText.TrimStart()) + "\" />");
                        //end of 06-08-2019
                    }
                }
                s.Append("</block>");
            }
            //23-01-2020    //21-02-2020 included else condition selector for the App blocks
            else
            {
                App = node.SelectSingleNode(".//a");
                if (App != null)
                {
                    s.Append("<block type=\"apps\" url=\"\">");
                    string innertext = string.Empty;
                    HtmlNode TextNode = App.SelectSingleNode(".//div[@class='mdKzW']");
                    if (TextNode != null)
                        innertext = TextNode.InnerText;

                    s.Append("<item url=\"" + SetUrl(App.Attributes["href"].Value) + "\" title=\"" + SetTitle(innertext.TrimStart()) + "\" />");

                    s.Append("</block>");
                }
            } // end of 23-01-2020  // 21-02-2020
            return s.ToString();
        }

        private string GetBlockType(HtmlNode node)
        {
            HtmlNode nd = node.SelectSingleNode(".//div[@class='KNcnob']/g-img");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='fn6bCb']|.//g-tray-header[@class='kno-fb-ctx zbA8Me ndEm3b']");
            if (nd == null)
                nd = node.SelectSingleNode(".//g-tray-header[@class='kno-fb-ctx gsrt nQE5cd ieGFJe ndEm3b']");  // 06-12-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//g-tray-header[@class='kno-fb-ctx gsrt ieGFJe ndEm3b']");  // 16-12-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//g-tray-header[@class='kno-fb-ctx erfML zbA8Me ndEm3b']");  // 18-12-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//g-tray-header[@class='kno-fb-ctx KULUEe zbA8Me ndEm3b']");  // 23-01-2020 included selector for top stories block
            if (nd == null)
                nd = node.SelectSingleNode(".//g-tray-header[@class='kno-fb-ctx lQckZe zbA8Me ndEm3b']");  // 07-02-2020 included selector for videos block
            if (nd == null)
                nd = node.SelectSingleNode(".//g-tray-header[@class='kno-fb-ctx lQckZe gsrt ieGFJe ndEm3b']");  // 21-02-2020 included selector for videos block
            if (nd == null)
                nd = node.SelectSingleNode(".//g-tray-header[@class='kno-fb-ctx Gq01wc zbA8Me ndEm3b']");//19-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='qDSRad']");  // changes on 05-07-2019
            if (nd != null)
            {
                bool ts = true;
                HtmlNode n = node.SelectSingleNode(".//div[@class='FRH7Ye gsrt']");
                if (n == null)
                    n = node.SelectSingleNode(".//div[@class='TSyGMd']/a");  // changes on 05-07-2019
                if (n != null)
                {
                    if (n.InnerText.ToLower().Contains("news"))
                        ts = false;
                }
                // 23-10-2019
                if (ts)
                {
                    n = node.SelectSingleNode(".//div[@role='heading']");
                    if (n != null)
                    {
                        if (n.InnerText.ToLower().Trim() == "videos" || n.InnerText.ToLower().Trim() == "video" || n.InnerText.Trim() == "فيديوهات")  // 16-12-2019
                            return "Videos";
                        if (n.InnerText.ToLower().Trim() == "recipes" || n.InnerText.ToLower().Trim() == "ricette")  // 20-03-2020 // 18-12-2019
                            return "Carousel";

                    }
                }
                if (ts)
                    return "Topstories";
            }

            if (node.SelectSingleNode(".//div[@id='imso-root']") != null || node.SelectSingleNode(".//div[@class='tsp-view r-iDNua10DBk4I']") != null
                || node.SelectSingleNode(".//div[@class='nA3Vyd SBFvB']") != null || node.SelectSingleNode(".//div[@class='nJXhWc nA3Vyd']") != null || node.SelectSingleNode(".//div[@class='AE4e7c']") != null)  //22-05-2020 included selector for event block
                return "Event";

            //swapped 19-03-2020            
            nd = node.SelectSingleNode(".//div[@class='kp-wholepage EyBRub ss6qqb mnr-c kp-wholepage-osrp']");//16-09-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='NFQFxe viOShc LKPcQc mod']");  //20-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='NFQFxe XbtRGb qxsd a84NUc CQKTwc mod']");  //20-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='c94Vsf Y1mqLe kp-rgc']");  // 20-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Uyhxfe ZdjxGf']");  // 30-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Y37F6d Nn2Stf']");  // 21-04-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='HnYYW FIdh1']");  // 01-06-2020
            if (nd != null)
            {
                // 24-04-2020
                nd = node.SelectSingleNode(".//h2");
                if (nd != null)
                    if (nd.InnerText.ToLower().Trim() == "recipes" || nd.InnerText.ToLower().Trim() == "ricette" || nd.InnerText.ToLower().Trim() == "recetas" || nd.InnerText.ToLower().Trim() == "recept")//17-06-2020 //22-05-2020 receipes language included
                        return "Carousel";
                // 24-04-2020

                nd = node.SelectSingleNode(".//div[@class='kp-blk nGydZ Wnoohf OJXvsb']|.//div[@class='NFQFxe XbtRGb qxsd xsZWvb EfDVh WDjuKe mod']|.//div[@class='NFQFxe viOShc LKPcQc mod']");  // 16-06-2020
                if (nd == null)
                    return "KnowledgePanel";
            }
            //swapped 19-03-2020


            // 30-10-2019
            nd = node.SelectSingleNode(".//div[@class='_ELb']/a");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='twQ0Be']/a");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kno-fb-ctx']");
            if (nd == null)
                nd = node.SelectSingleNode(".//span[@class='DOvy9b z1asCe KXvzXb']");  //23-01-2020  // 21-02-2020  included selector for video card
            if (nd != null)
            {
                return "VideoCard";
            }

            nd = node.SelectSingleNode(".//div[@class='oLO3I']/div");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='qDOt0b']");    //26-11-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div/a[@class='B1uW2d ellip PZPZlf']");    //21-02-2020 included selector for AnswerCard block
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='F7SFG']");  //21-04-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Nhsae']");//01-05-2020
            if (nd != null)
            {
                return "AnswerCard";
            }


            // changes on 15-07-2019
            nd = node.SelectSingleNode(".//div[@class='qs-io aig-lst']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='ki5rnd']"); //23-01-2020   //21-02-2020 included selector for App block
            if (nd != null)
            {
                return "Apps";
            }
            //16-08-2019
            nd = node.SelectSingleNode(".//div[@jsmodel='uIhXXc']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@jsname='GDPwke']"); // 18-10-2019
            if (nd != null)
                if (node.SelectSingleNode(".//img[contains(@alt,'Map of ')]") == null) //15-05-2020
                    return "Carousel";

            nd = node.SelectSingleNode(".//div[@class='TyzpY']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='SRYuRe']");
            if (nd != null)
                return "Videos";

            nd = node.SelectSingleNode(".//div[@class='TvV1fe']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='zK9jzc B3JUpd']");
            if (nd != null && node.SelectSingleNode(".//div[contains(@class, ' knowledge-panel ')]") == null)
            {
                // Changes in Videos block on 25-06-2019
                if (node.InnerText.ToLower().Contains("videos") || node.InnerText.StartsWith("فيديوهات"))    // 29-11-2019
                {
                    return "Videos";
                }
            }
            //changed on 21-08-2019
            else
            {
                nd = node.SelectSingleNode(".//div[@role='heading']/div[@class='HnYYW']");
                if (nd == null)
                    nd = node.SelectSingleNode(".//div[@role='heading']/div[@class='HnYYW i8lZMc']");  // 21-02-2020 
                if (nd != null)
                {
                    if (nd.InnerHtml.ToLower().StartsWith("video") || nd.InnerHtml.ToLower().StartsWith("vídeo")) //07-02-2020 included title for video block for different language
                        return "Videos";
                    else if (nd.InnerHtml.ToLower().StartsWith("top stories") || nd.InnerHtml.Contains("Interesting finds")) // 18-12-2019)
                        return "Topstories";
                    HtmlNode nd1 = node.SelectSingleNode(".//div[@class='UDZeY fAgajc']|.//div[@class='rKFBM gsrt CAd2fd wp-ms']");  //31-03-2020
                    if (nd1 != null)
                    {
                        return "AnswerCard";
                    }
                }
            }
            //changed on 16-09-2019
            nd = node.SelectSingleNode(".//div[@class='LMMXP']");
            if (nd != null)
            {
                if (nd.InnerHtml.ToLower().StartsWith("video") || nd.InnerHtml.StartsWith("Vidéos"))    // 01-11-2019
                    return "Videos";
            }
            nd = node.SelectSingleNode(".//div[@class='zbA8Me f4wI BmP5tf']/div|.//div[@jsname='wRSfy']");//04-11-2019
            if (nd != null)
            {
                return "Videos";
            }

            nd = node.SelectSingleNode(".//*[@id='rXuTZe']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='I2lQic']");//05-11-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='utyL0c']");//01-05-2020
            if (nd != null)
            {
                return "Maps";
            }
            else
            {
                nd = node.SelectSingleNode(".//div[@class='mnr-c']/g-link/a");
                if (nd != null)
                    if (nd.Attributes["href"].Value.Contains("/maps/"))
                        return "Maps";

                // changes on 08-07-2019
                nd = node.SelectSingleNode(".//img[@alt='map image']|.//img[@alt='Affected area map']|.//img[contains(@alt,'Map of ')]");  // 13-03-2020 //01-05-2020
                if (nd != null)
                    return "Maps";
            }

            nd = node.SelectSingleNode(".//div[@class='JVrfPc']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='bUNBRd mnr-c']|.//div[@class='HnYYW i8lZMc']"); //13-03-2020 //include on 2019-06-24
            if (nd != null)
            {
                if (nd.InnerText.Contains("Twitter"))
                    return "Twitters";
            }
            nd = node.SelectSingleNode(".//div[@class='_OKe']");
            if (nd != null)
            {
                nd = node.SelectSingleNode(".//div[@class='_Q1n']");
                if (nd != null)
                    return "PeopleAlsoAsk";

                nd = node.SelectSingleNode(".//div[@id='imso-root']");
                if (nd != null)
                    return "Event";

                return "AnswerCard";
            }
            if (node.SelectSingleNode(".//span[@data-original-name='People also ask']") != null
                || node.SelectSingleNode(".//h2[@class='MA9Une zbA8Me']") != null
                || node.SelectSingleNode(".//div[@data-hveid='CD0Q-wE']") != null
                || node.SelectSingleNode(".//h2[@class='XS4Rbf zbA8Me']") != null
                || (node.SelectSingleNode(".//div[@class='kp-blk cUnQKe OJXvsb']") != null
                && node.SelectSingleNode(".//div[@class='answered-question']") == null)
                || (node.SelectSingleNode(".//div[@class='kp-blk cUnQKe Wnoohf OJXvsb']") != null))
            //&& node.SelectSingleNode(".//div[@class='HnYYW i8lZMc']").InnerText != "People also search for")  //21-05-2020 )
            //&& node.InnerText.Contains("People also ask"))) // 17-09-2019 //07-02-2020 commented to remove title
            {
                if (!node.InnerText.StartsWith("People also search for")) //25-05-2020
                    return "PeopleAlsoAsk";
            }
            // changed on 05-07-2019
            else if (node.SelectSingleNode(".//div[@class='HnYYW']") != null)
            {
                HtmlNode n = node.SelectSingleNode(".//div[@class='HnYYW']");
                //if (n.InnerText == "People also ask" || n.InnerText == "Nutzer fragen auch")//12-08-2019
                if (n.InnerText == "People also ask" || n.InnerText == "Nutzer fragen auch" || n.InnerText == "Le persone hanno chiesto anche" || n.InnerText == "Orang juga bertanya")  //26-11-2019
                    return "PeopleAlsoAsk";
            }

            if (node.SelectSingleNode(".//div[@id='cwmcwd']") != null || node.SelectSingleNode(".//div[@class='vk_bk vk_ans']") != null
                || node.SelectSingleNode(".//div[@class='vk_ans vk_bk']") != null || node.SelectSingleNode(".//div[@data-tts='answers']") != null
                || node.SelectSingleNode(".//div[@class='vk_gy vk_sh whenis']") != null || node.SelectSingleNode(".//div[@class='N6Sb2c i29hTd']") != null
                || node.SelectSingleNode(".//div[@class='kp-blk c2xzTb OJXvsb']") != null
                || (node.SelectSingleNode(".//div[@class='kp-blk cUnQKe OJXvsb']") != null
                && node.SelectSingleNode(".//div[@class='answered-question']") != null)
                || node.SelectSingleNode(".//div[@class='vkc_np kkww4d']") != null    // changed on 05-07-2019
                || node.SelectSingleNode(".//div[@class='UDZeY fAgajc']") != null)     // 13-03-2020

                return "AnswerCard";

            nd = node.SelectSingleNode(".//div[@id='kx']");
            if (nd != null)
            {
                return "Carousel";
            }

            // changes on 05-07-2019
            nd = node.SelectSingleNode(".//div[@class='rKFBM gsrt wp-ms']/div");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='rKFBM gsrt iows2d wp-ms']/div");  //19-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='JNkvid gsrt wp-ms']/div");  //15-05-2020
            if (nd != null)
            {
                if (node.SelectSingleNode(".//g-scrolling-carousel") != null)
                    // 04-11-2019
                    if (nd.InnerText.StartsWith("Movies") || nd.InnerText.StartsWith("Mga Pelikula")
                        || nd.InnerText.StartsWith("Film") || nd.InnerText.StartsWith("Filme")
                         || nd.InnerText.StartsWith("Books")) // 09-06-2020
                    {
                        return "Carousel";
                    }
            }

            nd = node.SelectSingleNode(".//div[@class='bUNBRd mnr-c']/div/g-tray-header/div");   // || node.SelectSingleNode(".//div[@id='imagebox_bigimages']") != null)
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='bUNBRd mnr-c']/g-tray-header/div[@class='N60dNb']/a");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='N60dNb']/a|.//g-tray-header/div[@class='N60dNb i8lZMc']/div[@class='rqLLId i8lZMc']"); //05-06-2020 //08-06-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='bUNBRd mnr-c']/g-tray-header/div[@class='N60dNb i8lZMc']/a");    //17-02-2020 included selector for images
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='gID6df']|.//div[@id='iur']/a"); //12-06-2020//05-06-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='GNxIwf']");  // 18-03-2020
            if (nd != null)
            {
                //if (nd.InnerText == "Images" || nd.InnerText == "Immagini"|| nd.InnerText == "Im?genes")
                return "Images";
            }
            else
            {
                nd = node.SelectSingleNode(".//div[@class='rKFBM gsrt wp-ms']");    // changes on 11-07-2019
                if (nd != null)
                {
                    if (nd.InnerText == "About" || nd.InnerText == "Images" || nd.InnerText == "Imágenes")  // 07-02-2020 and 10-02-2020 21-02-2020 included title for images block
                        return "Images";
                    if (nd.InnerText.Trim() == "Eventos")  // 07-02-2020 included title for Event block
                        return "Event";
                }
            }

            nd = node.SelectSingleNode(".//div[@id='fac-ut']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@id='fac-tc']");     // Changes on 25-06-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@id='knowledge-finance-wholepage__entity-summary']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='dDoNo vk_bk gsrt']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='aviV4d']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='rbR0cd y yi']"); //21-05-2020 finance block included selector
            if (nd != null)
            {
                return "Finance";
            }
            if (node.SelectSingleNode(".//table[@class='nrg']") != null || node.SelectNodes(".//div[@class='pIpgAc KKgUze XO51F OAX6kd']") != null
                || node.SelectNodes(".//div[@class='pIpgAc KKgUze XO51F']") != null || node.SelectNodes(".//a[@class='C8nzq JTuIPc']") != null || node.SelectNodes(".//a[@class='C8nzq BmP5tf']") != null)
            {
                return "SiteLinks";
            }

            //13-08-2019
            nd = node.SelectSingleNode(".//div[@class='aJegcc']");
            if (nd != null)
                if (node.SelectNodes(".//div[@class='xCCdqb']") == null)//16-09-2019
                {
                    return "ProductListedAds";
                }
            return "";
        }

        private bool IsBlock(HtmlNode node)
        {
            // changes on 05-07-2019
            HtmlNode nd = node.SelectSingleNode(".//div[@class='HnYYW']|.//g-tray-header[@role='heading']|.//div[@role='heading']");
            if (nd != null)
            {
                if (nd.InnerText == "Top stories" || nd.InnerText == "Noticias principales" || nd.InnerText == "Interesting finds")
                    return true;
                else if (nd.InnerText == "More results" || nd.InnerText == "Top results" || nd.InnerText == "Toppresultater"
                    || nd.InnerText == "Fler resultat" || nd.InnerText == "Flere resultater" || nd.InnerText == "Plus de résultats")    // 13-12-2019
                    return false;
            }
            //17-01-2020
            nd = node.SelectSingleNode(".//div[@class='Lgnr0e J88qA BmP5tf']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']");
            if (nd != null)
            {
                //30-03-2020
                if (node.SelectSingleNode(".//div[@class='g card-section jiwmWe']") != null)
                    return false;
                if (node.SelectSingleNode(".//div[@class='MUxGbd v0nnCb lyLwlc']") != null)//25-04-2020
                    return false;
                return true;
            }

            //22-11-2019
            nd = node.SelectSingleNode(".//div[@class='g kno-result rQUFld mnr-c g-blk']");
            if (nd != null)
            {
                return true;
            }

            //29-11-2019
            nd = node.SelectSingleNode(".//div[@class='KJDcUb WzRKRb']");
            if (nd != null)
            {
                return false;
            }


            //start 13-08-2019
            nd = node.SelectSingleNode(".//div[@id='sports-app']");
            if (nd != null)
            {
                return true;
            }
            nd = node.SelectSingleNode(".//nav[@class='baPFxb g kSMK2']");
            if (nd != null)
            {
                return true;
            }//end 13-08-2019

            nd = node.SelectSingleNode(".//div[@data-tts='answers']|.//div[@class='N6Sb2c i29hTd']|.//div[@class='kp-blk c2xzTb OJXvsb']|.//div[@class='answered-question']");
            if (nd != null)
            {
                return true;
            }
            nd = node.SelectSingleNode(".//div[@class='oLO3I']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='aZVgnb']/h2");  // 08-06-2020  
            if (nd != null)
                return true;
            // Changes in Finance block on 25-06-2019
            nd = node.SelectSingleNode(".//div[@id='fac-tc']");
            if (nd != null)
                return true;

            nd = node.SelectSingleNode(".//div[@class='mnr-c']/g-link/a");
            if (nd != null)
                if (nd.Attributes["href"].Value.Contains("/maps/"))
                    return true;

            nd = node.SelectSingleNode(".//div[@class='srg']");
            if (nd != null)
                return false;
            nd = node.SelectSingleNode(".//g-card[@id='tscffb']");
            if (nd != null)
            {
                nd = node.SelectSingleNode(".//g-card[@class='XqIXXe']");
                if (nd != null)
                {
                    nd = node.SelectSingleNode(".//div[@class='zK9jzc B3JUpd i8lZMc']"); //17-01-2020 selector changed videos block
                    if (nd == null)
                        nd = node.SelectSingleNode(".//g-tray-header[@class='kno-fb-ctx lQckZe gsrt ieGFJe ndEm3b']");  //21-02-2020 included selector for videos card
                    if (nd != null)
                    {
                        //if (nd.InnerText.Trim() == "Recipes" || nd.InnerText.Trim() == "Recept" || nd.InnerText.Trim() == "Ricette")// 21-11-2019
                        return true;
                    }//04-11-2019"
                    return false;
                }
                return true;
            }
            nd = node.SelectSingleNode(".//div[@jscontroller='iht5n']");
            //nd = node.SelectSingleNode(".//div[@jsmodel='uIhXXc']");
            if (nd != null)
                return false;

            nd = node.SelectSingleNode(".//div[@jscontroller='UrRncd']/div/div/a[@class='C8nzq BmP5tf amp_r']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@jscontroller='UrRncd']/div/div/a[@class='C8nzq BmP5tf']");  // 25-10-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='KJDcUb']/a[@class='C8nzq BmP5tf amp_r']");  // 25-10-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='KJDcUb']/a[@class='C8nzq BmP5tf']");  // 25-10-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kp-blk Wnoohf OJXvsb']");
            if (nd != null)
            {
                return false;
            }
            //start 06-08-2019
            nd = node.SelectSingleNode(".//div[@class='f570C']");
            if (nd != null)
            {
                return true;
            }
            //end 06-08-2019

            nd = node.SelectSingleNode(".//g-card[@class='XqIXXe']");//21-08-2019
            if (nd != null)
            {
                return true;
            }//21-08-2019"

            if (!node.HasClass("srg")) // 19-09-2019
            {
                nd = node.SelectSingleNode(".//div[@class='mnr-c xpd O9g5cc uUPGi']|.//div[@class='ytwLQd']");//05-06-2020 missing classic links
                if (nd != null && node.SelectSingleNode(".//div[@class='EDblX m8vZ3d']") == null)   // 16-10-2019
                {
                    return false;
                }
                else if (nd != null && node.SelectSingleNode(".//div[@class='d5oMvf KJDcUb']") != null) // 22-01-2020 included selector for classic links
                {
                    return false;
                }
            }

            return (!node.HasClass("srg")); // && node.SelectSingleNode(".//div[@class='ZINbbc xpd']") == null);   // block                
        }

        private bool IsOrganic(HtmlNode node)
        {
            return (node.HasClass("srg") || node.SelectSingleNode(".//div[@class='oITGTd aSYQ6c']") != null
                || node.SelectSingleNode(".//div[@class='ZINbbc xpd']") != null
                || node.SelectSingleNode(".//div[@class='mnr-c xpd O9g5cc uUPGi']") != null
                || node.SelectSingleNode(".//div[@class='mnr-c']") != null //05-06-2020
                || node.Attributes["class"]?.Value == "mnr-c xpd O9g5cc uUPGi"
                || node.SelectSingleNode(".//div[@class='vC5Ym']") != null
                || node.SelectSingleNode(".//div[@class='kp-blk Wnoohf OJXvsb']") != null
                || (node.SelectSingleNode(".//g-card[@class='XqIXXe']") != null && node.SelectSingleNode(".//g-card[@id='tscffb']") != null));
        }

        internal object GetTop100GoogleUKMobileImages_PageURLs(string kw, string v1, string v2, string v3, string v4, string v5)
        {
            throw new NotImplementedException();
        }

        internal object GetTop100GoogleUKMobileImages_ImageURLs(string kw, string v1, string v2, string v3, string v4, string v5)
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
                if (!url.Contains("://")) // 30-04-2020
                    url = "http://" + url;

            if (url.Contains("&amp;grqid="))
                url = url.Remove(url.IndexOf("&amp;grqid="));

            //  13-12-2019
            if (url.Contains("&grqid="))
                url = url.Remove(url.IndexOf("&grqid="));

            if (url.Contains("\0"))
                url = url.Replace("\0", "%00");

            if ((url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("ftp://")) && (!url.Contains("/aclk?") && !url.Contains("search?num=100")))  // 30-04-2020 //18-02-2020 and 24-02-2020 included condition
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

            return WebUtility.HtmlEncode(url.Replace("\x00", "%00")).Replace("\\\\u003d", "=").Replace('\u0002', ' ').Replace('\u0018', ' ').Replace('\f', ' ').Trim(); //22-04-2020
        }

    }
}

