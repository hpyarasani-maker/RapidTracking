using HtmlAgilityPack;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace RapidTrackingLibrary
{
    public class iOS
    {
        int orgLinks;
        public string html; string seid = string.Empty;//23-06-2023
        public string ProcessDocument(string seid, string keyword, HtmlDocument doc, out int count)
        {
            this.seid = seid;//23-06-2023
            count = 0;

            if (doc == null) throw new Exception("No source found.");
            orgLinks = 0;
            string ndText = "";

            html = doc.DocumentNode.OuterHtml;
            StringBuilder sb = new StringBuilder();
            sb.Append("<searchResult searchEngine=\"" + seid + "\" keyword=\"" + WebUtility.HtmlEncode(keyword) + "\" date=\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\" >");
            try  //28-09-2020  try catch.
            {
                sb.Append("<section col=\"main\">");
                string topStuff = GetTopStuff(doc);
                ndText = topStuff;
                sb.Append(topStuff);

                HtmlNodeCollection nodeCol = doc.DocumentNode.SelectNodes("//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']/div[@class='MUxGbd v0nnCb lyLwlc']|//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']/div/div[@class='MUxGbd v0nnCb lyLwlc']");   //29-04-2020
                if (nodeCol != null)
                    nodeCol = nodeCol[nodeCol.Count - 1].SelectNodes("a/div");  //28-04-2020
                if (nodeCol == null)//20-09-2023
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div[@class='MjjYud']/div|//div[@id='rso']/div[@class='MjjYud']/block-component|.//div[contains(@class,'TzHB6b')]");//22-09-2023//21-09-2023//20-09-2023
                if (nodeCol == null || doc.DocumentNode.SelectNodes("//div[@class='WtZO4e']/div|//div[@classname='WtZO4e']/div")?.Count > 1)//25-09-2023
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div|//div[@id='rso']/g-card|//div[@id='taw']/div[@class='med']/div[2]/div|//div[@id='rso']/nav|//div[@id='rso']/block-component/div|//div[contains(@id, 'arc-srp')]/div/div[@class='MjjYud']|//div[contains(@id, 'arc-srp')]/div/div/div[@class='MjjYud']|.//div[contains(@class,'TzHB6b')]");//26-02-2024 removed selector//19-02-2024//09-02-2024//02-02-2024
                if (nodeCol != null && nodeCol.Count == 1)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div|//div[@class='vC5Ym DhKAUb']/div");  //23-03-2023  //17-09-2019
                if (nodeCol != null && nodeCol.Count <= 5 && doc.DocumentNode.SelectNodes("//div[contains(@id,'kp-wp-tab-')]") != null)//02-02-2024//31-01-2024
                    nodeCol = doc.DocumentNode.SelectNodes("//div[contains(@class,'Ww4FFb vt6azd DlUvEb')]|//div[contains(@class,'uVMCKf Ww4FFb vt6azd')]" +
                    "|//div[contains(@class,'wHYlTd Ww4FFb vt6azd')]|//div[contains(@class,'Lv2Cle Ww4FFb vt6azd')]|//div[contains(@class,'Ww4FFb vt6azd xpd')]" +
                    "|//div[contains(@class,'Ww4FFb vt6azd oGMpge')]|//div[contains(@class,'Ww4FFb vt6azd tRkSqb')]|//div[contains(@class,'Ww4FFb vt6azd F6CFcc')]" +
                    "|//div[@class='lU8tTd']");//12-03-2024
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//*[@id='tscffb']");

                if (nodeCol == null) throw new Exception("No block found.");
                //if (nodeCol == null) return string.Empty; 
                //if (nodeCol == null) goto BOTTOMSTUFF;             

                foreach (HtmlNode node in nodeCol)
                {
                    HtmlNode fsh = node.SelectSingleNode(".//*[@id='knowledge-finance-wholepage__fw-sticky-header']");
                    if (fsh != null)
                    {
                        HtmlNodeCollection nc = node.SelectNodes("./[@class='knowledge-finance-wholepage__section wp-ms']"); // /div[1]
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
                            n = node.SelectSingleNode(".//div[@class='PyJv1b gsmt PZPZlf rq9RNe']/span[@role='heading']");  // 20-11-2020 KP block selector
                        if (n == null)
                            n = node.SelectSingleNode(".//div[contains(@class,'DoxwDb PZPZlf')]");//27-09-2021 //23-09-2021 for missing KP block
                        if (n == null)
                            n = node.SelectSingleNode(".//div[@class='Ftghae iirjIb']");//16-09-2019 //
                        if (n == null)
                            n = node.SelectSingleNode(".//div[contains(@class,'PZPZlf')]");//27-02-2023 //15-11-2021 KP
                        if (n != null)//25-09-2023
                        {
                            if (node.SelectSingleNode(".//div[contains(@class,'WlTAzf ')]|.//div[@jsname='dTDiAc']") == null || node.SelectSingleNode(".//div[contains(@class,'XbtRGb qxsd')]") != null)//23-10-2023//25-09-2023//22-09-2023
                            {
                                string heading = n.InnerText;
                                sb.Append("<block type=\"knowledgeGraph\" url=\"\" title=\"" + SetTitle(heading) + "\" />");
                            }
                        }
                        /*if (n != null)//19-01-2023 //23-01-2023 Googlehotels else KP Block
                        {
                            string googleHotels = string.Empty;
                            HtmlNode Gh = node.SelectSingleNode(".//div[@class='fKw1wf']");
                            if (Gh != null)
                            {
                                googleHotels = GetGoogleHotels(Gh);
                                if (!string.IsNullOrEmpty(googleHotels))
                                {
                                    sb.Append("<block type=\"googleHotels\" url=\"\">");
                                    sb.Append(googleHotels);
                                    sb.Append("</block>");
                                }
                            }
                            else if (string.IsNullOrEmpty(googleHotels))
                            {
                                string heading = n.InnerText;
                                sb.Append("<block type=\"knowledgeGraph\" url=\"\" title=\"" + SetTitle(heading) + "\" />");
                            }
                        }*///19-01-2023 //23-01-2023 Googlehotels else KP Block
                           //n = node.SelectSingleNode(".//div[@class='kno-fiu kno-liu']");//11-08-2023 Images block
                           //if (n != null && node.SelectSingleNode(".//div[contains(@class,'PZPZlf')]") == null)//23-10-2023
                           //{
                           //    sb.Append("<block type=\"images\" url=\"\">");
                           //    sb.Append(GetImages(node));
                           //    sb.Append("</block>");
                           //}//26-10-2023//11-08-2023 commented
                           //n = node.SelectSingleNode(".//block-component/div[contains(@class, 'kno-result')]|.//div[@class='UDZeY fAgajc']");//20-09-2023//30-01-2023 Answer Card
                           //if (n != null)
                           //{
                           //    sb.Append("<block type=\"answerCard\" url=\"\">");
                           //    sb.Append(GetAnswerCard(n));
                           //    sb.Append("</block>");
                           //}//30-01-2023 Answer Card
                           //n = node.SelectSingleNode(".//g-img[@class='o8ebK']|.//g-img[@class='gRTVof']|.//div[@class='Xlcxdd']");//20-09-2023//10-10-2022//07-09-2022 missing KP block
                           //if (n != null && node.SelectSingleNode(".//div[@class='KrvXD']") == null)///13-12-2022
                           //{
                           //    sb.Append("<block type=\"maps\" url=\"\"></block>");
                           //} //25-10-2023 commented//07-09-2022
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
                                //Current 15-12-2020 swapped from bottom HtmlNodeCollection
                                HtmlNodeCollection nc = node.SelectNodes(".//div[contains(@class,'TzHB6b')]|//div[contains(@id, 'arc-srp')]/div/div[@class='MjjYud']|//div[contains(@id, 'arc-srp')]/div/div/div[@class='MjjYud']");//29-03-2023
                                if (nc == null)
                                    nc = node.SelectNodes(".//div[contains(@class,'TzHB6b')]|.//div[@class='MjjYud']/div/div/div[@class='Ww4FFb vt6azd xpd EtOod pkphOe']|.//div[@class='Lv2Cle Ww4FFb vt6azd']");//23-03-2023//14-12-2022
                                if (nc == null || nc.Count <= 2) //14-12-2022
                                    nc = node.SelectNodes(".//div[@class='TjcfIc eE3xqf B03h3d V14nKc ptcLIOszQJu__wholepage-card wp-ms']|.//div[@class='WvKfwe a3spGf']/div");//13-12-2022 //28-11-2022
                                if (nc == null)
                                    nc = node.SelectNodes(".//div[@class='WvKfwe']/div|.//div[@class='WvKfwe a3spGf']/div" +
                               "|.//div[@class='ChlgHf']|.//div[@class='a3spGf WvKfwe']/div|.//div[contains(@class,'TzHB6b mnr-c UBoxCb')]" + //17-11-2022
                                "|.//div[@class='WvKfwe a3spGf']/g-card|.//div[@class='WvKfwe a3spGf']/block-component");//20-05-2022  
                                //if (nc == null || node.SelectNodes(".//div[@id='kp-wp-tab-overview']/div") != null)//07-10-2021 answer card and PAA blocks
                                if (nc == null)//02-09-2022
                                    nc = node.SelectNodes(".//div[@id='kp-wp-tab-overview']/div"); //15-12-2020
                                //end of swapped
                                if (nc == null) //|.//div[@class='a3spGf WvKfwe']/div //23-05-2020
                                    nc = node.SelectNodes(".//div[@class='Kot7x eXEBMb Znsfnf']/div[@class='GhpATe pttBJc']"); //15-04-2020
                                if (nc == null)//|.//div[@class='kp-blk c2xzTb OJXvsb']//23-05-2020
                                    nc = node.SelectNodes(".//div[@class='UDZeY']/div|.//div[@class='vC5Ym']/div|.//div[@class='kp-blk cUnQKe Wnoohf OJXvsb']");       //23-05-2020
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
                        catch { }
                    }
                }

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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return string.Empty;

        }
        /// <summary>
        /// Regex to return mobile top 100 classic links
        /// </summary>
        /// <param name="seid"></param>
        /// <param name="keyword"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public string ProcessClassicLinks(string seid, string keyword, HtmlDocument doc)
        {
            if (doc == null) throw new Exception("No source found.");
            orgLinks = 0;
            html = doc.DocumentNode.OuterHtml;
            StringBuilder sb = new StringBuilder();
            sb.Append("<searchResult searchEngine=\"" + seid + "\" keyword=\"" + WebUtility.HtmlEncode(keyword) + "\" date=\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\" >");
            sb.Append("<section col=\"main\">");
            try
            {
                //string matchPattern = "(<div class=\\WKJDcUb\\W>*<a class=\\W[C8nzq BmP5tf]*[C8nzq JTuIPc]*[C8nzq Tj0U2 BmP5tf]*[C8nzq JTuIPc amp_r\\W data-amp=|\\W|C8nzq Tj0U2 BmP5tf amp_r\\W data-amp=|\\W|C8nzq BmP5tf amp_r\\W data-amp=|data-idx=\\W\\d\\W href=|\\W href=]*\"(.*?)\")|" +
                //        "(<div class=\\WXv4xee\\W>*<h3 class=\\WyuRUbf JtG40d V7Sr0\\W><a class=\\W[sXtWJb|sXtWJb gsrt amp_r|sXtWJb amp_r\\W data-amp=|\\W|\\W href=]*\"(.*?)\") ";
                string matchPattern = "<[a-z0-9\\W]{11}[a-z0-9A-Z\\s]*[0-9|a-b]{1}\"\\S<[a-z\\D]{9}[A-Z|a-z{1}][A-Z|0-9{1}]([a-z]{3}|[A-Za-z]{4})[a-zA-Z0-9\\s_]*\"\\s(data-amp|href|data-idx)\\D\"(.*?)\"";
                string URLpattern = "([\\w-./+?()&~'%!,-_.?$=;:]*)?";
                Regex re = new Regex(matchPattern + URLpattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mc = re.Matches(html);
                foreach (Match m in mc)
                {
                    string url = m.Groups[3].Value;
                    url = SetUrl(url);
                    if (!string.IsNullOrEmpty(url) && orgLinks < 100)
                    {
                        sb.Append("<item url=\"" + url + "\" />");
                        orgLinks++;
                    }
                }
            }
            finally { }

            sb.Append("</section>");
            sb.Append("<section col=\"right\">");
            sb.Append("</section>");
            sb.Append("</searchResult>");

            return sb.ToString();
        }

        public string GetRightStuff(HtmlDocument doc)
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
            return s.ToString();
        }

        public string GetBottomStuff(HtmlDocument doc)
        {
            StringBuilder s = new StringBuilder();
            //25-09-2019                        //swaped productlistedads 14-05-2020
            if (doc.DocumentNode.SelectSingleNode("//div[@class='mnr-c IGtt6d imgac']") != null || doc.DocumentNode.SelectSingleNode("//div[@class='IGtt6d imgac mnr-c']") != null) //22-06-2021 bottom PLAds 
            {
                //HtmlNode pla = doc.DocumentNode.SelectSingleNode(".//div[contains(@class, 'commercial-unit-mobile-top')]");//commented 18-01-2024
                //if (pla == null)
                //    pla = doc.DocumentNode.SelectSingleNode(".//div[contains(@class, 'commercial-unit-mobile-bottom')]");   // 18-09-2018 commented //18-01-2024
                HtmlNode pla = doc.DocumentNode.SelectSingleNode(".//div[contains(@class, 'commercial-unit-mobile-bottom')]");//18-01-2024
                if (pla != null)
                {
                    HtmlNode h3 = pla.SelectSingleNode(".//div[contains(@class,'dxR8gf')]/h3");//18-08-2022 PL item urls
                    if (h3 == null)
                        h3 = pla.SelectSingleNode(".//div[@class='richlist-top-shopping-title']/h3");
                    if (h3 == null)
                        h3 = pla.SelectSingleNode(".//div[@class='gsrt richlist-top-shopping-title']/h3");
                    if (h3 == null)
                        h3 = pla.SelectSingleNode(".//div[@class='gsrt dxR8gf']");

                    if (h3 != null)
                    {
                        if ((pla.SelectSingleNode(".//h3[contains(@class,'r')]") != null && pla.SelectSingleNode(".//h3[@role='heading']") != null || (pla.SelectSingleNode(".//div[@class='YW615c']") != null && pla.SelectSingleNode(".//div[@role='heading']") != null))//25-03-2022 //13-11-2019 //20-07-2020 included "contains" 
                         || h3.InnerText.StartsWith("Shop for") || h3.InnerText.StartsWith("See ") || WebUtility.HtmlDecode(h3.InnerText).StartsWith("Ads·See ") || h3.InnerText.StartsWith("See&nbsp;")//07-02-2022
                         || h3.InnerText.StartsWith("Ver ") || WebUtility.HtmlDecode(h3.InnerText).StartsWith("Anuncios·Ver ")  //15-07-2020 included for product lists ads
                         || h3.InnerText.StartsWith("Anúncios&middot;Ver ") || WebUtility.HtmlDecode(h3.InnerText).StartsWith("Ads·") //09-08-2021 //10-07-2021 //16-07-2020
                         || ((pla.SelectSingleNode(".//h3[contains(@class,'xZu9ed mfMhoc')]") != null || h3.HasClass("xc15De")) && pla.SelectSingleNode(".//h3[@role='heading']") != null)//26-09-2022 //10-07-2021
                         || (pla.SelectSingleNode(".//div[@class='Mckyte']") != null) //12-08-2021
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
                                    if (!string.IsNullOrEmpty(nd.SelectSingleNode(".//h4|.//div[contains(@class,'bXPcId pymv4e')]").InnerText) && !string.IsNullOrEmpty(url.Trim())) //04-11-2022   //13-11-2019
                                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(nd.SelectSingleNode(".//h4|.//div[contains(@class,'bXPcId pymv4e')]").InnerText) + "\" />"); //05-01-2023
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
                            s.Append("</block>");
                        }
                    }
                }
            }//25-09-2019
            // Ads  added or condition
            HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//div[@id='tadsb']/div[@class='C4eCVc c']/ol/li|.//div[@id='tadsb']/div[@class='uEierd']"); //18-09-2020 included selector for bottom adwords // 24-04-2020   included class selector
            if (col != null)
            {
                s.Append("<block type=\"adwords\" url=\"\">");
                foreach (HtmlNode nd in col)
                {
                    HtmlNode n = nd.SelectSingleNode(".//div[@class='d5oMvf KJDcUb']/a|.//div[contains(@class,'v5yQqb')]/a"); //11-11-2021
                    if (n == null)
                        n = nd.SelectSingleNode(".//div[@class='d5oMvf KJDcUb dJMePd T4Yo']/a|.//div[@class='hJlC3']/a");//21-02-2024  //22-06-2020
                    if (n == null)
                        n = nd.SelectSingleNode(".//div[@class='d5oMvf KJDcUb WzRKRb']/a"); // 17-12-2019
                    if (n == null)
                        n = nd.SelectSingleNode(".//div/a[2]|.//div/div/div[@class='d5oMvf']/a");
                    if (n != null)
                    {
                        //22-06-2020
                        string title = string.Empty;
                        HtmlNode t = n.SelectSingleNode(".//h3|.//div[@role='heading']");
                        if (t != null)
                            title = t.InnerText;
                        //end 22-06-2020

                        try  //28-09-2020  try catch.
                        {
                            //24-08-2020
                            string url = string.Empty;
                            //27-08-2020
                            if (!string.IsNullOrEmpty(GetRedirectedUrl_TextAds(n.Attributes["href"]?.Value))) // 12-06-2020
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
                            else if (!string.IsNullOrEmpty(GetRedirectedUrl_TextAds(n.SelectSingleNode(".//span[@class='yKd8Hd qzEoUe']")?.InnerText)))//08-02-2022
                            {
                                url = GetRedirectedUrl_TextAds(n.SelectSingleNode(".//span[@class='yKd8Hd qzEoUe']")?.InnerText);
                            }//08-02-2022
                            else
                            {
                                HtmlNode n1 = nd.SelectSingleNode(".//div[@class='ads-visurl']/cite|.//div[@class='QNz0M ellip GsCRYb']/cite");
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


        public string GetTopStuff(HtmlDocument doc)
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
                // product listed ads //start of change 11-01-2022 line number 409 to 498
                if (doc.DocumentNode.SelectSingleNode("//div[@class='mnr-c IGtt6d imgac cTMkTb']") != null
                   || doc.DocumentNode.SelectSingleNode("//div[@class='mnr-c IGtt6d imgac qs-ic fp-w cTMkTb']") != null
                   || doc.DocumentNode.SelectSingleNode("//div[contains(@class,'IGtt6d imgac mnr-c')]|//div[@id='tads']/g-card/div[contains(@class, 'mnr-c')]") != null //18-01-2024  //01-02-2022 //16-09-2021 missing ProductListAds
                   || doc.DocumentNode.SelectSingleNode("//div[@id='activities-carousel-container']") != null //11-01-2022
                   || doc.DocumentNode.SelectSingleNode("//div[@class='mnr-c Ioy6jb']") != null //07-02-2022
                   )
                {
                    HtmlNode pla = crNode.SelectSingleNode(".//div[contains(@class, 'commercial-unit-mobile-top')]");
                    if (pla == null)
                        pla = doc.DocumentNode.SelectSingleNode(".//div[contains(@class, 'commercial-unit-mobile-bottom')]");   // 18-09-2018
                    if (pla == null)
                        pla = doc.DocumentNode.SelectSingleNode(".//div[@id='tauc']/div[contains(@class, 'mnr-c')]");   // 11-01-2022
                    if (pla == null)
                        pla = doc.DocumentNode.SelectSingleNode(".//div[@id='tads']/div/div[contains(@class, 'mnr-c')]|.//div[@id='tads']/g-card/div[contains(@class,'mnr-c')]");//18-01-2024 //07-02-2022
                    if (pla == null)
                        pla = doc.DocumentNode.SelectSingleNode(".//div/g-scrolling-carousel[@id='kV5x0e']");//16-01-2024
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
                        if (h3 == null)
                            h3 = pla.SelectSingleNode(".//div[@class='jGAUQb']"); //07-02-2022
                        if (h3 == null)
                            h3 = pla.SelectSingleNode(".//h3[@class='TWApbd']/div[@class='xc15De']|.//div[@class='TWApbd']/h3[@class='xc15De']");//08-01-2024  //11-01-2022
                        if (h3 == null)
                            h3 = pla.SelectSingleNode(".//div[@class='YW615c']"); //25-03-2022
                        if (h3 == null)
                            h3 = pla.SelectSingleNode(".//div[@class='Mbv3zb']");//28-12-2023
                        if (h3 == null)
                            h3 = pla.SelectSingleNode(".//div[@class='dGACyd']|.//div[@class='UULivd']");//18-01-2024//16-01-2024
                        if (h3 != null)
                        {
                            if ((pla.SelectSingleNode(".//h3[contains(@class,'r')]") != null && pla.SelectSingleNode(".//h3[@role='heading']") != null) //13-11-2019 //20-07-2020 included "contains" 
                                || h3.InnerText.StartsWith("Shop for") || h3.InnerText.StartsWith("See ") || WebUtility.HtmlDecode(h3.InnerText).StartsWith("Ads·See ") || h3.InnerText.StartsWith("See&nbsp;")//07-02-2022
                                || h3.InnerText.StartsWith("Ver ") || WebUtility.HtmlDecode(h3.InnerText).StartsWith("Anuncios·Ver ") || WebUtility.HtmlDecode(h3.InnerText).StartsWith("Sponsored")//18-01-2024  //15-07-2020 included for product lists ads
                                || h3.InnerText.StartsWith("Anúncios&middot;Ver ") || WebUtility.HtmlDecode(h3.InnerText).StartsWith("Ads·") //09-08-2021 //10-07-2021 //16-07-2020
                                || (pla.SelectSingleNode(".//h3[contains(@class,'xZu9ed mfMhoc')]") != null && pla.SelectSingleNode(".//h3[@role='heading']") != null) //10-07-2021
                                || (pla.SelectSingleNode(".//div[@class='Mckyte']") != null) //12-08-2021
                                || (pla.SelectSingleNode(".//div[@class='ZPze1e']") != null) //27-03-2023
                                || (pla.SelectSingleNode(".//div[@role='heading']") != null)   // 08-11-2023 
                                )
                            {
                                s.Append("<block type=\"productListedAds\" url=\"\">");

                                HtmlNodeCollection cl = pla.SelectNodes(".//a[@class='pla-unit eUPzHb']|.//div[@class='mnr-c pla-unit']/a[2]" +
                                  "|.//a[@class='plantl pla-unit-single-clickable-target clickable-card']|.//g-inner-card[contains(@class,'stOtnd VoEfsd')]/div/div/a" +
                                  "|.//g-inner-card[contains(@class,'B5kg8b bcWZB')]/a");//27-09-2022//25-09-2020 updated contains //15-07-20
                                if (cl == null)
                                    cl = pla.SelectNodes(".//div[@class='ZPze1e']/a"); //07-02-2022
                                if (cl == null)
                                    cl = pla.SelectNodes(".//a[@class='pla-unit']");
                                if (cl == null)
                                    cl = pla.SelectNodes(".//div[@class='fyZ0Ff']/a");  //25-06-2020
                                if (cl != null)
                                {
                                    foreach (HtmlNode nd in cl)
                                    {
                                        var url = nd.Attributes["href"].Value;
                                        url = GetRedirectedUrl(url);
                                        //25-06-2020
                                        string title;
                                        if (nd.SelectSingleNode(".//h4") != null)
                                            title = nd.SelectSingleNode(".//h4").InnerText;
                                        else
                                            title = nd.InnerText;

                                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(url.Trim()))
                                        {
                                            //26-06-2020
                                            int indx = url.IndexOf("%3F");
                                            if (indx > 0)
                                                url = url.Remove(indx);
                                            //end 26-06-2020
                                            s.Append("<item url=\"" + SetUrl(url.Replace("&nbsp;", "")) + "\" title=\"" + SetTitle(title.Replace("&nbsp;", " ")) + "\" />");
                                        }
                                        //end 25-06-2020
                                    }
                                }
                                //09-09-2019
                                else if (doc.DocumentNode.SelectNodes(".//div[@class='RL6uuc gws-product_ads-showcase_immersive__immersive-tile']//div[@class='PhX95']|//div[@id='activities-carousel-container']") != null) //11-01-2022
                                {
                                    HtmlNodeCollection hidedNodes = doc.DocumentNode.SelectNodes(".//div[@class='RL6uuc gws-product_ads-showcase_immersive__immersive-tile']"); //01-11-2019
                                    if (hidedNodes == null)
                                        hidedNodes = doc.DocumentNode.SelectNodes(".//div[@class='OkuxMe']");   // 13-11-2019
                                    if (hidedNodes == null)
                                        hidedNodes = doc.DocumentNode.SelectNodes(".//div[@class='roG2hd']");   //11-01-2022
                                    HtmlNode urlnode, innertextNode;
                                    if (hidedNodes != null)
                                    {
                                        foreach (HtmlNode planode in hidedNodes)
                                        {
                                            //01-11-2019
                                            urlnode = planode.SelectSingleNode(".//div[@class='PhX95']|.//div[@class='UBq0ab']");
                                            innertextNode = planode.SelectSingleNode(".//div[@class='Ved4gc']|.//div[@class='UBq0ab']");
                                            string url = string.Empty; //11-01-2022
                                            if (urlnode != null)
                                                //11-11-2019
                                                url = GetProductListedUrls(urlnode.InnerText.ToString().Replace("&nbsp;", ""));
                                            else
                                            {
                                                url = planode.SelectSingleNode(".//g-inner-card/a").Attributes["href"]?.Value;
                                                innertextNode = planode.SelectSingleNode(".//div[@class='gCv54b']");
                                                url = GetProductListedUrls(url.Replace("&nbsp;", ""));
                                            } //end of 11-01-2022

                                            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(innertextNode.InnerText))    //13-11-2019
                                                s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(innertextNode.InnerText) + "\" />");  // 04-11-2019
                                        }
                                    }
                                }

                                s.Append("</block>");
                            }
                        }
                    }
                }//end of change //11-01-2022

                HtmlNodeCollection col = crNode.SelectNodes(".//div[contains(@id,'tads')]/ol/li");
                if (col == null)
                    col = doc.DocumentNode.SelectNodes("//div[@id='tads']/div/ol/li|//div[@jsname='hWE2jd']|//div[@id='tads']/div[@class='uEierd']|.//div[@id='tads']/div/div[@class='uEierd']" +
                       "|//div[@id='tads']/div[@class='mnr-c O9g5cc uUPGi']|//div[contains(@class,'yDDB0e')]");//19-08-2022//28-03-2022 "/div" included//04-10-2021 updated selector for missing

                if (col != null)
                {
                    s.Append("<block type=\"adwords\" url=\"\">");
                    foreach (HtmlNode nd in col)
                    {
                        HtmlNode n = nd.SelectSingleNode(".//div[@class='ad_cclk']/a[2]");
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='d5oMvf']/a|.//div[contains(@class,'v5yQqb')]/a"); //11-11-2021
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='d5oMvf KJDcUb']/a|.//div[@class='hJlC3']/a");//21-02-2024
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='d5oMvf KJDcUb WzRKRb']/a");  // 29-11-2019
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='IM8JJ']/a");//19-08-2022
                        if (n == null)
                            n = nd.SelectSingleNode(".//div/a[@class='V0MxL']");    // changes on 28-06-2019
                        if (n == null)
                            n = nd.SelectSingleNode(".//a[@jsname='wOJZib']");  // 01-04-2020
                        if (n != null)
                        {
                            string title = (n.SelectSingleNode(".//h3") != null) ? n.SelectSingleNode(".//h3").InnerText
                                : (n.SelectSingleNode(".//div[@role='heading']") != null) ? n.SelectSingleNode(".//div[@role='heading']").InnerText
                                : (n.SelectSingleNode(".//div[@class='mdzVfb gAWudd']") != null) ? n.SelectSingleNode(".//div[@class='mdzVfb gAWudd']").InnerText  // 01-04-2020
                                : (n.SelectSingleNode(".//div[@class='pXVgMc']") != null) ? n.SelectSingleNode(".//div[@class='pXVgMc']").InnerText //19-08-2022
                                : n.InnerText;

                            //24-08-2020
                            try  //28-09-2020  try catch.
                            {
                                string url = string.Empty;
                                //27-08-2020
                                if (!string.IsNullOrEmpty(GetRedirectedUrl_TextAds(n.Attributes["href"]?.Value))) // 12-06-2020
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
                                else if (!string.IsNullOrEmpty(GetRedirectedUrl_TextAds(n.SelectSingleNode(".//span[@class='yKd8Hd qzEoUe']")?.InnerText)))//08-02-2022
                                {
                                    url = GetRedirectedUrl_TextAds(n.SelectSingleNode(".//span[@class='yKd8Hd qzEoUe']")?.InnerText);
                                }//08-02-2022
                                else
                                {
                                    HtmlNode n1 = nd.SelectSingleNode(".//div[@class='ads-visurl']/cite|.//div[@class='QNz0M ellip GsCRYb']/cite");
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
                        HtmlNodeCollection App2 = answernode.SelectNodes(".//div[@class='ytwLQd']/h3/a");
                        if (App2 == null)
                            App2 = answernode.SelectNodes(".//div[@class='WLSb4b']");//23-06-2020
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

                //starts 01-01-2021
                HtmlNode kgNode = crNode.SelectSingleNode(".//div[@class='p7xMX qs-ic fp-w']");//01-01-2021
                if (kgNode == null)
                    kgNode = crNode.SelectSingleNode(".//div[@class='c ptJHdc commercial-unit-mobile-top']|.//div[@class='SPZz6b']");
                // ends 01-01-2021


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

        public string ProcessNode(HtmlNode node)
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

        public string ProcessOrganic(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds;

            if (node.SelectSingleNode(".//div[@jscontroller='iht5n']") != null)
                nds = node.SelectNodes(".//div[@jscontroller='iht5n']/div");
            else
            {
                //nds = node.SelectNodes(".//div[@class='mnr-c waTp2e xpd O9g5cc uUPGi']|.//div[@class='mnr-c luh4tb xpd O9g5cc uUPGi']|.//div[@class='mnr-c xpd O9g5cc uUPGi']|.//div[@class='g mnr-c']");//12=11-2021 CL//12-11-2021//24-03-2021//15-12-2020 removed selector//06-10-2020 classic link selector //19-06-2020   //20-01-2020 selector changed for two classic links
                nds = node.SelectNodes(".//div[@class='mnr-c waTp2e xpd O9g5cc uUPGi']|.//div[@class='mnr-c luh4tb xpd O9g5cc uUPGi']"); //15-11-2021
                if (nds == null)//26-03-2021
                    nds = (node.SelectNodes(".//div[contains(@id,'tsuid')]//a[@class='Xhbgy']") != null && node.SelectNodes(".//div[contains(@class,'KJDcUb')]") != null) ? nds = node.SelectNodes(".//div[@class='mnr-c xpd O9g5cc uUPGi']") : nds = null;
                //end 26-03-2021
                if (nds == null)
                    //nds = node.SelectNodes(".//div[@class='BYM4Nd']|.//div[@class='mnr-c OH1ZUd xpd O9g5cc uUPGi']");//27-01-2022 classic links //14-01-2022
                    nds = (node.SelectNodes(".//div[@class='BYM4Nd']|.//div[@class='mnr-c OH1ZUd xpd O9g5cc uUPGi']") != null) ? nds = node.SelectNodes(".//div[@class='UDZeY']/div/div") : nds = null;
                if (nds == null && node.SelectNodes(".//div[@class='BYM4Nd']|.//div[@class='mnr-c OH1ZUd xpd O9g5cc uUPGi']") != null) //26-09-2022//21-02-2022
                    nds = node.SelectNodes(".//div[@class='BYM4Nd']|.//div[@class='mnr-c OH1ZUd xpd O9g5cc uUPGi']|.//div[@class='Ww4FFb vt6azd xpd EtOod pkphOe']");//26-09-2022//21-02-2022
                if (nds == null)
                    if (node.SelectSingleNode(".//div[contains(@class,'YgXj7b')]") == null) //08-05-2021 applied contains//19-01-2021
                        nds = node.SelectNodes(".//div[contains(@class,'KJDcUb')]|.//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']|.//g-card[@id='tscffb']|.//div[@class='mnr-c PHap3c']|.//div[contains(@class, 'Ww4FFb vt6azd PHap3c')]" + //14-12-2022
                         "|.//div[@jsname='wRSfy']|.//g-card[@class='g F6CFcc']|.//div[@class='mnr-c xpd O9g5cc uUPGi']|.//div[@class='urrG9 v5yQqb jqWpsc']|.//g-card[@class='T98FId']" + //11-07-2023 //31-05-2022
                        "|.//div[@class='mnr-c']/div/div[@class='P8ujBc v5yQqb jqWpsc']|.//div[contains(@class,'EtOod pkphOe')]|.//div[@class='mnr-c']/div/div[@class='P8ujBc jqWpsc']" +
                        //"|.//div[@class='fhQnRd']|.//div[@id='iur']|.//div[contains(@class,'cUnQKe wHYlTd Ww4FFb vt6azd')]|.//div[@class='uVMCKf Ww4FFb vt6azd']|.//div[@class='Ww4FFb vt6azd tRkSqb']|.//div[@class='xLjnU']");//28-08-2023 //18-08-2023
                        "|.//div[@class='fhQnRd']|.//div[@id='iur']|.//div[contains(@class,'Ww4FFb vt6azd')]");//01-09-2023//30-08-2023

                if (nds == null) //24-11-2022
                    nds = node.SelectNodes(".//div[@class='P8ujBc v5yQqb jqWpsc']|.//div[contains(@class,'Ww4FFb vt6azd')]|.//div[contains(@class,'EtOod pkphOe')]");//06-03-2023//16-01-2023 //24-11-2022//14-09-2022
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='mnr-c O9g5cc uUPGi']|.//div[@class='mnr-c xpd O9g5cc uUPGi']|.//div[@class='HD8Pae mnr-c xpd O9g5cc uUPGi']|.//div[@class='urrG9 v5yQqb jqWpsc']" + //31-05-2022
                        "|.//div/g-card[@class='XqIXXe']|.//g-card[@id='tscffb']|.//g-card[@class='g F6CFcc']|.//div[@class='khgTR lWEpfd']" +
                        "|.//div[@class='khgTR R5lVqb']|.//div[@class='mnr-c fp-w qs-ic aig-grd']|.//g-card[@class='URhAHe']" +
                        "|.//div[@class='mnr-c IcwJCe']|.//div[@class='g card-section svwwZ']|.//div[@class='g mnr-c']" + //15-11-2021  //03-11-2020//26-08-2020 incuded contains functions to the selector//29-07-2020 //20-05-2020 missing classic link //05-06-2020
                        "|.//div[contains(@class,'card-section')]|.//div[@class='wU9Tkd']|.//div[@jsname='wRSfy']|.//div[@class='tKdlvb jqWpsc']|.//div[@class='mnr-c YibVsd']" +//16-12-2021
                         //"|.//div[@class='mnr-c YibVsd']|.//div[@class='mnr-c xpd EtOod pkphOe']|.//div[contains(@class,'EtOod pkphOe')]");//30-06-2022//10-06-2022//16-12-2021 commented //29-06-2022
                         "|.//div[@class='mnr-c YibVsd']|.//div[contains(@class,'EtOod pkphOe')]|.//div[contains(@class,'Ww4FFb vt6azd')]");//02-09-2022 video block
                if (nds == null)//22-02-2022
                    if (node.Attributes["class"]?.Value == "mnr-c" && node.SelectSingleNode(".//div/div[contains(@class,'P8ujBc')]") != null)//22-02-2022
                        nds = node.SelectNodes(".//div/div[contains(@class,'P8ujBc')]");//22-02-2022
                if (nds == null)
                    if (node.Attributes["class"]?.Value == "mnr-c xpd O9g5cc uUPGi") //09-09-2021 applied ? condition
                        nds = node.SelectNodes(".//div[contains(@class,'KJDcUb')]"); //28-07-2020 //29-07-2020 included contains function
                if (nds == null)
                    nds = node.SelectNodes(".//div[@class='setTDc']|.//div[contains(@class, 'P8ujBc v5yQqb')]|.//div[@class='WFyfFf yOwOKc']");//14-02-2024
                if (nds == null && node.Attributes["jsname"]?.Value == "pKB8Bc")//14-02-2024
                    nds = node.SelectNodes(".");//14-02-2024//21-09-2023 //02-02-2022 moved from 687 line   // 25-10-2019
            }
            if (nds != null)
            {
                foreach (HtmlNode nd in nds)
                {
                    try
                    {
                        //12-11-2021 duplicates CLs
                        if (nd.Attributes["class"]?.Value != null)
                        {
                            if (nd.SelectSingleNode(".//div[@class='" + nd.Attributes["class"].Value + "']") != null)
                                continue;
                        }//12-11-2021
                        //14-11-2019
                        if (nd.SelectSingleNode(".//div[@jscontroller='i5z2Rc']") != null
                            || nd.SelectSingleNode(".//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']") != null //13-12-2019
                             || nd.SelectSingleNode(".//div[@class='MUxGbd v0nnCb lyLwlc']") != null    //16-12-2020
                            || nd.SelectSingleNode(".//div[@class='E8hWLe SVMeif BmP5tf']") != null) //21-07-2022
                        {
                            s.Append(GetSiteLinks(nd)); if (nd.SelectSingleNode(".//div[@class='ki5rnd']|.//div[@class='yR4jwc']|.//div[@class='UyqAp']|.//div[@class='qs-io aig-lst']") != null)//30-08-2023 
                                continue;
                        }
                        //21-02-2020  included selector for the Apps Block
                        if (nd.SelectSingleNode(".//div[@class='ki5rnd']|.//div[@class='yR4jwc']|.//div[@class='qs-io aig-lst']") != null)//30-08-2023 //31-08-2020  //20-05-2020 included selector for app block
                        {
                            s.Append(GetApps(nd));
                            continue;
                        }

                        //27-09-2019
                        if (nd.HasClass("F6CFcc") || nd.SelectSingleNode(".//g-inner-card[@class='Bf5NPb']|.//g-tray-header[contains(@class, 'Bf5NPb')]") != null)//30-08-2023//twitter block //18-08-2023  
                        {
                            if (nd.SelectSingleNode(".//div[contains(@class,'qdrjAc Dwsemf')]|.//div[@class='Bv2VAe']") != null)//30-08-2023 //28-10-2021  //11-11-2019  //11-11-2019
                            {
                                s.Append(GetTwitterCards(nd));
                                continue;
                            }
                        }
                        if (!nd.HasClass("T98FId") && (nd.HasClass("fhQnRd") || nd.SelectSingleNode(".//div[contains(@class,'kno-fb-ctx')]") != null))//11-07-2023//02-04-2023//01-02
                        {
                            s.Append("<block type=\"topStories\" url=\"\">");
                            s.Append(GetTopStories(nd)); //02-04-2023 changed param node to nd
                            s.Append("</block>");
                            continue;
                        }//01-02-2023 top stories
                        if (nd.Id == "iur")//01-02-2023 images
                        {
                            s.Append("<block type=\"images\" url=\"\">");
                            s.Append(GetImages(nd)); //02-04-2023 changed param node to nd
                            s.Append("</block>");
                            continue;
                        }//01-02-2023 images
                        if (nd.SelectSingleNode(".//div[@class='ouy7Mc']") != null && (node.SelectNodes(".//div[@class='AuVD wHYlTd Ww4FFb vt6azd']") == null || !nd.HasClass("AuVD wHYlTd Ww4FFb vt6azd")))//30-08-2023//24-08-2023//02-04-2023 People also Ask
                        {
                            s.Append("<block type=\"peopleAlsoAsk\" url=\"\">");
                            s.Append(PeopleAlsoAsk(nd));
                            s.Append("</block>");
                            continue;
                        }//People also ask
                        if (nd.HasClass("T98FId"))//PopularProducts //11-07-2023
                        {
                            s.Append("<block type=\"popularProducts\" url=\"\">");
                            s.Append(GetPopularProducts(node));
                            s.Append("</block>");
                            continue;
                        }//PopularProducts //11-07-2023
                        if (nd.SelectSingleNode(".//g-inner-card[@class='zf84ud THG0oc VoEfsd']") != null)
                            continue;
                        //end 27-09-2019

                        try
                        {
                            // 17-10-2019
                            if (nd.Name == "g-card" || nd.Attributes["class"]?.Value == "HD8Pae mnr-c xpd O9g5cc uUPGi" || nd.Attributes["jsname"]?.Value == "wRSfy") //12-10-2021 //11-10-2021
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


                            HtmlNode img = nd.SelectSingleNode(".//div[@class='G5NbBd']/div|.//g-img/img[@class='rISBZc zr758c']|.//span[@class='z1asCe UIgqBe']|.//div[contains(@class, 'i5w0Le')]|.//div[@class='Ylm8Fc']|.//div[@class='HaDSKc']");//10-06-2023//15-02-2023//26-05-2022//16-12-2021

                            if (img != null)
                            {
                                try
                                {
                                    //if (Regex.IsMatch(img.OuterHtml, "id=\"vidthumb\\d*\"") || nd.SelectSingleNode(".//div[contains(@class,'YgXj7b')]|.//div[contains(@class,'qW7zYd')]") != null || node.SelectSingleNode(".//div[@class='YgXj7b']|.//div[@class='Y37F6d Nn2Stf']") != null || nd.SelectSingleNode(".//div[@class='hq7Nmd']|.//div[@class='UT9Awd S3PB2d']") != null) //04-05-2022
                                    if (Regex.IsMatch(img.OuterHtml, "id=\"vidthumb\\d*\"") || Regex.IsMatch(img.OuterHtml, "id=\"dimg_\\d*\"") || img.Attributes["class"].Value.Contains("__video-result") || nd.SelectSingleNode(".//div[contains(@class,'c8rnLc flgn0c')]") != null || nd.SelectSingleNode(".//video-voyager") != null || nd.SelectSingleNode(".//div[@class='J1mWY']") != null)//10-06-2023//29-03-2023//23-03-2023//08-05-2021 contains//09-02-2021 //05-01-2021 //21-12-2020//16-12-2020 //14-12-2020
                                    {
                                        HtmlNode vdo = nd.SelectSingleNode(".//a[contains(@class,'BmP5tf')]"); //11-11-2021
                                        // video block.
                                        if (vdo == null)
                                            vdo = nd.SelectSingleNode(".//div[@class='th N3nEGc']/a|.//a[@class='d3q37c']");//14-02-2024
                                        if (vdo != null && nd.SelectNodes(".//g-scrolling-carousel[@class='kQ9KOd']") == null)//28-09-2022
                                        {
                                            string url = vdo.Attributes["href"].Value;
                                            string title = ""; // vdo.SelectSingleNode(".//div[contains(@class, 'MUxGbd v0nnCb')]").InnerText; //13-07-2022 //22-03-2021
                                            HtmlNode t = vdo.SelectSingleNode(".//div[contains(@class, 'MUxGbd v0nnCb')]");
                                            if (t == null)
                                                t = nd.SelectSingleNode(".//div[@role='heading']");//12-12-2023
                                            if (t != null)
                                                title = t.InnerText; //end 13-07-2022
                                            if (url.StartsWith("http") || url.StartsWith("https") || url.StartsWith("ftp")) //30-04-2020
                                            {
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
                                n = nd.SelectSingleNode(".//a[contains(@class,'cz3goc BmP5tf')]");//03-02-2023 //31-05-2022
                            if (n == null)
                                n = nd.SelectSingleNode(".//a[contains(@class,'sXtWJb')]"); //16-12-2020
                            if (n == null)
                                n = nd.SelectSingleNode(".//g-link/a");
                            if (n == null)
                            {
                                n = nd.SelectSingleNode(".//div[@class='rc']");
                                if (n != null)
                                    n = nd.SelectSingleNode(".//h3[@class='r']/a|.//h3[@class='r']/div/a|.//h3[contains(@class,'yuRUbf JtG40d')]/a"); //04-11-2020 //09-06-2020 //16-09-2020 included selector for classic link
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
                             || nd.Attributes["class"].Value == "MGqjK" || nd.Attributes["class"].Value == "setTDc" || nd.Attributes["class"].Value.Contains("khgTR") //26-08-2020    // 20-05-2020
                             || node.Attributes["class"]?.Value == "mnr-c xpd O9g5cc uUPGi" || nd.Attributes["class"].Value == "KJDcUb" // 14-12-2020  //20-01-2020 // selectors for two classic links block
                             || nd.Attributes["class"].Value == "mnr-c luh4tb xpd O9g5cc uUPGi" || nd.Attributes["class"].Value == "mnr-c PHap3c" //05-01-2021
                             || nd.Attributes["class"].Value == "mnr-c OH1ZUd xpd O9g5cc uUPGi" //27-01-2022 classic links
                             || nd.Attributes["class"].Value == "g card-section svwwZ" || nd.Attributes["class"].Value == "card-section"  //15-12-2020//03-11-2020 //06-10-2020 classic type block type
                             || nd.Attributes["class"].Value == "d5oMvf KJDcUb" || nd.Attributes["class"].Value == "c6gxKe card-section" //20-05-2021) //09-02-2021
                             || nd.Attributes["class"].Value.Contains("KJDcUb") || nd.Attributes["class"].Value == "mnr-c OH1ZUd xpd O9g5cc uUPGi" //27-01-2022 classic links //09-09-2021 missing classic links
                                                                                                                                                   // || nd.Attributes["class"].Value.Contains("mnr-c xpd EtOod pkphOe")//30-09-2022 commented //10-06-2022
                             || nd.Attributes["class"].Value.Contains("EtOod pkphOe") || nd.Attributes["class"].Value.Contains("WFyfFf")//14-02-2024 //30-06-2022//29-06-2022
                             || nd.Attributes["class"].Value == "wU9Tkd" || nd.Attributes["class"].Value.Contains("P8ujBc") //22-02-2022 //01-02-2022 //10-07-2021
                             || nd.Attributes["class"].Value == "g card-section" || nd.Attributes["class"].Value == "card-section svwwZ") //21-09-2023//30-08-2021 missing classic link
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

                                HtmlNode img = nd.SelectSingleNode(".//g-img[contains(@class,'P64nJb')]//img|.//div[@class='BNeawe wyrwXc HrGdeb']|.//div[@class='Y37F6d Nn2Stf']/img"); //08-04-2022
                                if (img != null)
                                {
                                    try
                                    {
                                        //19-10-2021 commented
                                        if (Regex.IsMatch(img.OuterHtml, "id=\"vidthumb\\d*\"") || nd.SelectSingleNode(".//div[contains(@class,'YgXj7b')]|.//div[contains(@class,'qW7zYd')]|.//div[@class='Y37F6d Nn2Stf']") != null || nd.SelectSingleNode(".//div[@class='hq7Nmd']") != null) //01-06-2022
                                        {
                                            HtmlNode n = nd.SelectSingleNode(".//h3[@class='r']/a");
                                            // video block.
                                            if (n == null)
                                                n = nd.SelectSingleNode(".//div[@class='th N3nEGc']/a");
                                            if (n == null) //11-11-2021
                                                n = nd.SelectSingleNode(".//a[contains(@class,'BmP5tf')]");  // 10-06-2020 swapped from below //08-01-2020 //included selector for video block
                                            //if (n == null)
                                            //    n = nd.SelectSingleNode(".//a[@class='C8nzq BmP5tf']");  // 10-06-2020 swapped from above
                                            //if (n == null)
                                            //    n = nd.SelectSingleNode(".//a[@class='C8nzq Tj0U2 BmP5tf']");    // 29-11-2019
                                            //end 11-11-2021
                                            if (n == null)
                                                n = nd.SelectSingleNode(".//div[@class='au0C1b u78HIe']/a"); //15-12-2020 video selector
                                            string url = n.Attributes["href"].Value;
                                            //string title = n.InnerText.Replace("'", "").Replace("\"", ""); //commented on 02-03-2021
                                            string title = nd.SelectSingleNode(".//div[@class='BNeawe UwRFLe']") != null ? nd.SelectSingleNode(".//div[@class='BNeawe UwRFLe']").InnerText : n.InnerText.Replace("'", "").Replace("\"", ""); //02-03-2021 title for video block item urls
                                            //28-10-2019
                                            if (n.SelectSingleNode(".//div[@role='heading']") != null)
                                                title = n.SelectSingleNode(".//div[@role='heading']").InnerText;
                                            if (url.StartsWith("http") || url.StartsWith("https") || url.StartsWith("ftp")) //30-04-2020
                                            {
                                                if (!s.ToString().Contains("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />"))//16-02-2021 video block if condition to avoid duplicates with classic link
                                                {
                                                    s.Append("<block type=\"video\" url=\"\">");
                                                    s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                                                    s.Append("</block>");
                                                }
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
                                    nv = nd.SelectSingleNode(".//div[contains(@class,'Z26q7c')]/div/a");////15-07-2022 13-07-2022
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
                                    nv = nd.SelectSingleNode(".//div[@class='P8ujBc v5yQqb jqWpsc']/a");//16-11-2021 TS Item urls
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//div[@class='P8ujBc jqWpsc']/a"); //22-11-2021 for bad classic links
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//g-link/a");
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//div[@class='fM8c FUksre']/a"); //22-06-2020
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//a[contains(@class,'sXtWJb')]"); //16-12-2020
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//a[contains(@class,'cz3goc BmP5tf')]");//06-10-2022
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//a[@class='d3q37c']");//14-02-2024
                                if (nv == null)
                                    nv = nd.SelectSingleNode(".//div/a");  //25-06-2020
                                if (nv == null)
                                {
                                    nv = nd.SelectSingleNode(".//div[@class='rc']|.//div[@class='ytwLQd']");//05-06-2020 missing classic links
                                    if (nv != null)
                                        nv = nd.SelectSingleNode(".//h3[@class='r']/a|.//h3[@class='r']/div/a|.//h3[contains(@class,'yuRUbf JtG40d')]/a"); //03-11-2020  // 09-06-2020
                                }
                                //if (nv == null)//06-10-2022 commented //01-02-2022 
                                //  nv = nd.SelectSingleNode(".//a[contains(@class,'cz3goc BmP5tf')]");//10-06-2022 contains//01-02-2022
                                if (nv != null)
                                {
                                    string u = nv.Attributes["href"].Value;
                                    HtmlNode d = nv.SelectSingleNode(".//div[@role='heading']");
                                    if (d == null)
                                        d = nv.SelectSingleNode(".//div[@class='BNeawe vvjwJb AP7Wnd UwRFLe']"); //22-06-2020
                                    if (d == null)
                                        d = nd.SelectSingleNode(".//div[@role='heading']");//24-11-2022
                                    if (d == null)
                                        d = nd.SelectSingleNode(".//div[@class='bvTQqb']");//10-07-2021
                                    string t = "";
                                    if (d != null)
                                        t = d.InnerText;
                                    else
                                        t = nv.InnerText;

                                    //05-01-2023 adwords in middle page
                                    if (node.SelectSingleNode(".//div[@class='eMXfhf']") != null)
                                    {
                                        s.Append("<block type=\"adwords\" url=\"\">");
                                        u = SetUrl(u);
                                        if (!string.IsNullOrEmpty(u))
                                            s.Append("<item url=\"" + SetUrl(u) + "\"  title=\"" + SetTitle(t) + "\" />");
                                        s.Append("</block>");
                                    }//05-01-2023
                                    else if (orgLinks < 100)
                                    {
                                        u = SetUrl(u);
                                        if (u.StartsWith("http") || u.StartsWith("https") || u.StartsWith("ftp")) //30-04-2020
                                        {
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
        public string GetTopSights(HtmlNode node)//19-01-2023 new element top sights
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[@class='l10H9c']");
            foreach (HtmlNode nd in nds)
            {
                try
                {
                    string reviews = string.Empty;
                    string title = nd.SelectSingleNode(".//div[@class='MiF81b']/div")?.InnerText.Trim() ?? "";
                    string description = nd.SelectSingleNode(".//div[@class='UEiegc Y4Ql3c MUxGbd wuQ4Ob WZ8Tjf lEBKkf']")?.InnerText.Trim() ?? "";
                    string price = nd.SelectSingleNode(".//div[@class='UEiegc iiNTId MUxGbd lyLwlc aLF0Z OSrXXb']")?.InnerText.Trim() ?? "";
                    string rating = nd.SelectSingleNode(".//span[@class='LORxxe']/span")?.InnerText.Trim() ?? "";
                    var revs = nd.SelectNodes(".//span[@aria-hidden='true']");
                    if (revs != null)
                        foreach (var r in revs)
                        {
                            reviews = r.InnerText;
                            if (rating != r.InnerText)
                            {
                                reviews = r.InnerText;
                            }
                        }
                    string reviewNumbers = ConvertReviews(reviews);
                    if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(rating))
                        s.Append("<item url=\"\" description=\"" + SetTitle(description) + "\" price=\"" + SetTitle(price) + "\"  reviews=\"" + SetTitle(reviewNumbers) + "\" rating=\"" + SetTitle(rating) + "\"  title=\"" + SetTitle(title) + "\" />");

                }
                catch { }
            }
            return s.ToString();
        }//19-01-2023 new element top sights

        public string GetGoogleHotels(HtmlNode node) //19-01-2023 Google Hotels from KP block
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[@class='c94Vsf Y1mqLe kp-rgc']");
            if (nds != null)
                foreach (HtmlNode nd in nds)
                {
                    string title = nd.SelectSingleNode(".//h2[contains(@class, 'qrShPb')]/span")?.InnerText.Trim() ?? "";
                    string rating = nd.SelectSingleNode(".//span[contains(@class,'YrbPuc')]")?.InnerText.Trim() ?? "";
                    string reviews = nd.SelectSingleNode(".//span[@class='RDApEe YrbPuc']")?.InnerText.Replace("(", "").Replace(")", "").Trim() ?? "";
                    string reviewNumbers = ConvertReviews(reviews);
                    HtmlNode web = node.SelectSingleNode(".//div[@jsname='UXbvIb']/a[@class='FFdnyb']");//30-01-2023 for website in url attribute
                    string website = string.Empty;
                    if (website != null)
                    {
                        website = web.Attributes["href"]?.Value ?? "";
                    }//30-01-2023 for website in url attribute
                    s.Append("<item url=\"" + SetUrl(website) + "\" rating=\"" + SetTitle(rating) + "\" reviews=\"" + SetTitle(reviewNumbers) + "\" title=\"" + SetTitle(title) + "\" />");
                }
            nds = node.SelectNodes(".//div[@data-section-type='ads']/div");
            string prices = string.Empty;//24-01-2023
            if (nds != null)
            {
                foreach (HtmlNode nd in nds)
                {
                    string value = nd.SelectSingleNode(".//span[contains(@class,'r9dLMc')]")?.InnerText.Trim() ?? "";
                    string seller = nd.SelectSingleNode(".//div[@class='BWpDXc']/span")?.InnerText.Trim() ?? "";
                    if (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(seller))
                        prices += "<item url=\"\" seller=\"" + SetTitle(seller) + "\"  type=\"ads\" value=\"" + SetTitle(value) + "\" />";//24-01-2023
                }
            }
            nds = node.SelectNodes(".//div[@data-section-type='organic']/div");
            if (nds != null)
            {
                foreach (HtmlNode nd in nds)
                {
                    string value = nd.SelectSingleNode(".//span[@class='r9dLMc']")?.InnerText.Trim() ?? "";
                    string seller = nd.SelectSingleNode(".//div[@class='BWpDXc']")?.InnerText.Trim() ?? "";
                    if (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(seller))
                        prices += "<item url=\"\" seller=\"" + SetTitle(seller) + "\" type=\"organic\"  value=\"" + SetTitle(value) + "\"/>";//24-01-2023
                }
            }
            if (string.IsNullOrEmpty(prices)) return string.Empty;
            s.Append("<price>" + prices + "</price>");
            return s.ToString();
        } //19-01-2023 Google Hotels from KP block
        public string GetRefineBySearches(HtmlNode node)//22-11-2022 refine the searches
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nodes = node.SelectNodes(".//a[@jsname='vlj0of']");
            if (nodes != null)
            {
                foreach (HtmlNode nd in nodes)
                {
                    string url = nd.Attributes["href"]?.Value;
                    string title = nd.InnerText;
                    s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                }
            }
            return s.ToString();
        }//22-11-2022 refine the searches
        public string GetPopularProducts(HtmlNode node)//popular products//05-01-2023
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nodes = node.SelectNodes(".//ul/product-viewer-group/li|.//ul/div[@class='Ez5pwe']/li" +
              "|.//div[@class='MhZJBd']/div[@jsname='U5epZb']|.//div[@class='MhZJBd']/div/div[@jsname='U5epZb']" +
              "|.//ul[contains(@class,'gYe3nc LAEtub')]/li|.//ul[contains(@class,'lvS33d nodrpc')]/li");//16-11-2023//23-05-2023//08-05-2023
            if (nodes == null)
                nodes = node.SelectNodes(".//ul[contains(@class, 'sho-apgc__product-grid')]/li|.//ul/li[@class='J4ZKO wTrwWd']|.//ul[contains(@class,'sho-apgc__product-grid')]/div/li");//06-03-2023
            if (nodes != null)
            {
                foreach (HtmlNode nd in nodes)
                {
                    string url = string.Empty;
                    string title = string.Empty;
                    string price = string.Empty;
                    string name = string.Empty;
                    string price_value = "0";//28-11-2023
                    //HtmlNode link = nd.SelectSingleNode(".//a");
                    HtmlNode link = nd.SelectSingleNode(".//div[@class='NcgzWb Nplhsf']|.//div[@class='yDecDe w4sdvb']|.//g-inner-card[@class='ivG6i']/div|.//div[@class='NcgzWb']/div[@class='Nplhsf']|.//div[@class='UC8ZCe QS8Cxb']|.//div[contains(@class,'wTrwWd')]");//07-06-2023//25-04-2023//16-03-2023
                    if (link != null)
                    {
                        url = link.Attributes["href"]?.Value;
                        title = link.SelectSingleNode(".//div[contains(@class,'ZsI9Vc')]|.//div[@jsname='r4nke']|.//div[@class='aTc6pf']|.//div[@class='aqszKe']|.//div[contains(@class,'SsM98d')]|.//div[contains(@class,'gkQHve')]")?.InnerText ?? "";//09-08-2023//16-06-2023//27-02-2023//05-01-2023
                        price = link.SelectSingleNode(".//div[@class='Ijn7Rc']|.//div[@class='vy5bA dpJO9']|.//div[@class='uSZhvf Dxiee']/span[1]|.//div[@class='xQbyBc']/span|.//span[contains(@class,'lmQWe')]")?.InnerText ?? "";//06-03-2023//27-02-2023 //05-01-2023
                        name = link.SelectSingleNode(".//div[@class='DAB5ue']|.//div[@class='NemW5e']/span|.//div[contains(@class, 'ChC0jd')]/span|.//div[contains(@class,'kV5zMb')]/span[1]|.//span[@class='rw5ecc RmEs5b rOlovd']|.//span[contains(@class,'WJMUdc')]")?.InnerText ?? "";//07-12-2023//23-06-2023//09-05-2023//10-04-2023//16-03-2023 //27-02-2023//05-01-2023
                        if (price != "")//28-11-2023
                        {
                            price = ConvertNumber(price);
                            price_value = Convertprice(price);
                        }//28-11-2023
                    }
                    if (!string.IsNullOrEmpty(SetUrl(url)) || !string.IsNullOrEmpty(title))
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" price=\"" + SetTitle(price) + "\" priceValue=\"" + SetTitle(price_value) + "\" site=\"" + SetTitle(name) + "\" />");//28-11-2023
                }
            }
            return s.ToString();
        }//05-01-2023//22-11-2022 popular block
        //17-10-2019
        //17-10-2019
        public string SetVideos(HtmlNode nd)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode videos = nd.SelectSingleNode(".//div[@class='TyzpY']|.//div[@jsname='TFTr6']");//28-08-2023
            if (videos == null)
                videos = nd.SelectSingleNode(".//div[@class='SRYuRe']");
            if (videos == null)
                videos = nd.SelectSingleNode(".//div[@class='TvV1fe']");
            if (videos == null)
                videos = nd.SelectSingleNode(".//div[contains(@class,'B3JUpd')]"); //12-10-2021
            if (videos == null)
                videos = nd.SelectSingleNode(".//jsname[@class='ibnC6b']/a|.//div[@jsname='ibnC6b']/a");
            if (videos == null)
                videos = nd.SelectSingleNode(".//div[@class='BycXVc']/a");//12-10-2021 //11-10-2021
            if (videos != null)
            {
                s.Append("<block type=\"videos\" url=\"\">");
                //get video urls;
                s.Append(GetVideos(nd));
                s.Append("</block>");
            }
            return s.ToString();
        }

        public string ProcessBlock(HtmlNode node)
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
                case "hotel":
                    s.Append("<block type=\"hotelPack\" url=\"\">");//24-03-2022
                    s.Append(GetHotels(node));
                    s.Append("</block>");//24-03-2022
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
                    s.Append("<block type=\"productListedAds\" url=\"\">");
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
                case "jobs":  //20-11-2020
                    s.Append(GetJobs(node));
                    break;
                /*case "topsights": //23-03-2022
                    s.Append("<block type=\"topSights\" url=\"\">");
                    s.Append(GetTopSights(node));
                    s.Append("</block>");
                    break;*/
                /* case "flights":
                     s.Append("<block type=\"google_flights\" url=\"\">");
                     s.Append(GetFlights(node));
                     s.Append("</block>");//23-03-2022
                     break;*/
                /*case "refine": //refine the search 23-11-2022
                    s.Append("<block type=\"refineBySearches\" url=\"\">");
                    s.Append(GetRefineBySearches(node));
                    s.Append("</block>");
                    break;*/
                case "popular": //22-11-2022 popular block
                    s.Append("<block type=\"popularProducts\" url=\"\">");
                    s.Append(GetPopularProducts(node));
                    s.Append("</block>");
                    break;//22-11-2022
                case "findresultson":
                    //s.Append("<block type=\"findResultsOn\" url=\"\">");
                    s.Append(GetFindResultsOn(node));
                    //s.Append("</block>");
                    break;//09-08-2023//07-07-2023
                default:
                    break;
            }
            return s.ToString();

        }

        //20-11-2020
        public string GetJobs(HtmlNode node)
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
        }// end 20-11-2020

        // 18-10-2019
        public string GetCarousel(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            ArrayList al = new ArrayList();  //25-06-2020
            HtmlNodeCollection nd = node.SelectNodes(".//div[@jsmodel='uIhXXc']/div/g-scrolling-carousel/div/div/div/ul[@class='Kjd0sd']/div/div/g-inner-card/a");
            if (nd == null)//13-02-2024
                nd = node.SelectNodes(".//div[@class='GWRUbe LAZUFc']/span/g-link/a");//13-02-2024
            if (nd == null)
                nd = node.SelectNodes(".//div[@jsname='WUSFrc']/g-link/a|.//div[@jsname='WUSFrc']/div/g-link/a" +
                    "|.//div[@class='v1uiFd']/g-link/a|.//div[@class='bogMee']/g-link/a|.//g-inner-card[@class='VoEfsd']/g-link/a" +
                    "|.//div[@class='SuG7wd']/g-inner-card/g-link/a|.//div[contains(@class,'LAALze')]/g-inner-card/g-link/a" +
                    "|.//div[contains(@class,'LAALze')]/g-inner-card/div/g-link/a|.//div[@class='bogMee']/div/g-link/a");//30-01-2024//11-12-2023//01-02-2023//14-04-2022 ///23-11-2021/25-10-2021
            string url = "";
            if (nd != null)
            {
                foreach (HtmlNode nd1 in nd)
                {
                    url = nd1.Attributes["href"].Value;
                    HtmlNode hn = nd1.SelectSingleNode(".//div[@class='mB12kf JRhSae nDgy9d']");
                    if (hn == null)
                        //hn = nd1.SelectSingleNode(".//div[@class='hfac6d']"); //22-07-2020 commented
                        hn = nd1.SelectSingleNode(".//div[contains(@class,'hfac6d')]|.//span[contains(@class,'hfac6d')]");//11-12-2023 //22-07-2020 included selector for carousel title and applied contains class
                    //if (hn == null)
                    //    hn = nd1.SelectSingleNode(".//div[@class='hfac6d oz3cqf vH5Lmd']");  //22-07-2020 commented // 02-06-2020
                    if (hn == null)
                        hn = nd1.SelectSingleNode(".//div[contains(@class,'iORXPe')]");//23-05-2022//25-10-2021
                    string title = hn.InnerText;
                    al.Add("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");  //25-06-2020
                    //s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />"); //25-06-2020
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
                    nd = node.SelectNodes(".//div[@class='OixsOd']/a");//25-06-2020
                if (nd != null) //26-03-2021
                                //return string.Empty; //26-03-2021
                    foreach (HtmlNode nd1 in nd)
                    {
                        string title = ""; //25-06-2020
                        url = nd1.Attributes["href"].Value;
                        HtmlNode hn = nd1.SelectSingleNode(".//div[@class='oyj2db']"); //S20Xzc
                        if (hn == null)
                            hn = nd1.SelectSingleNode(".//div[@class='wfg6Pb']");    //21-05-2020
                        if (hn == null)
                            hn = nd1.SelectSingleNode(".//div[@class='S20Xzc']");    //23-05-2020
                        if (hn == null)
                            hn = nd1.SelectSingleNode(".//div[@class='JjtOHd Bgg9M']");  //23-05-2020 
                        if (hn == null)
                            hn = nd1.SelectSingleNode(".//div[@class='BNeawe vvjwJb AP7Wnd UwRFLe']"); //09-07-2020 carousel included selector

                        //25-06-2020
                        if (hn == null)
                            hn = nd1.SelectSingleNode(".//div[@class='pBi0X']");
                        if (hn != null)
                            title = hn.InnerText;

                        al.Add("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");

                        //string title1 = hn.InnerText; 
                        //s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title1) + "\" />");

                        //end 25-06-2020
                    }
            }
            if (nd == null) //25-03-2021
            {
                nd = node.SelectNodes(".//div[@class='Q9mvUc']");
                if (nd != null)
                {
                    foreach (HtmlNode nd1 in nd)
                    {
                        HtmlNode a = nd1.SelectSingleNode(".//a");
                        url = a.Attributes["href"].Value;
                        HtmlNode hn = nd1.SelectSingleNode(".//span[@class='rQMQod Xb5VRe']|.//span[@class='UMOHqf EDgFbc']"); //04-10-2021 missing carousel block
                        string title = hn.InnerText;
                        al.Add("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                    }
                }
            } //end 25-03-2021
            string caitems = GetCarouselURLs(al); //25-06-2020
            s.Append(caitems);
            return s.ToString();
        }

        // 18-10-2019
        public string GetCarouselURLs(ArrayList al) //25-06-2020
        {
            StringBuilder s = new StringBuilder();
            string matchPattern = "\\Wn,\\Wx222003\\Wx22:\\Wnull\\W\\Wx22(.*?)\\Wx22,\\Wx22(.*?)\\Wx22\\W\\Wx22(.*?)\\Wx22\\Wnull";
            try //30-06-2021
            {
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
                                al.Add("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(text) + "\" />"); //25-06-2020
                                                                                                                 //s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(text) + "\" />"); //25-06-2020
                            }
                        }
                    }
                }
            }
            catch { }//30-06-2021

            //25-06-2020
            foreach (string itm in al)
            {
                if (s.ToString().Contains(itm) || string.IsNullOrEmpty(itm)) continue;
                s.Append(itm);
            }
            //end 25-06-2020


            return s.ToString();
        }

        public string GetProductListedUrls(string url)
        {
            if (url.Contains("www") && url.Contains("http") != true)
            {
                url = "http://" + url;
            }
            else if (url.Contains("www") != true && url.Contains("http") != true)
            {
                url = "http://www." + url;
            }
            else if (url.Contains("www") != true && url.Contains("http")) //27-08-2021 removed !=true
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
        public string ProductListedAds(HtmlNode node)
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
                    //if (url.Contains("http"))
                    //{
                    //    url = nd1.Attributes["href"].Value;
                    //}
                    s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                }
            }

            return s.ToString();
        }//start 13-08-2019

        public string GetVideoCard(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();

            HtmlNode nd = node.SelectSingleNode(".//div[@class='_ELb']/a");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='twQ0Be']/a|.//div[@class='dbsr']/a");//08-01-2024
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='NFQFxe XbtRGb qxsd mod']/div/a"); //23-01-2020  // 21-02-2020 included selector for video cards block
            if (nd != null)
                s.Append("<block type=\"videoCard\" url=\"" + SetUrl(nd.Attributes["href"].Value) + "\"></block>");

            return s.ToString();
        }

        public string GetSiteLinks(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode n = node.SelectSingleNode(".//h3[@class='r']/a");
            if (n == null)
                n = node.SelectSingleNode(".//a[@class='C8nzq JTuIPc']");
            if (n == null)
                n = node.SelectSingleNode(".//a[contains(@class,'BmP5tf')]"); //14-09-2020 contains
            if (n == null)
                n = node.SelectSingleNode(".//a[contains(@class,'sXtWJb')]");//16-12-2020 //15-12-2020
            if (n != null)
            {
                if (orgLinks < 100)
                {
                    HtmlNode t = n.SelectSingleNode(".//div[@role='heading']");
                    if (t == null)
                        t = n.SelectSingleNode(".//span"); //15-12-2020
                    if (string.IsNullOrEmpty(t?.InnerText.Trim()))//13-07-2022
                    {
                        n = node.SelectSingleNode(".//div[@class='BmP5tf']/a");
                        t = n?.SelectSingleNode(".//div[@role='heading']");
                    }//end 13-07-2022
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
                nds = node.SelectNodes(".//div[contains(@class,'MUxGbd v0nnCb lyLwlc')]/a"); //21-07-2022
            if (nds == null)
                nds = node.SelectNodes(".//div[contains(@class,'MUxGbd lyLwlc')]/a"); //01-11-2022
            if (nds == null)//11-12-2023
                nds = node.SelectNodes(".//div[contains(@class, 'Va3FIb EE3Upf lVm3ye')]/a");//11-12-2023
            if (nds == null)//08-12-2023
                nds = node.SelectNodes(".//div[@class='E8hWLe SVMeif BmP5tf']/div/a");//08-12-2023
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

        public string GetKnowledgePanel(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode nd = node.SelectSingleNode(".//div[@class='kp-header']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kp-blk knowledge-panel OJXvsb']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kp-blk fm06If knowledge-panel OJXvsb']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kp-blk EyBRub knowledge-panel Wnoohf OJXvsb']"); //20-11-2020 KP block Selectors
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
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='MQv7ze']|.//div[@class='W2E8sb']|.//div[@class='grvXqb']");//27-02-2024//08-11-2023  // 23-06-2020
            if (nd != null)
            {
                string hdr = "";
                //string hdr = nd.SelectSingleNode(".//div[@role='heading']/div[1]/span").InnerText;
                HtmlNode hdrNode = nd.SelectSingleNode(".//div[@role='heading']/div[1]/span|.//div[@role='heading']"); //05-10-2020 KP block title included
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
                if (hdrNode == null)
                    hdrNode = node.SelectSingleNode(".//div[@class='MQv7ze']");  // 23-06-2020
                //if (hdrNode == null)
                //    hdrNode = node.SelectSingleNode(".//h2[@class='qrShPb kno-ecr-pt PZPZlf HOpgu gsmt mfMhoc']/span");  //21-07-2020 included selector for KP title
                if (hdrNode == null)
                    hdrNode = node.SelectSingleNode(".//h2[contains(@class, 'qrShPb')]/span"); //30-07-2020 included contains functions to KP title
                if (hdrNode == null)//08-11-2023
                    hdrNode = node.SelectSingleNode(".//div[@class='IDQilf MBeuO JIFdL']");//08-11-2023
                if (hdrNode != null)
                    hdr = hdrNode.InnerText;

                s.Append("<block type=\"knowledgeGraph\" url=\"\" title=\"" + SetTitle(hdr) + "\" />");
            }

            return s.ToString();
        }
        /*private string PeopleAlsoAsk(HtmlNode node)//commented 19-01-2022
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
               nds = node.SelectNodes(".//div[@jsname='bVEB4e']");//12-07-2020 //missing people also ask block for recipes keywords
           if (nds == null)
               nds = node.SelectNodes(".//div[@jsname='ARU61']"); // 14-12-2020
           if (nds == null)
               nds = node.SelectNodes(".//div[@jsname='lN6iy']"); //14-12-2021 tiles for Peope also ask block
           if (nds == null)
               return string.Empty;
           foreach (HtmlNode nd in nds)
           {
               s.Append("<item url=\"\" title=\"" + SetTitle(nd.InnerText) + "\" />");
           }
           return s.ToString();
       }*/
        public string PeopleAlsoAsk(HtmlNode node) //included item urls code 31-01-2022
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
                nds = node.SelectNodes(".//div[@jsname='bVEB4e']");//12-07-2020 //missing people also ask block for recipes keywords
            if (nds == null)
                nds = node.SelectNodes(".//div[@jsname='ARU61']"); // 14-12-2020
            if (nds == null)
                nds = node.SelectNodes(".//div[@jsname='lN6iy']"); //14-12-2021 tiles for Peope also ask block
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
        }//31-01-2022

        public string GetPeopleAlsoAskUrls(string[] titles) //People also method 31-01-2022 //06-06-2022
        {
            StringBuilder s = new StringBuilder();
            //TimeSpan ts = TimeSpan.FromMilliseconds(500);
            //string pattern = @"[WEB_ANSWERS_STANDARD_RESULT_|K3M0Td g1Khaf MUmB9 zbA8Me](.*?)div class\\x3d\\x22Xv4xee\\x22\\x3e\\x3ch3 class\\x3d\\x22yuRUbf JtG40d MBeuO q8U8x\\x22\\x3e\\x3ca class\\x3d\\x22sXtWJb\\x22 href\\x3d\\x22(.*?)\\x22";
            //string pattern = @"div class\\x3d\\x22Xv4xee\\x22\\x3e\\x3ch3 class\\x3d\\x22yuRUbf JtG40d MBeuO q8U8x\\x22\\x3e\\x3ca class\\x3d\\x22sXtWJb\\x22 href\\x3d\\x22(.*?)\\x22"; //09-06-2022
            string pattern = @"div class\\x3d\\x22Xv4xee\\x22\\x3e\\x3ch3 class\\x3d\\x22yuRUbf JtG40d MBeuO q8U8x\\x22\\x3e[ \\x3cdiv\\x3e]*\\x3ca class\\x3d\\x22sXtWJb\\x22 href\\x3d\\x22(.*?)\\x22"; //03-08-2023
            //Regex re = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline,ts);
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = re.Matches(html);
            ArrayList myList = new ArrayList();
            int x = 0;
            char[] yt = { '\\', '2', '6' };
            try
            {
                foreach (Match m in mc)
                {
                    //string url = HttpUtility.HtmlDecode(m.Groups[2].Value);
                    string url = HttpUtility.HtmlDecode(m.Groups[1].Value); //09-06-2022
                    if (url.StartsWith("http") || url.StartsWith("https"))
                    {
                        url = SetYTUrl(url, yt); //12-03-2022
                        if (x < titles.Length)//22-02-2022
                            s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(titles[x++]) + "\" />");//22-02-2022
                    }
                }
            }
            catch { }
            string pattern1 = @"\\x22\\x3e\\x3ca jsname\\x3d\\x22UWckNb\\x22 class\\x3d\\x22sXtWJb\\x22 href\\x3d\\x22(.*?)\\x22";//21-09-2023 //11-09-2023
            re = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            mc = re.Matches(html);
            try
            {
                foreach (Match m in mc)
                {
                    string url = HttpUtility.HtmlDecode(HttpUtility.HtmlEncode(m.Groups[1].Value));
                    if (url.StartsWith("http") || url.StartsWith("https"))
                    {
                        url = SetYTUrl(url, yt);
                        if (x < titles.Length)
                            s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(titles[x++]) + "\" />");
                    }
                }
            }
            catch { }//11-09-2023
            for (; x < titles.Length; x++)//18-02-2022
                s.Append("<item url=\"\" title=\"" + SetTitle(titles[x]) + "\" />");//18-02-2022
            return s.ToString();
        }

        /*public string GetAnswerCard(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//h3[@class='r']/a");
            if (nds == null)
                nds = node.SelectNodes(".//h3/a[contains(@class,'sXtWJb')]"); //17-01-2022 //05-10-2020 for answer card     //26-11-2019
            if (nds == null) //03-08-2023
                nds = node.SelectNodes(".//h3/div/a[contains(@class,'sXtWJb')]"); //03-08-2023
            if (nds == null) //11-09-2023
                nds = node.SelectNodes(".//h3/div/span/a[contains(@class,'sXtWJb')]"); //11-09-2023
            if (nds == null)//23-12-2021
                nds = node.SelectNodes(".//a[@class='GBgvb']|.//a[@class='B1uW2d ellip PZPZlf']");//20-09-2023//23-12-2021
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='WcS13d']/a|.//div[contains(@class, 'V3FYCf')]/a");//08-02-2024  //05-10-2020 included selector for missing classic links
            if (nds == null)
                return string.Empty;
            foreach (HtmlNode nd in nds)
            {
                s.Append("<item url=\"" + SetUrl(nd.Attributes["href"].Value) + "\" title=\"" + SetTitle(nd.InnerText) + "\" />");
            }
            return s.ToString();
        }*/
        public string GetAnswerCard(HtmlNode node)//07-03-2024
        {
            StringBuilder s = new StringBuilder();
            string desc = string.Empty;
            bool lst = false;
            bool tbl = node.SelectSingleNode(".//table") != null;
            bool chrt = node.SelectSingleNode(".//div[contains(@class, 'kpd-ch')]") != null;
            bool video = node.SelectSingleNode(".//span[@class='z1asCe UIgqBe']/svg") != null;
            string f_title = node.SelectSingleNode(".//h2/div[1]|.//div[@role='heading' and @aria-level='3']/div[contains(@class, 'JgzqYd')]|.//div[@class='iKJnec']")?.InnerText ?? "";
            HtmlNode a = node.SelectSingleNode(".//h3/a[contains(@class,'sXtWJb')]|.//h3/div/a[contains(@class,'sXtWJb')]|.//h3/div/span/a[contains(@class,'sXtWJb')]");
            string url = a?.Attributes["href"].Value ?? "";
            string title = a?.InnerText ?? "";
            HtmlNodeCollection ls = node.SelectNodes(".//ul/li|.//ol/li");
            if (ls == null)
            {
                HtmlNode list = node.SelectSingleNode(".//g-accordion");
                if (list != null)
                {
                    ls = list.SelectNodes(".//span[@class='s2ZLHc']");
                }
            }
            if (ls != null)
            {
                lst = true;
                foreach (var l in ls)
                {
                    desc += l.InnerText + "\\n";//12-03-2024
                }
                desc = desc.Remove(desc.Length - 1);
            }
            if (tbl)
            {
                HtmlNodeCollection tblRows = node.SelectNodes(".//table/tbody/tr");
                if (tblRows != null)
                {
                    desc = "";
                    foreach (HtmlNode r in tblRows)
                    {
                        HtmlNodeCollection th = r.SelectNodes(".//th");
                        if (th != null)
                        {
                            foreach (HtmlNode t in th)
                            {
                                desc += t.InnerText + "\\t";//12-03-2024
                            }
                        }
                        HtmlNodeCollection td = r.SelectNodes(".//td");
                        if (td != null)
                        {
                            foreach (HtmlNode t in td)
                            {
                                desc += t.InnerText + "\\t";//12-03-2024
                            }
                        }
                        desc = desc.Remove(desc.Length - 1) + "n";
                    }
                    //desc = desc.Remove(desc.Length - 1);//12-03-2024
                }
            }
            else
                desc = node.SelectSingleNode(".//span[contains(@class,'ILfuVd')]")?.InnerText ?? "";
            string cardType = lst ? "list" : tbl ? "table" : video ? "video" : chrt ? "chart" : "text";
            s.Append("<block type=\"answerCard\" url=\"" + SetUrl(url) + "\" >");
            s.Append("<item featureTitle=\"" + SetTitle(f_title) + "\" url=\"" + SetUrl(url) + "\" title=\"" +
                SetTitle(title) + "\" description=\"" + SetTitle(desc) + "\" cardType=\"" + cardType + "\" />");
            s.Append("</block>");
            return s.ToString();
        }//07-03-2024

        public string GetTwitterCards(HtmlNode node)
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
                    nds = node.SelectNodes(".//g-card-section[contains(@class,'jDsVJf')]/a"); //27-10-2021

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
        public string GetImages(HtmlNode node)
        {
            bool existed = false;
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[contains(@class,'eA0Zlc PZPZlf ITO9Cc ivg-i')]|.//div[@jsname='dTDiAc']");//12-08-2021 //09-08-2021
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='GNxIwf']/div[@jscontroller='xc1DSd']/div/a/g-inner-card/g-img[@class='BA0A6c']/img");  //30-10-2019
            if (nds == null) //|.//div[@class='eA0Zlc JX86yc ivg-i']/g-inner-card/g-img[@class='BA0A6c']/img //30-09-2020 removed
                nds = node.SelectNodes(".//div[@class='GNxIwf']/div[@jscontroller='xc1DSd']/a/g-inner-card/g-img[@class='BA0A6c']/img|.//div[contains(@class,'eA0Zlc')]/g-inner-card/g-img[@class='BA0A6c']/img");//30-09-2020 //05-06-2020 included selector for images //13-01-2020 included selector for images
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='eR2XS']/g-inner-card/div/a/g-img[@class='SeXxHf']/img"); //08-06-2020
            if (nds == null)
                nds = node.SelectNodes(".//div[@class='OixsOd']/a|.//div[contains(@class,'eA0Zlc')]/g-img[@class='BA0A6c']/img");//22-03-2021 changed //15-12-2020  //17-07-2020  //13-07-2020 selector included for images
            if (nds != null)
                foreach (HtmlNode nd in nds)
                {
                    //09-08-2021
                    if (nd.Attributes.Contains("data-lpage"))
                    {
                        string url = nd.Attributes["data-lpage"].Value.Trim();
                        if (url.StartsWith("//www.")) url = "http:" + url;
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"\" />");
                        existed = true;
                    }//end 09-08-2021
                    else if (nd.Attributes.Contains("data-src"))
                    {
                        string url = nd.Attributes["data-src"].Value.Trim();
                        if (url.StartsWith("//www.")) url = "http:" + url;
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"\" />");
                        existed = true;
                    }
                    //start 13-07-2020 condition item urls in images block
                    else if (nd.Attributes.Contains("href"))
                    {
                        string url = nd.Attributes["href"].Value.Trim();
                        if (url.StartsWith("//www.")) url = "http:" + url;
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"\" />");
                        existed = true;
                    }//end 13-07-2020
                }
            if (!existed)
            {
                string imgItems = GetImageURLs();
                s.Append(imgItems);
            }
            return s.ToString();
        }

        public string GetImageURLs()
        {
            StringBuilder s = new StringBuilder();

            string matchPattern1 = @"]n,\[x22(.*?)x22";
            string matchPattern2 = @"]\n,\[x22(.*?)\?";
            string matchPattern3 = "\"ou\":\"(.*?)\",";
            //string matchPattern4 = "\\W\\W\\Wx22http[s]*://(.*?)\\Wx22";   // 17-02-2020 included pattern
            string matchPattern4 = @"\[0,\\x22[\w-\d]*:\\x22,\[\\x22(.*?)\\x22,"; //18-02-2020 replaced pattern for above 17-02-2020
                                                                                  //string matchPattern5 = "<img data-src=\\W(.*?)(&amp;s)?\"\\s"; //06-11-2020 //24-06-2020
                                                                                  //string matchPattern5 = "\\d{3}px\\W><img data-src=\\W(.*?)(&amp;s)?\"\\s"; //13-11-2020 //06-11-2020 //24-06-2020 //02-06-2021
            string matchPattern5 = "\\d{2,3}[px|\\W]?\\W><img data-src=\\W(.*?)(&amp;s)?\"\\s"; //02-06-2021 new pattern
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
            //24-06-2020
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
            //end 24-06-2020

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


        public string GetTopStories(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//g-card-section/a");
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
                nds = node.SelectNodes(".//g-inner-card/div/a|.//div[@class='kno-fb-ctx n49mp']/div/a|.//div[@class='zZ9K7e']/a|.//div[contains(@class,'kno-fb-ctx')]/div/a|.//div[contains(@class,'kno-fb-ctx')]/div/div/a|.//g-inner-card/div/div/a");//10-08-2022 TS Item Urls
            if (nds == null)
                nds = node.SelectNodes(".//g-inner-card/div/div/a");    // 13-12-2019
            if (nds == null)
                nds = node.SelectNodes(".//lazy-load-item/div/a");
            if (nds == null)
                //nds = node.SelectNodes(".//div[@class='amp_re dbsr']/a|.//div[@class='dbsr']/a|.//div[@class='Fq8eSd']/a|.//div[@class='y1Boce']/div/a|.//div[@class='amp_re']/a");   // 18-12-2019//27-07-2020 commented
                nds = node.SelectNodes(".//div[contains(@class,'amp_re')]/a|.//div[@class='dbsr']/a|.//div[@class='Fq8eSd']/a|.//div[@class='y1Boce']/div/a|.//div[@data-ved]/a");//28-07-2020   // 18-12-2019//27-07-2020
            if (nds != null)
                foreach (HtmlNode nd in nds)
                {
                    //changes on 28-06-2019
                    HtmlNode title = nd.SelectSingleNode(".//div[@class='d4FON']"); // 03-06-2020 swapped from below line.
                    if (title == null)
                        title = nd.SelectSingleNode(".//div[@role='heading']");
                    if (title == null)//27-04-2023
                        title = nd.SelectSingleNode(".//div[contains(@class,'nDgy9d')]");//27-04-2023
                    if (title == null)
                        title = nd.SelectSingleNode(".//div[@class='nDgy9d']");   //changes on 05-07-2019
                    if (title == null)
                        title = nd.SelectSingleNode(".//div[contains(@class,'poMUXd')]"); //27-07-2020
                    if (title == null)
                        title = nd.SelectSingleNode(".//div[@class='mkVq5']");//27-11-2020 top stories titles
                    if (title == null)
                        title = nd.SelectSingleNode(".//div[contains(@class,'tNxQIb ynAwRc nDgy9d')]|.//div[contains(@class,'ynAwRc tNxQIb nDgy9d')]");//08-05-2023//01-02-2023//11-01-2023//11-01-2023
                    if (title == null)//18-08-2023
                        title = nd.SelectSingleNode(".//div[contains(@class,'eAaXgc aKoISd')]");//18-08-2023
                    if (title == null)//20-09-2023
                        title = nd.SelectSingleNode(".//div[contains(@class,'eAaXgc WZaQJf')]");//20-09-2023
                    if (title == null)
                        title = nd.SelectSingleNode(".//div[contains(@class,'eAaXgc cPUhZb')]");//10-10-2023
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
                        if (nd.SelectSingleNode(".//div[@class='poMUXd']") != null || nd.SelectSingleNode(".//div[@class='mCBkyc nDgy9d']") != null
                           || nd.SelectSingleNode(".//div[@class='poMUXd oz3cqf vH5Lmd']") != null || nd.SelectSingleNode(".//div[contains(@class,'mCBkyc')]") != null//19-04-2023
                            || nd.SelectSingleNode(".//div[@class='mCBkyc tNxQIb ynAwRc nDgy9d']|.//div[contains(@class,'ynAwRc tNxQIb nDgy9d')]") != null)//08-05-2023//30-05-2022 //10-06-2020 //04-06-2020  //14-05-2020
                        {
                            HtmlNode title = nd.SelectSingleNode(".//div[@class='poMUXd']");
                            if (title == null)
                                title = nd.SelectSingleNode(".//div[@class='mCBkyc nDgy9d']");    //14-05-2020
                            if (title == null)
                                title = nd.SelectSingleNode(".//div[@class='poMUXd oz3cqf vH5Lmd']");//04-06-2020
                            if (title == null)
                                title = nd.SelectSingleNode(".//div[contains(@class,'mCBkyc')]"); //19-04-2023//10-06-2020//10-06-2020
                            if (title == null)
                                title = nd.SelectSingleNode(".//div[contains(@class,'tNxQIb ynAwRc nDgy9d')]");//01-02-2023//11-01-2023//30-05-2022
                            string url = nd.Attributes["href"].Value;
                            s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title.InnerText) + "\" />");
                        }
                    }
                }
            }

            return s.ToString();
        }

        public string GetVideos(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//g-inner-card/a|.//div[@class='QvOPBf']");//05-03-2024
            if (nds == null)
                nds = node.SelectNodes(".//div[@data-attrid='OsrpVideos']/a|.//div[@class='ALzVK']/div/div/a");
            if (nds == null)
                nds = node.SelectNodes(".//div[@jsname='ibnC6b']/a|.//div[@jscontroller='OmmTPc']" +
                  "|.//div[@class='QevLbc']/a|.//a[@class='OIDuRe']|.//div[@class='Q9mvUc']" +
                  "|.//div[@class='KJDcUb']|.//a[@class='ygih0']|.//div[@class='NBoMDb']/a" +
                  "|.//a[@class='dyWXTb']|.//div[@class='MCDgie']/a[@class='BG7Pyb']" +
                  "|.//div[@class='GFu7kc']/a|.//div[@class='iyrWae']/a");//26-02-2024//21-11-2023//24-08-2023 //17-05-2022 //07-04-2022 videos item urls
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
                            n = nd.SelectSingleNode(".//div[@class='oyj2db']|.//span[@class='rQMQod Xb5VRe']");//30-07-2020
                        if (n == null)//24-08-2023
                            n = nd.SelectSingleNode(".//div[@class='nOmCmb']");//24-08-2023
                        if (n == null)
                            n = nd.SelectSingleNode(".//div[@class='k8M7xd OSrXXb tNxQIb ynAwRc']");//14-08-2023
                        if (n == null)//21-11-2023
                            n = nd.SelectSingleNode(".//div[@class='ZhosBf MBI8Pd PwQv4e']");//21-11-2023
                        try
                        {
                            // Changes in Videos block on 25-06-2019
                            HtmlNode t = nd.SelectSingleNode(".//div[@role='heading']|.//div[@class='WDJH5']");//30-09-2022
                            if (n != null)
                                title = n.InnerText;//05-03-2024
                            else
                                title = t.InnerText;//05-03-2024
                        }
                        catch
                        {
                            HtmlNode t = nd.SelectSingleNode(".//div[contains(@class,'fJiQld')]");//30-11-2020 videos title selector //26-06-2020
                            if (t != null)
                                title = t.InnerText;
                            // end of changes in videos.
                        }

                        //26-06-2020
                        string url = ""; //nd.Attributes["href"].Value.Trim();                         
                        if (nd.Attributes["href"] != null)
                            url = nd.Attributes["href"].Value.Trim();
                        else if (nd.SelectSingleNode(".//a") != null) // 14-12-2020
                            url = nd.SelectSingleNode(".//a").Attributes["href"].Value; // 14-12-2020
                        else
                            url = nd.Attributes["data-url"].Value;
                        //end 26-06-2020
                        if (url.Contains("/search?") || url.StartsWith("#")) url = "";//11-04-2022
                        string creator = nd.SelectSingleNode(".//div[@class='YDQ8Te Fy8FNd OSrXXb']/span[3]")?.InnerText ?? "";//04-12-2023
                        if (!string.IsNullOrEmpty(url.Trim()) || !string.IsNullOrEmpty(title.Trim())) //11-04-2022
                             s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                            //s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" creatorName=\"" + SetTitle(creator) + "\" />");//04-12-2023
                    }
                    catch { }
                }
            else
            {
                // 16-12-2019
                nds = node.SelectNodes(".//div[@jscontroller='OmmTPc']|.//div[@jscontroller='zIwOx']"); //23-03-2021
                if (nds != null)
                {
                    foreach (HtmlNode nd in nds)
                    {
                        string url = nd.Attributes["data-surl"].Value;
                        if (string.IsNullOrEmpty(url)) //23-02-2021
                            url = nd.Attributes["data-url"]?.Value; //23-02-2021
                        //string title = nd.SelectSingleNode(".//div[@class='fJiQld']|.//div[@class='fJiQld oz3cqf vH5Lmd']").InnerText;//12-06-2020
                        string title = nd.SelectSingleNode(".//div[contains(@class,'fJiQld')]").InnerText; //end 23-03-2021
                        if (url.Contains("/search?")) url = "";
                        s.Append("<item url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                    }
                }
                // 16-12-2019
            }

            return s.ToString();
        }

        // changes on 15-07-2019
        public string GetApps(HtmlNode node)
        {
            StringBuilder s = new StringBuilder();
            HtmlNode App = node.SelectSingleNode(".//div[@class='qs-io aig-lst']"); //d5oMvf
            if (App != null)
            {
                //HtmlNode App1 = App.SelectSingleNode(".//h3[@class='header-title yovt']");
                s.Append("<block type=\"apps\" url=\"\">");

                //06-08-2019
                HtmlNodeCollection App2 = App.SelectNodes(".//div[@class='aig-i aew2ab']");
                if (App2 == null)
                    App2 = App.SelectNodes(".//div[@class='qiL93c']");//18-07-2020
                if (App2 == null)
                    App2 = App.SelectNodes(".//div[@class='EzLsDb']"); //24-03-2021 missing apps url
                if (App2 == null) //25-01-2023
                    App2 = App.SelectNodes(".//ul[@class='ZEz0bc']/li[@class='pOWtie']");//25-01-2023
                if (App2 != null)
                {
                    foreach (HtmlNode nd in App2)
                    {
                        HtmlNode App3 = nd.ChildNodes[0];
                        //31-08-2020
                        HtmlNode titleNode = nd.SelectSingleNode(".//div[@class='dlErff']|.//div[@class='SZCqL tNxQIb OSrXXb']");//25-01-2023
                        string title;
                        if (titleNode != null)
                            title = titleNode.InnerText.Trim();
                        else
                            title = nd.InnerText.TrimStart();

                        s.Append("<item url=\"" + SetUrl(App3.Attributes["href"].Value) + "\" title=\"" + SetTitle(title) + "\" />");
                        //end 31-08-2020

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
                    HtmlNode TextNode = App.SelectSingleNode(".//div[contains(@class,'mdKzW')]");//12-11-2021 app block
                    if (TextNode != null)
                        innertext = TextNode.InnerText;
                    else //27-07-2022
                        innertext = App.InnerText; //27-07-2022

                    s.Append("<item url=\"" + SetUrl(App.Attributes["href"].Value) + "\" title=\"" + SetTitle(innertext.TrimStart()) + "\" />");

                    s.Append("</block>");
                }
            } // end of 23-01-2020  // 21-02-2020
            return s.ToString();
        }
        public string GetFlights(HtmlNode node) //06-07-2023
        {
            StringBuilder s = new StringBuilder();
            string destination = string.Empty;
            string origin = string.Empty;
            HtmlNode dest = node.SelectSingleNode(".//div[@jsname='GseVJb']");
            if (dest != null && (dest.InnerText.Contains("destination") || dest.InnerText.Contains("Ziel") || dest.InnerText.Contains("destinazione") || dest.Attributes["aria-label"].Value.Contains("Ziel") || dest.Attributes["aria-label"].Value.Contains("destin")))//22-09-2023//24-08-2023//13-07-2023
            {
                var spn = node.SelectSingleNode(".//div[@jsname='S9WWYc']").SelectNodes(".//span[not(contains(@aria-hidden, 'false'))]");
                foreach (var sp in spn)
                {
                    origin += sp.InnerText + " ";
                }
                origin = origin.Trim();
                spn = dest.SelectNodes(".//span[not(contains(@aria-hidden, 'false'))]");
                foreach (var sp in spn)
                {
                    destination += sp.InnerText + " ";
                }
                destination = destination.Trim();
            }
            else //09-08-2023
            {
                try
                {
                    dest = node.SelectSingleNode(".//span[@class='XaP5ee IFnjPb RES9jf']|.//span[@class='mgAbYb OSrXXb RES9jf IFnjPb']|.//h3[@class='IFnjPb RES9jf']");//02-01-2023
                    int lenIndex = dest.InnerText.IndexOf(" to ") >= 0 ? dest.InnerText.IndexOf(" to ") :
                        dest.InnerText.IndexOf(" nach ") >= 0 ? dest.InnerText.IndexOf(" nach ") : -1;
                    if (lenIndex == -1)
                        lenIndex = dest.InnerText.IndexOf(" from ") >= 0 ? dest.InnerText.IndexOf(" from ") : -1;
                    origin = dest?.InnerText?.Substring(0, lenIndex).Trim(); // dest.InnerText.IndexOf(" to "));
                    if (origin.ToLower().Equals("flights") || origin.Equals("Flüge")) origin = string.Empty;
                    int len = dest.InnerText.IndexOf(" to ") >= 0 ? dest.InnerText.IndexOf(" to ") + 4 :
                        dest.InnerText.IndexOf(" nach ") >= 0 ? dest.InnerText.IndexOf(" nach ") + 6 : -1;
                    if (len == -1)
                    {
                        len = dest.InnerText.IndexOf(" from ") >= 0 ? dest.InnerText.IndexOf(" from ") + 6 : -1;
                        origin = dest?.InnerText.Substring(len).Trim();
                    }
                    else
                        destination = dest?.InnerText.Substring(len).Trim(); // dest.InnerText.IndexOf(" to ") + 4).Trim();
                }
                catch { }
            }
            s.Append("<block type=\"flightPack\" url=\"\" title=\"\" origin=\"" + SetTitle(origin) + "\" destination=\"" + SetTitle(destination) + "\" >");
            HtmlNodeCollection nds = node.SelectNodes(".//div[@class='aieQre']/div/a|.//div[contains(@class,'LQQ1Bd')]/div/a|.//div[@class='qR29te']/div/a");//02-01-2024
            if (nds != null)
            {
                foreach (HtmlNode nd in nds)
                {
                    try
                    {
                        //02-01-2024 start changes
                        string airline = nd.SelectSingleNode(".//span[@class='ps0VMc']|.//div[@class='A4fsl']|.//div[@class='eqdsgd']|.//div[@class='ZhosBf MBI8Pd dctkEf']")?.InnerText.Trim() ?? "";
                        string hours = nd.SelectSingleNode(".//span[@class='sRcB8']|.//div[@class='QTPlac']/span[2]")?.InnerText.Trim(); // ?? "0h 0m";
                        if (string.IsNullOrEmpty(hours))
                        {
                            hours = nd.SelectSingleNode(".//div[@class='oYQBg v5K7qb ApHyTb cHaqb']")?.InnerText;
                            if (!string.IsNullOrEmpty(hours))
                            {
                                int indx = hours.IndexOf("&middot;");
                                hours = hours.Substring(indx + "&middot;".Length);
                            }
                            if (string.IsNullOrEmpty(hours))
                                hours = "0h 0m";
                        }
                        string connecting = nd.SelectSingleNode(".//span[@class='u85UCd']|.//div[@class='QTPlac']/span[1]")?.InnerText.Trim(); // ?? "";
                        if (string.IsNullOrEmpty(connecting))
                        {
                            connecting = nd.SelectSingleNode(".//div[@class='oYQBg v5K7qb ApHyTb cHaqb']")?.InnerText;
                            if (!string.IsNullOrEmpty(connecting))
                            {
                                int indx = connecting.IndexOf("&middot;");
                                connecting = connecting.Substring(0, indx);
                            }
                            if (string.IsNullOrEmpty(connecting))
                                connecting = "";
                        }
                        string price = nd.SelectSingleNode(".//span[@class='xqqLDd']|.//span[@class='cirEce']|.//div[@class='n22NNe']|.//div[@class='rZFLMc']")?.InnerText.Trim() ?? "0";
                        //02-01-2024 end
                        string priceValue = string.Empty;
                        string hoursValue = string.Empty;
                        string title = string.Empty;
                        if (node.SelectSingleNode(".//div[@class='UgpQWe']|.//div[@class='xwYPZe']") == null)//24-08-2023
                        {
                            title = airline;
                            airline = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(hours))
                        {
                            hoursValue = ConvertHours(hours);
                        }
                        if (!string.IsNullOrEmpty(price))
                        {
                            priceValue = Convertprice(price);
                        }
                        s.Append("<item url=\"\" airline=\"" + SetTitle(airline) + "\" title=\"" + SetTitle(title) + "\" duration=\"" + SetTitle(hours) + "\" durationValue=\"" + hoursValue + "\" connections=\"" + SetTitle(connecting) + "\" price=\"" + price + "\" priceValue=\"" + priceValue + "\" />");
                    }
                    catch { }
                }
            }
            else
            {
                nds = node.SelectNodes(".//table[contains(@class, 'YFsOAd')]/tbody/tr");//22-09-2023
                if (nds != null)
                {
                    foreach (var nd in nds)
                    {
                        try
                        {
                            string airline = "";
                            string hours = nd.SelectSingleNode(".//div[@class='BNeawe DwrKqd']/span")?.InnerText.Trim() ?? nd.SelectSingleNode(".//td[1]/div/div[contains(@class,'BNeawe')]/span[2]")?.InnerText.Trim() ?? "0h 0m"; //27-09-2023 //22-09-2023
                            string connecting = nd.SelectSingleNode(".//div[contains(@class,'BNeawe')]/span[@class='BNeawe']")?.InnerText.Trim() ?? nd.SelectSingleNode(".//div[@class='BNeawe']")?.InnerText.Trim() ?? "";//22-09-2023
                            string price = nd.SelectSingleNode(".//div[@class='BNeawe DwrKqd']")?.GetDirectInnerText() ?? nd.SelectSingleNode(".//td[2]/div/div[@class='BNeawe']")?.GetDirectInnerText() ?? "0";//22-09-2023
                            string priceValue = string.Empty;
                            string hoursValue = string.Empty;
                            if (!string.IsNullOrEmpty(hours))
                            {
                                hoursValue = ConvertHours(hours);
                            }
                            if (!string.IsNullOrEmpty(price))
                            {
                                priceValue = Convertprice(price);
                            }
                            s.Append("<item url=\"\" title=\"\" airline=\"" + SetTitle(airline) + "\" duration=\"" + SetTitle(hours) + "\" durationValue=\"" + hoursValue + "\" connections=\"" + SetTitle(connecting) + "\" price=\"" + price + "\" priceValue=\"" + priceValue + "\" />");
                        }
                        catch { }
                    }
                }
            }
            s.Append("</block>");
            return s.ToString();
        }//06-07-2023//03-07-2023//05-06-2023
        public string GetHotels(HtmlNode node)//24-03-2022 new element Maps
        {
            StringBuilder s = new StringBuilder();
            HtmlNodeCollection nds = node.SelectNodes(".//div[contains(@class,'M0T4Vc')]");//17-05-2023
            if (nds == null)//17-05-2023
                nds = node.SelectNodes(".//div[@class='eIE7ad']");//17-05-2023
            if (nds == null)//17-05-2023
                nds = node.SelectNodes(".//div[@class='Vzzxlb']");//17-05-2023
            if (nds == null)//29-06-2023
                nds = node.SelectNodes(".//div[@class='bwQnad cH66Ue']");//29-06-2023
            if (nds == null)//20-02-2024
                nds = node.SelectNodes(".//div[contains(@class,'EXH1Ce')]");//27-02-2024//20-02-2024
            if (nds != null)
            {
                foreach (HtmlNode nd in nds)
                {
                    try
                    {
                        string price_value = string.Empty;
                        string additional_info = string.Empty;
                        string title = nd.SelectSingleNode(".//div[contains(@class,'BTPx6e')]|.//div[@class='Yt787 aKoISd']|.//span[@class='Yt787']")?.InnerText.Trim() ?? "";//20-02-2024 //29-06-2023//01-03-2023
                        string rating = nd.SelectSingleNode(".//span[contains(@class,'YrbPuc')]")?.InnerText.Trim() ?? "";
                        string reviews = nd.SelectSingleNode(".//span[@class='RDApEe YrbPuc']")?.InnerText.Trim() ?? "";
                        string price = nd.SelectSingleNode(".//div[contains(@class,'VSZCrf')]/span|.//div[@class='QIeQge']|.//div[@class='Z6yUYe RiJqbb RES9jf']|.//div[@class='rDUZLd JNI6Yb']/span|.//div[@class='wHYlTd RES9jf OSrXXb']/span")?.InnerText.Trim() ?? "";//27-02-2024//20-02-2024//29-06-2023//02-03-2023//22-02-2023
                        if (price != "")
                        {
                            price_value = Convertprice(price);
                        }
                        var desc = nd.SelectNodes(".//div[@class='I9B2He']|.//div[contains(@class,'dLtZ8b')]|.//div[@class='ZIFkhf ApHyTb']|.//div[@class='COW5R']|.//div[contains(@class,'mMeJe')]");//15-03-2023//01-03-2023//27-02-2023//22-02-2023
                        if (desc != null)
                            foreach (var d in desc)
                            {
                                additional_info += d.InnerText + ",";
                            }
                        string reviewNumbers = ConvertReviews(reviews);
                        if (string.IsNullOrEmpty(reviewNumbers) && string.IsNullOrEmpty(rating) && string.IsNullOrEmpty(price))
                        {
                            if (!string.IsNullOrEmpty(additional_info)) additional_info = additional_info.Remove(additional_info.Length - 1);//15-05-2023
                            s.Append("<item url=\"\" additionalInfo=\"" + SetTitle(additional_info) + "\" title=\"" + SetTitle(title) + "\" />");
                        }
                        else if (string.IsNullOrEmpty(price))
                        {
                            if (!string.IsNullOrEmpty(additional_info)) additional_info = additional_info.Remove(additional_info.Length - 1);
                            s.Append("<item url=\"\" rating=\"" + SetTitle(rating.Replace(",", ".")) + "\" totalReviews=\"" + SetTitle(reviewNumbers) + "\" additionalInfo=\"" + SetTitle(additional_info) + "\" title=\"" + SetTitle(title) + "\" />");
                        }
                        else if (string.IsNullOrEmpty(reviewNumbers) || string.IsNullOrEmpty(rating))
                        {
                            if (!string.IsNullOrEmpty(additional_info)) additional_info = additional_info.Remove(additional_info.Length - 1);//15-05-2023
                            s.Append("<item url=\"\" price=\"" + SetTitle(price) + "\" priceValue=\"" + SetTitle(price_value) + "\" additionalInfo=\"" + SetTitle(additional_info) + "\" title=\"" + SetTitle(title) + "\" />");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(additional_info)) additional_info = additional_info.Remove(additional_info.Length - 1);
                            s.Append("<item url=\"\" price=\"" + SetTitle(price) + "\" priceValue=\"" + SetTitle(price_value) + "\" rating=\"" + SetTitle(rating.Replace(",", ".")) + "\" totalReviews=\"" + SetTitle(reviewNumbers) + "\" additionalInfo=\"" + SetTitle(additional_info) + "\" title=\"" + SetTitle(title) + "\" />");
                        }
                    }
                    catch { }
                }
            }
            return s.ToString();
        }//24-03-2022
        public string GetFindResultsOn(HtmlNode node)//09-08-2023//07-07-2023 FindResultsOn Block
        {
            StringBuilder s = new StringBuilder();
            s.Append("<block type=\"findResultsOn\" url=\"\">");
            HtmlNodeCollection nds = node.SelectNodes(".//div/a[@class='dVjlWe']|.//div/a[@class='t2Yvdb']|.//div/a[@class='SsH3c']");
            if (nds != null)
            {
                foreach (var nd in nds)
                {
                    string url = nd.Attributes["href"].Value;
                    string source = nd.SelectSingleNode(".//span[@class='dsJOWd']|.//span[@class='izosSe']|.//span[@class='UhM1oe']")?.InnerText ?? "";
                    string title = nd.SelectSingleNode(".//div[@class='NNFu9b nDgy9d']")?.InnerText ?? "";
                    s.Append("<item source=\"" + SetTitle(source) + "\" url=\"" + SetUrl(url) + "\" title=\"" + SetTitle(title) + "\" />");
                }
            }
            s.Append("</block>");
            if (node.SelectSingleNode(".//div[contains(@class,'WlTAzf')]|.//div[@class='tkQJMd']") != null || node.Attributes["class"]?.Value == "WlTAzf mnr-c vk_c" || node.Attributes["class"]?.Value == "WlTAzf Ww4FFb vt6azd vk_c")//04-10-2023
            {
                s.Append(GetFlights(node));
            }
            return s.ToString();
        }//09-08-2023//07-07-2023 FindResultsOn Block
        public string ConvertReviews(string reviews)//20-01-2023 display only numbers
        {
            if (string.IsNullOrEmpty(reviews)) return null;//26-06-2023
            Regex rx = new Regex("\\.\\d*K");
            if (rx.IsMatch(reviews))
                reviews = Regex.Replace(reviews, "[^0-9K]", "").Replace("K", "00");
            else
                reviews = Regex.Replace(reviews, "[^0-9K]", "").Replace("K", "000");
            return reviews;
        }//20-01-2023 display only numbers
        public string ConvertNumber(string value)//23-06-2023
        {
            if (string.IsNullOrEmpty(value))
                return null;
            StringBuilder sb = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                double d = char.GetNumericValue(c);
                if (d < 0 || d % 1 != 0)
                    sb.Append(c);
                else
                    sb.Append((int)d);
            }
            return sb.ToString();
        }//23-06-2023
        public string ConvertCurrency(string value, int seid)//23-06-2023
        {
            var val = ConvertNumber(value);
            if (val == null) return null;
            var res = Convertprice(val);
            var locale = SearchParams.searches.FirstOrDefault(l => l.seid == seid).locale;
            var cs = new RegionInfo(locale).ISOCurrencySymbol;
            return string.Join(" ", cs, res);
        }//23-06-2023
        public string Convertprice(string price)
        {
            string patternprice = "[\\d]+";
            string p = price.Contains("€") ? price.Replace(" ", "").Replace(".", "") : price.Replace(",", "").Replace("٬", "");//01-01-2024
            Match mc = Regex.Match(p, patternprice, RegexOptions.IgnoreCase);
            if (mc.Success)
                price = mc.Value;
            if (price.Equals("unknown"))
                price = "0";
            if (price.Equals("check price"))//01-06-2023
                price = "0";//01-06-2023
            if (price.Equals("vérifier le prix"))//01-06-2023
                price = "0";//01-06-2023
            if (price.Equals("Preis prüfen"))
                price = "0";
            if (price.Equals("controlla il prezzo"))
                price = "0";
            if (price.Equals("Consulta el precio"))//26-02-2024
                price = "0";
            if (price.Equals("Consulta el precio."))
                price = "0";
            if (price.Equals("–"))
                price = "0";
            if (price.Equals("Free"))
                price = "0";
            if (price.Equals("ฟรี"))
                price = "0";
            if (price.Equals("Gratis"))
                price = "0";
            if (price.Equals("免費"))
                price = "0";//26-02-2024
            return price;
        }
        public string ConvertHours(string hours) //05-07-2023
        {
            Match match = Regex.Match(hours, @"(\d+)[\s]?[d|T][\W]* (\d+)[\s]?(h|Std)[\W]* (\d+)[\s]?(m|[M|m]in)");
            if (match.Success)
            {
                int days = Convert.ToInt32(match.Groups[1].Value);
                int hrs = Convert.ToInt32(match.Groups[2].Value);
                int min = Convert.ToInt32(match.Groups[4].Value);
                return (days > 0 || hrs > 0 || min > 0) ? ((days * 24) + hrs + (min / 60.0)).ToString("##.##") : "0.0";
            }
            match = Regex.Match(hours, @"(\d+)[\s]?[d|T][\W]* (\d+)[\s]?(h|Std\.)");
            if (match.Success)
            {
                int days = Convert.ToInt32(match.Groups[1].Value);
                int hrs = Convert.ToInt32(match.Groups[2].Value);
                return (days > 0 || hrs > 0) ? ((days * 24) + hrs) + "." + "0" : "0.0";
            }
            match = Regex.Match(hours, @"(\d+)[\s]?(h|Std)[\W]* (\d+)[\s]?(m|[M|m]in)");
            if (match.Success)
            {
                int hrs = Convert.ToInt32(match.Groups[1].Value);
                int min = Convert.ToInt32(match.Groups[3].Value);
                return (hrs > 0 || min > 0) ? (hrs + (min / 60.0)).ToString("##.##") : "0.0";
            }
            match = Regex.Match(hours, @"(\d+)[\s]?[d|T]");
            if (match.Success)
            {
                int days = Convert.ToInt32(match.Groups[1].Value);
                return (days > 0) ? (days * 24).ToString("##.##") : "0.0";
            }
            match = Regex.Match(hours, @"(\d+)[\s]?(h|Std)");
            if (match.Success)
            {
                int hrs = Convert.ToInt32(match.Groups[1].Value);
                return (hrs > 0) ? hrs.ToString("##.##") : "0.0";
            }
            match = Regex.Match(hours, @"(\d+)[\s]?(m|[M|m]in)");
            if (match.Success)
            {
                int min = Convert.ToInt32(match.Groups[1].Value);
                return (min > 0) ? (min / 60.0).ToString("##.##") : "0.0";
            }
            match = Regex.Match(hours, @"(\d+)");//23-09-2023
            if (match.Success)
            {
                int min = Convert.ToInt32(match.Groups[1].Value);
                return (min > 0) ? (min / 60.0).ToString("##.##") : "0.0";
            }//23-09-2023
            return "0.0";
        }//05-07-2023
        public string GetBlockType(HtmlNode node)
        {
            HtmlNode nd = node.SelectSingleNode(".//div[@class='KNcnob']/g-img");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='fn6bCb']|.//g-tray-header[@class='kno-fb-ctx zbA8Me ndEm3b']|.//div[@class='fhQnRd']"); //12-12-2020 applied selector for TS BT
            if (nd == null)
                nd = node.SelectSingleNode(".//g-tray-header[contains(@class,'kno-fb-ctx')]");//27-07-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='qDSRad']");  // changes on 05-07-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[contains(@class,'xSoq1')]");//08-01-2021 top stories //19-06-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='wtFsOb']"); //07-08-2021 missing top stories
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='CXo9G']"); //08-01-2021
            if (nd == null)
                nd = node.SelectSingleNode(".//g-card[@class='I7zR5']");//21-09-2022 top stories
            if (nd != null && node.SelectSingleNode(".//div[contains(@class,'knowledge-panel')]") == null) //26-08-2020 included KP selector
            {
                bool ts = true;
                //start07-08-2020 //map selector
                if (node.SelectSingleNode(".//img[@alt='Affected area']") != null)
                    ts = false;
                // end07-08-2020
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
                    n = node.SelectSingleNode(".//div[@role='heading']|.//g-tray-header[@role='heading']|.//g-inner-card[contains(@class,'kno-fb-ctx')]");//06-01-2021//21-12-2020 top stories only
                    if (n != null)
                    {
                        if (node.SelectNodes(".//div[@class='OSrXXb ZE0LJd']") == null && (n.InnerText.ToLower().Trim() == "videos" || n.InnerText.ToLower().Trim().StartsWith("video") || n.InnerText.Trim() == "فيديوهات" || n.InnerText.Trim() == "วิดีโอ" || n.InnerText.Trim() == "影片" || n.InnerText.Trim() == "Vidéos"))//06-10-2022
                            return "Videos";
                        if (n.InnerText.ToLower().Trim() == "recipes" || n.InnerText.ToLower().Trim() == "ricette")  // 20-03-2020 // 18-12-2019
                            return "Carousel";
                        if (n.InnerText.ToLower().Trim().Contains("top sights") || n.InnerText.ToLower().Trim().Contains("popular trip")) //19-09-2020 does not picks this blocks
                            ts = false;
                        if (node.SelectSingleNode(".//div[@class='YoZiHf']") != null) //08-01-2021 for not top sights
                            ts = false;
                        if (node.SelectSingleNode(".//div[contains(@class,'h7uAbe')]") != null)//26-04-2023 //27-02-2023
                            ts = false;
                    }
                }
                if (ts)
                    return "Topstories";
            }
            /*  if (node.SelectSingleNode(".//div[contains(@class, 'RPdfze')]") != null || (node.SelectSingleNode(".//div[contains(@class, 'Qkn3ie')]" +
                "|.//div[@class='lMMUFc']") != null && node.SelectSingleNode(".//div[contains(@class,'WlTAzf')]" +
                "|.//div[contains(@class,'WFxqwc')]|.//div[contains(@class,'PZPZlf')]" +
                "|.//div[@class='KmNjGe Iu7yDc']") == null))//09-01-2024//08-01-2024//15-12-2023//28-11-2023//09-10-2023//22-09-2023//21-07-2023//07-07-2023
                return "FindResultsOn";*/
            /*nd = node.SelectSingleNode(".//g-tray-header[contains(@class,'kno-fb-ctx gsrt')]"); //23-03-2022//19-01-2023 //Top Sights and Flights
            if (nd != null)
            return "TopSights";*/ //23-03-2022//19-01-2023
            if ((node.SelectSingleNode(".//div[contains(@class,'WlTAzf')]|.//div[contains(@class, 'tkQJMd')]|.//div[@class='zhYvOe']") != null //03-01-2024
                || node.Attributes["class"]?.Value == "WlTAzf mnr-c vk_c" || node.Attributes["class"]?.Value == "WlTAzf Ww4FFb vt6azd vk_c")
                && node.SelectSingleNode(".//div[@jscontroller='UjNCHf']") == null)//02-01-2024//22-09-2023//21-09-2023//03-07-2023 //23-03-2022
                return "Flights";//23-03-2022

            if (node.SelectSingleNode(".//g-card[@class='cvoI5e']|.//g-tray-header[contains(@class,'iI6nue')]") != null || node.SelectSingleNode(".//g-card[@class='U8KfXc']") != null)//12-03-2024//11-01-2023 //23-11-2020 //20-11-2020
            {
                return "Jobs";
            }

            if (node.SelectSingleNode(".//div[@id='imso-root']") != null || node.SelectSingleNode(".//div[contains(@class,'tsp-view')]") != null //24-11-2020 for eventresults included contains func //node.SelectSingleNode(".//div[@class='tsp-view r-iDNua10DBk4I']") != null
                || node.SelectSingleNode(".//div[@class='nA3Vyd SBFvB']") != null || node.SelectSingleNode(".//div[@class='nJXhWc nA3Vyd']") != null
                || node.SelectSingleNode(".//div[@class='AE4e7c']") != null || node.SelectSingleNode(".//div[@class='SBFvB']") != null //06-10-2021 Event block   //22-05-2020 included selector for event block
                || node.SelectSingleNode(".//div[@class='imso-ml-c PZPZlf']|.//div[@class='RRXMad']|.//div[@jsname='GDPwke'] ") != null)//22-09-2022 event block
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
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='MQv7ze']");  // 23-06-2020 
            //if (nd == null)
            //    nd = node.SelectSingleNode(".//div[@class='kp-blk EyBRub knowledge-panel OJXvsb']");//05-10-2020 commented  //13-07-2020 images block type and KP block type
            if (nd == null)
                nd = node.SelectSingleNode(".//div[contains(@class,'kp-blk EyBRub')]|.//div[contains(@class,'kp-hc')]"); //16-02-2022 //05-10-2020 included selector for missing KP block
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Y2NmGf']|.//div[@class='aiNDEb']|.//div[@class='grvXqb']");//27-02-2024//08-11-2023//26-07-2022
            if (nd != null)
            {
                // 24-04-2020
                //nd = node.SelectSingleNode(".//h2"); //22-07-2020 commented
                nd = node.SelectSingleNode(".//div[@class='g8xmv']|.//div[@class='gnAmUb']"); //25-10-2021
                if (nd != null)
                    return "Carousel";
                // 24-04-2020

                nd = node.SelectSingleNode(".//div[@class='kp-blk nGydZ Wnoohf OJXvsb']|.//div[@class='NFQFxe XbtRGb qxsd xsZWvb EfDVh WDjuKe mod']|.//div[@class='NFQFxe viOShc LKPcQc mod']|.//div[@class='B3nbW mfMhoc']|.//div[@class='Ph8vHd']");//08-04-2022 KP
                if (nd == null || node.SelectSingleNode(".//div[@class='kp-header']") != null || node.SelectSingleNode(".//div[@class='K2Sb0e kp-header']") != null)   //23-06-2020 //19-06-2020
                    if (node.SelectSingleNode(".//div[@class='RzdJxc']|.//div[@class='EDblX DAVP1 qIfKhf yUxSId']|.//div[@class='CEzIMc h7uAbe']|.//div[@class='jhtnKe']") == null) //27-02-2023 //06-12-2021 wrong block  //30-08-2021 video block missing
                        return "KnowledgePanel";
            }
            //swapped 19-03-2020


            // 30-10-2019
            nd = node.SelectSingleNode(".//div[@class='_ELb']/a");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='twQ0Be']/a|.//div[@class='dbsr']/a");//08-01-2024
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kno-fb-ctx']");
            if (nd == null)
                nd = node.SelectSingleNode(".//span[@class='DOvy9b z1asCe KXvzXb']");   //23-01-2020  // 21-02-2020  included selector for video card
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
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='aD8dbe']");//14-09-2020  Answered Card selector
            if (nd == null)
                nd = node.SelectSingleNode(".//div/span[@class='YkgoD z1asCe GYDk8c']|.//div/span[@class='Ca2Qlc z1asCe GYDk8c']|.//div[@class='MHStgc']/span"); //05-10-2020 Answercard
            if (nd == null)
                nd = node.SelectSingleNode(".//w-answer/div[@class='MUxGbd t51gnb lyLwlc lEBKkf']"); //29-09-2020 answer card
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='PZPZlf hb8SAc']");//21-12-2020 selector for answer card block
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='ifM9O']"); //28-02-2022
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='MUxGbd gbj1yb WZ8Tjf aLF0Z OSrXXb']"); //04-11-2022
            if (nd == null)
                nd = node.SelectSingleNode(".//div[contains(@class, 'vkc_np')]");//05-12-2023 answer card
            if (nd == null)//13-12-2023
                nd = node.SelectSingleNode(".//div[@class='V3FYCf']|.//div[@class='LkFg6b']|.//div[@class='wHYlTd C5w57c']");//02-02-2024//01-02-2024//13-12-2023
            if (nd != null)
            {
                //nd = node.SelectSingleNode(".//div[@class='MVpwye Dv3U4e']");//05-12-2023//28-09-2022 answer card
                if (node.SelectSingleNode(".//div[@class='FEoF4d']|.//div[@class='kno-rdesc']|.//div[contains(@class,'rbR0cd')]" +
                   "|.//div[@class='knowledge-finance-wholepage-chart__fw-uch']|.//div[@class='WFxqwc']|.//div[@class='KrvXD']" +
                   "|.//div[@class='hoJlSb']|.//div[@class='Vo9TVc nG7hRb']|.//div[@jsname='xQjRM']") == null)//14-02-2024//06-02-2024//05-02-2024//07-12-2023//08-11-2023 //23-09-2023 //24-11-2022
                    return "AnswerCard";
            }


            // changes on 15-07-2019
            nd = node.SelectSingleNode(".//div[@class='qs-io aig-lst']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='ki5rnd']|.//div[@class='UyqAp']");//27-07-2022 //23-01-2020   //21-02-2020 included selector for App block
            if (nd != null)
            {
                if (node.SelectSingleNode(".//g-img[@class='o8ebK']") == null)//27-07-2022
                    return "Apps";
            }
            //16-08-2019
            nd = node.SelectSingleNode(".//div[@jsmodel='uIhXXc']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@jsname='GDPwke']"); // 18-10-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='g8xmv']"); //12-07-2022
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Y2NmGf']"); //05-01-2022
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='MIyI4c']"); //19-08-2021 updated for carousel block 
            if (nd != null)
                if (node.SelectSingleNode(".//img[contains(@alt,'Map of ')]|.//div[@jsname='sSUqrd']|.//div[@class='EDblX DAVP1']") == null || (node.SelectSingleNode(".//div[@class='g8xmv']|.//div[@class='gGI6vc']|.//div[@jsname='qz04Zb']") != null))//19-01-2024//19-09-2023//25-07-2022//21-07-2022 //19-07-2022
                    return "Carousel";

            nd = node.SelectSingleNode(".//div[@class='TyzpY']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='SRYuRe']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='w8TE8']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@id='tsuid196']");
            if (nd != null)
                if (!node.InnerText.Contains("Popular products") && node.SelectSingleNode(".//div[@class='I2lQic']") == null && node.SelectSingleNode(".//div[@class='IEBeid']") == null && node.SelectSingleNode(".//div[@class='IEBeid']") == null && node.SelectSingleNode(".//div[@id='iur']") == null)//29-08-2022)//09-03-2022//24-09-2021 maps //29-06-2021 avoiding wrong block
                    if (node.SelectSingleNode(".//div[contains(@class,'qdrjAc Dwsemf')]|.//div[contains(@class,'zTpPx')]") == null && node.SelectSingleNode(".//img[contains(@alt,'Map of ')]") == null && node.SelectSingleNode(".//g-img[@class='o8ebK']|.//div[@class='EDblX HG5ZQb']") == null)//19-12-2023//20-05-2022//05-05-2022 //28-10-2021 //28-10-2021
                        return "Videos";

            nd = node.SelectSingleNode(".//div[@class='TvV1fe']|.//div[@class='pXvdUe']"); //14-12-2020 videos
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='zK9jzc B3JUpd']");
            if (nd != null && node.SelectSingleNode(".//div[contains(@class, ' knowledge-panel ')]|.//span[@class='V7Sr0 mfMhoc']|.//div[@class='KoYIdc']") == null) //16-09-2021 selector removing wrong block //03-09-2021 missing carousel block
            {
                // Changes in Videos block on 25-06-2019
                if (node.InnerText.ToLower().Contains("video") || node.InnerText.StartsWith("فيديوهات"))    // 29-11-2019//02-04-2021 removed s from videos condition
                {
                    return "Videos";
                }
            }
            //changed on 21-08-2019
            else
            {
                nd = node.SelectSingleNode(".//div[@role='heading']/div[@class='HnYYW']");
                if (nd == null)
                    nd = node.SelectSingleNode(".//div[@role='heading']/div[@class='HnYYW i8lZMc']|.//div[@role='heading']/div[@class='HnYYW mfMhoc']|.//div[@class='BNeawe deIvCb AP7Wnd']|.//div[contains(@class,'HnYYW')]/div[@role='heading']");//19-02-2024//19-11-2021//30-07-2020   // 24-06-2020   // 21-02-2020
                if (nd == null)//26-02-2024
                    nd = node.SelectSingleNode(".//div[contains(@class, 'QlyiV')]");//26-02-2024
                if (nd != null)
                {
                    if (nd.InnerHtml.ToLower().StartsWith("video") || nd.InnerHtml.ToLower().StartsWith("vídeo")) //07-02-2020 included title for video block for different language
                    {//19-02-2024
                        if (node.SelectSingleNode(".//div[@class='vbbrJ']|.//div[contains(@class, 'CGCvRb')]|.//div[@class='uZ6Jn I0QQHd']|.//div[@class='EDblX HG5ZQb']") == null)
                            return "Videos";
                    }//19-02-2024
                    else if (nd.InnerHtml.ToLower().StartsWith("top stories") || nd.InnerHtml.Contains("Interesting finds") || nd.InnerText.Contains("Noticias "))//26-02-2024 // 18-12-2019)
                        return "Topstories";
                    HtmlNode nd1 = node.SelectSingleNode(".//div[@class='UDZeY fAgajc']|.//div[@class='rKFBM gsrt CAd2fd wp-ms']|.//div[@class='WFxqwc BGdUVb']");//07-12-2023  //31-03-2020
                    if (nd1 != null)
                    {
                        return "AnswerCard";
                    }
                }
            }
            nd = node.SelectSingleNode(".//div[contains(@class, 'HOslld dutT5c')]|.//div[contains(@class, 'XNfAUb')]");//20-02-2024
            if (nd != null && node.SelectSingleNode(".//div[contains(@class, 'MRtunc')]|.//div[@class='Wt5Tfe']|.//div[@class='RyIFgf']") == null
                || (node.SelectSingleNode(".//div[@class='VqeGe']") != null && node.SelectSingleNode(".//div[@class='NYidgb']|.//div[@class='xKf9F']") == null))//27-02-2024//26-02-2024//20-02-2024
            {
                if ((node.SelectSingleNode(".//div[@class='Gqsa8d']") != null && node.SelectSingleNode(".//div[@class='EDblX HG5ZQb']") != null)//27-02-2024
                    || node.SelectSingleNode(".//div[@class='HOslld dutT5c']|.//div[@class='zJUuqf adDDi']") != null)//27-02-2024
                    return "Hotel";
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
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='IbDT9d q8U8x aTI8gc RES9jf']"); //17-02-2022
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='IEBeid']|.//g-img[@class='o8ebK']|.//div[@class='KrvXD']|.//div[@class='hoJlSb']");//05-02-2024//16-09-2022//10-05-2022 //04-03-2022
            if (nd != null)
            {
                if (nd.SelectSingleNode(".//div[@jsname='r4nke']") == null)//27-07-2022
                    return "Maps";
            }
            else
            {
                nd = node.SelectSingleNode(".//div[@class='mnr-c']/g-link/a");
                if (nd != null)
                    if (nd.Attributes["href"].Value.Contains("/maps/"))
                        return "Maps";

                // changes on 08-07-2019
                nd = node.SelectSingleNode(".//img[@alt='map image']|.//img[@alt='Affected area map']|.//img[contains(@alt,'Map of ')]|.//img[@alt='Affected area']|.//img[@alt='Immagine mappa']|.//img[contains(@alt,'Map from')]|.//img[contains(@alt,'Mappa di')]");//02-05-2022//22-11-2021//30-11-2020 //19-06-2020 // 13-03-2020 //01-05-2020
                if (nd != null & node.SelectSingleNode(".//div[@class='Vvrpbd']") == null)//14-08-2023
                    return "Maps";
            }

            nd = node.SelectSingleNode(".//div[@class='JVrfPc']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='bUNBRd mnr-c']|.//div[@class='HnYYW i8lZMc']|.//div[@class='HnYYW mfMhoc']|.//div[@class='HnYYW']/div|.//div[@class='HnYYW DFkChc']");//04-07-2023//20-11-2020 twiter classic links//26-06-2020 //13-03-2020 //include on 2019-06-24
            if (nd == null)
                nd = node.SelectSingleNode(".//g-card[@class='g F6CFcc']|.//g-inner-card[contains(@class,'Bf5NPb')]|.//div[@class='Bv2VAe']");//23-10-2023//14-08-2023//26-06-2023//03-06-2021 twitter block
            if (nd != null)
            {
                if (nd.InnerText.Contains("Twitter") || nd.SelectSingleNode(".//g-link") != null) //07-01-2021 twitter link
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
                || node.SelectSingleNode(".//h2[@class='MA9Une zbA8Me']|.//h2[@class='wITvVb']") != null // 14-12-2020
                || node.SelectSingleNode(".//div[@data-hveid='CD0Q-wE']") != null
                || node.SelectSingleNode(".//h2[@class='XS4Rbf zbA8Me']") != null
                || (node.SelectSingleNode(".//div[@class='kp-blk cUnQKe OJXvsb']") != null
                || node.SelectSingleNode(".//div[@jsname='N760b']") != null//07-08-2020 included selector people also ask
                && node.SelectSingleNode(".//div[@class='answered-question']") == null)
                || (node.SelectSingleNode(".//div[@class='kp-blk cUnQKe Wnoohf OJXvsb']") != null))
            {
                if (!node.InnerText.StartsWith("People also search for") && node.SelectSingleNode(".//div[contains(@class,'Eee1Bd')]|.//div[contains(@class,'AuVD')]|.//div[@class='NYidgb']") == null)//13-10-2023//27-01-2023//11-07-2022 contains //21-12-2020//25-05-2020
                    return "PeopleAlsoAsk";
            }
            // changed on 05-07-2019
            else if (node.SelectSingleNode(".//div[@class='HnYYW']") != null)
            {
                HtmlNode n = node.SelectSingleNode(".//div[@class='HnYYW']");
                if (n.InnerText == "People also ask" || n.InnerText == "Nutzer fragen auch" || n.InnerText == "Le persone hanno chiesto anche" || n.InnerText == "Orang juga bertanya")  //26-11-2019
                    return "PeopleAlsoAsk";
            }

            else if (node.SelectSingleNode(".//span[@class='mfMhoc']") != null)//12-07-2020
            {
                HtmlNode n = node.SelectSingleNode(".//span[@class='mfMhoc']");
                if (n.InnerText == "People also ask" || n.InnerText == "Nutzer fragen auch" || n.InnerText == "Le persone hanno chiesto anche" || n.InnerText == "Orang juga bertanya")  //26-11-2019
                    return "PeopleAlsoAsk";
            }//12-07-2020

            if (node.SelectSingleNode(".//div[@id='cwmcwd']") != null || node.SelectSingleNode(".//div[@class='vk_bk vk_ans']") != null
                || node.SelectSingleNode(".//div[@class='vk_ans vk_bk']") != null || node.SelectSingleNode(".//div[@data-tts='answers']") != null
                || node.SelectSingleNode(".//div[@class='vk_gy vk_sh whenis']") != null || node.SelectSingleNode(".//div[@class='N6Sb2c i29hTd']") != null
                || node.SelectSingleNode(".//div[@class='kp-blk c2xzTb OJXvsb']") != null
                || (node.SelectSingleNode(".//div[@class='kp-blk cUnQKe OJXvsb']") != null
                && node.SelectSingleNode(".//div[@class='answered-question']") != null)
                || node.SelectSingleNode(".//div[@class='vkc_np kkww4d']") != null    // changed on 05-07-2019
                || (node.SelectSingleNode(".//div[@class='UDZeY fAgajc']") != null && node.SelectSingleNode(".//span[@class='at3QRb VqFMTc p8AiDd']") == null) //12-11-2021    // 13-03-2020
                || node.SelectSingleNode(".//div[@class='wQu7gc']") != null   //08-07-2020 mising answered card
                || node.SelectSingleNode(".//div[@class='kp-blk OJXvsb']") != null  //26-08-2020 included selector for answered 
                || (node.SelectSingleNode(".//div[@class='g card-section']") != null && node.SelectSingleNode(".//div[@class='tF2Cxc']") != null))//22-04-2021 answered card
                if (node.SelectSingleNode(".//div[@class='kno-rdesc']") == null)//23-09-2023
                return "AnswerCard";

            nd = node.SelectSingleNode(".//div[@id='kx']|.//div[@class='pXvdUe']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='OixsOd']"); //25-06-2020
            //if (nd != null && node.SelectSingleNode(".//span[@class='FCUp0c rQMQod']").InnerText != "Images")  //13-07-2020 same selector under images block use for both images and carousel
            if (nd != null && node.SelectSingleNode(".//span[@class='FCUp0c rQMQod']|.//span[contains(@class,'mfMhoc')]|.//span[@class='r0bn4c rQMQod tP9Zud']|.//span[@class='q8U8x aTI8gc RES9jf']").InnerText != "Images")//15-11-2021//15-09-2021 updated contains for carousel block//02-09-2021 Carousel block//24-02-2021
            {
                if (node.SelectSingleNode(".//div[@class='TOQyFc U48fD']") == null) //11-07-2022
                    return "Carousel";
            }


            nd = node.SelectSingleNode(".//div[contains(@class,'rKFBM')]/div"); //28-07-2020 included contains functions
            if (nd == null)
                nd = node.SelectSingleNode(".//div[contains(@class,'JNkvid')]/div"); //28-07-2020 included selectors with contains functions
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='sPeCJd']"); //21-12-2020 selector for carousel
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='R37eoe ibYmtf']");//28-10-2022
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Vvrpbd']");//05-12-2023
            if (nd != null)
            {
                if (node.SelectSingleNode(".//g-scrolling-carousel|.//div[@class='xKf9F']") != null)//26-02-2024
                    // 04-11-2019
                    if (nd.InnerText.StartsWith("Movies") || nd.InnerText.StartsWith("Mga Pelikula")
                        || nd.InnerText.StartsWith("Film") || nd.InnerText.StartsWith("Filme")
                        || nd.InnerText.StartsWith("Books") || nd.InnerText.StartsWith("Películas") || nd.InnerText.StartsWith("Pel&#237;culas"))//26-02-2024 //29-07-2020 // 09-06-2020
                    {
                        return "Carousel";
                    }
                if (node.SelectSingleNode(".//div[contains(@data-attrid,'movies')]|.//div[contains(@data-attrid,'book')]") != null) //07-01-2021 carousel block
                {
                    return "Carousel";
                }
            }


            nd = node.SelectSingleNode(".//div[@class='bUNBRd mnr-c']/div/g-tray-header/div");   // || node.SelectSingleNode(".//div[@id='imagebox_bigimages']") != null)
            if (nd == null)
                nd = node.SelectSingleNode(".//g-tray-header/div[contains(@class,'N60dNb')]/a"); //24-09-2020 update above line to this selector
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='N60dNb']/a|.//g-tray-header/div[@class='N60dNb i8lZMc']/div[@class='rqLLId i8lZMc']"); //05-06-2020 //08-06-2020
                //if (nd == null)
                // nd = node.SelectSingleNode(".//div[@class='bUNBRd mnr-c']/g-tray-header/div[@class='N60dNb i8lZMc']/a"); //21-07-2020 commented    //17-02-2020 included selector for images
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='gID6df']|.//div[@id='iur']/a|.//div[@id='iur']");//06-12-2021 image block //12-06-2020//05-06-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='GNxIwf']");  // 18-03-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='BNeawe UwRFLe']/span"); //15-12-2020 applied selector to avoid wrong item urls
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='mR2gOd pptFR']"); //25-02-2021 images selector
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='u4WRYb']");//06-12-2021 image block
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='ysxiae iRPzcb']");//23-10-2023
            if (nd != null)
            {
                if (node.SelectSingleNode(".//div[@class='VPyzge']") == null && (node.SelectSingleNode(".//div[@class='kno-fiu kno-liu']" +
                    "|.//div[@class='N60dNb mfMhoc']|.//div[@jsmodel='vqHyhf']|.//div[@jsmodel='Wn3aEc']|.//div[@class='HdBr8']|.//div[@class='VKHL9c']" +
                    "|.//div[@class='IZE3Td']") != null))//02-02-2024//01-02-2024//15-12-2023
                    return "Images";
            }
            else
            {
                nd = node.SelectSingleNode(".//div[@class='rKFBM gsrt wp-ms']|.//div[@class='JNkvid gsrt VJIO7b BUnLGf wp-ms']|.//span[@class='FCUp0c rQMQod']|.//span[@class='mgAbYb OSrXXb RES9jf IFnjPb']");//06-11-2023//11-08-2023 //13-07-2020 images selector    // changes on 11-07-2019
                if (nd != null)
                {
                    if (nd.InnerText == "About")//26-10-2023
                        return "AnswerCard";
                    if (nd.InnerText.StartsWith("Images") || nd.InnerText == "Imágenes")//26-10-20232020 and 10-02-2020 21-02-2020 included title for images block
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
                nd = node.SelectSingleNode(".//div[@class='aviV4d']|.//div[@class='ILuMad t6aJGf']|.//div[@class='c23Rxd UyGhtb']");//15-12-2023//12-10-2021
            if (nd == null)
                nd = node.SelectSingleNode(".//div[contains(@class,'rbR0cd')]");//08-05-2021 contains //21-05-2020 finance block included selector
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='knowledge-finance-wholepage-chart__fw-uch']"); //16-12-2021 finance block
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
            nd = node.SelectSingleNode(".//div[@class='aJegcc']|.//div[@class='IGtt6d imgac mnr-c cTMkTb']");//08-11-2023
            if (nd != null)
                if (node.SelectNodes(".//div[contains(@class,'xCCdqb')]|.//div[contains(@class, 'tyUpi')]|.//div[@class='baPFxb g kSMK2']|.//div[contains(@class,'wTrwWd')]") == null)//23-10-2023//11-04-2023//10-04-2023//06-12-2021 wrong PL block//16-09-2019
                {
                    return "ProductListedAds";
                }
            return "";
        }

        public bool IsBlock(HtmlNode node)
        {
            // changes on 05-07-2019
            HtmlNode nd = node.SelectSingleNode(".//div[@class='HnYYW']|.//g-tray-header[@role='heading']|.//div[@role='heading']");
            if (nd != null)
            {
                if (nd.InnerText.Trim() == "Top stories" || nd.InnerText.ToLower().Contains("noticias") || nd.InnerText.Trim() == "Notizie principali" || nd.InnerText.Trim() == "Interesting finds" //04-11-2020//16-09-2020
                     || nd.InnerText.ToLower().Contains("últimas noticias") || nd.InnerText.ToLower().Contains("det senaste")
                     || nd.InnerText.ToLower().StartsWith("latest") || nd.InnerText.ToLower().Contains("map")//07-08-2020  //23-06-2020 //22-06-2020
                     || nd.InnerText.ToLower().Contains("notícias")) //08-01-2021 top stories
                    if ((node.SelectSingleNode(".//div[@id='tscffb']") != null || node.SelectSingleNode(".//div[@class='KJDcUb']") == null) 
                        && node.SelectSingleNode(".//div/a[contains(@class,'C8nzq BmP5tf')]") == null && node.SelectSingleNode(".//div/a[contains(@class,'cz3goc BmP5tf')]") == null
                        && node.SelectSingleNode(".//g-card[@id='tscffb']") == null) //19-05-2022//12-11-2021 //25-05-2021 //04-01-2021 video block
                        return true;
                if (node.SelectSingleNode(".//div[@class='ttfMne']|.//div[@class='N60dNb mfMhoc']|.//h2[@class='OEsCyf mfMhoc']|.//div[@class='kp-blk c2xzTb OJXvsb']|.//div[@class='WpKAof']|.//g-card/div[@class='mnr-c']|.//div[@class='FQrfLd']") != null)//04-11-2022 //23-11-2021 CB //27-10-2021 //01-09-2021 missing AC block//12-07-2021 job block //12-07-2021 carousel block //19-01-2021 missing top stories
                    if (node.SelectSingleNode(".//div[@class='WvKfwe a3spGf']|.//div[@class='V1nn0e wgFKp']|.//div[@jscontroller='rMVp5e']|.//div[@jscontroller='rQR4vd']|.//div[contains(@class,'P8ujBc')]|.//div[contains(@class,'jqWpsc')]") != null)//16-09-2022//06-05-2022//21-01-2022//13-01-2022//17-12-2021 //12-10-2021
                        return false;
                    else if (node.SelectSingleNode(".//div[contains(@class,'BToiNc')]") == null)//11-07-2023
                        return true;//12-11-2021
                if (nd.InnerText == "More results" || nd.InnerText == "Top results" || nd.InnerText == "Toppresultater" || nd.InnerText == "También se buscó" //27-10-2021
                     || nd.InnerText == "Fler resultat" || nd.InnerText == "Flere resultater" || nd.InnerText == "Plus de résultats")    // 13-12-2019
                    return false;
            }
            if (node.SelectSingleNode(".//div[contains(@class, 'Z3ngN')]") != null) //22-03-2021
                return false;
            if (node.SelectSingleNode(".//video-voyager[@class='LnSx5b']|.//div[@class='lNvPub v5yQqb']|.//div[@class='adXOEf v5yQqb']|.//div[@class='T61Aje v5yQqb']") != null)//23-08-2023//15-03-2023//16-12-2021
                return false;//16-12-2021
            //17-01-2020
            nd = node.SelectSingleNode(".//div[@class='Lgnr0e J88qA BmP5tf']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']|.//div[@class='LEHmvf adDDi']|.//div[@class='V3FYCf']");//02-02-2024
            if (nd != null)
            {
                //30-03-2020
                if (node.SelectSingleNode(".//div[@class='g card-section jiwmWe']|.//div[contains(@class,'xpd EtOod pkphOe')]") != null)//30-08-2023
                    return false;
                if (node.SelectSingleNode(".//div[@class='MUxGbd v0nnCb lyLwlc']|.//div[@class='uVMCKf Ww4FFb vt6azd']") != null && node.SelectSingleNode(".//div[@class='LEHmvf adDDi']") == null)//29-09-2023//28-08-2023//25-04-2020
                    return false;
                return true;
            }

            // if (node.SelectSingleNode(".//div[@class='g card-section svwwZ']|.//div[@class='c6gxKe card-section']|.//div[@class='g card-section']") != null) //30-08-2021 //20-05-2021//03-11-2020
            if (node.SelectSingleNode(".//div[@class='g card-section svwwZ']|.//div[@class='c6gxKe card-section']|.//div[@class='g card-section']|.//div[@class='card-section svwwZ']") != null //10-11-2022
                && node.SelectSingleNode(".//div[@data-tts='answers']|.//div[@class='ifM9O']") == null)//27-01-2022 answered card//07-09-2021
                return false;

            //22-11-2019
            nd = node.SelectSingleNode(".//div[@class='g kno-result rQUFld mnr-c g-blk']|.//w-answer/div[@class='MUxGbd t51gnb lyLwlc lEBKkf']|.//div[@class='ifM9O']|.//div[@class='Q9mvUc']|.//div[@class='tkQJMd']|.//div[contains(@class, 'RPdfze')]|.//div[contains(@class, 'Qkn3ie')]");//07-07-2023 FindResultsOn block //03-07-2023//08-07-2022//16-02-2022 //01-10-2020 Answered Card selector included
            if (nd != null)
            {
                return true;
            }

            //29-11-2019
            nd = node.SelectSingleNode(".//div[@class='KJDcUb WzRKRb']|.//div[contains(@class,'P8ujBc')]" +
             "|.//div[@class='mnr-c P5XtRe']|.//div[@class='urrG9 v5yQqb jqWpsc']|.//div[@class='lNvPub cP7qLd v5yQqb']" +
             "|.//div[contains(@class,'EtOod pkphOe')]|.//div[@class='mnr-c Eiw3V']|.//div[contains(@class, 'WFyfFf')]");//14-02-2024//23-08-2023//17-05-2023//26-09-2022 //29-06-2022
            if (nd != null && nd.SelectSingleNode(".//div[contains(@class,'BNeawe')]") == null)//13-12-2023
            {
                return false;
            }

            nd = node.SelectSingleNode(".//div[@class='aJegcc']");//05-01-2023
            if (nd != null)
                return true;//05-01-2023
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

            nd = node.SelectSingleNode(".//div[@data-tts='answers']|.//div[@class='N6Sb2c i29hTd']|.//div[@class='kp-blk c2xzTb OJXvsb']|.//div[@class='answered-question']|.//div[@class='UDZeY fAgajc']"); //02-09-2020 included selector for (About)Answer Card
            if (nd != null)
            {
                return true;
            }
            nd = node.SelectSingleNode(".//div[@class='oLO3I']|.//div[contains(@class, 'tw-res')]");//27-10-2021
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='aZVgnb']/h2");  // 08-06-2020  
            if (nd != null)
                if (node.SelectSingleNode(".//div[@class='WvKfwe a3spGf']") != null)//28-10-2021
                    return false;//28-10-2021
                else//28-10-2021
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
                nd = node.SelectSingleNode(".//g-card[@class='XqIXXe']|.//div[@class='kGH5dd']");  //19-06-2020
                if (nd != null)
                {
                    nd = node.SelectSingleNode(".//div[@class='zK9jzc B3JUpd i8lZMc']"); //17-01-2020 selector changed videos block
                    if (nd == null)
                        //nd = node.SelectSingleNode(".//g-tray-header[@class='kno-fb-ctx lQckZe gsrt ieGFJe ndEm3b']"); //08-09-2020 commented //21-02-2020 included selector for videos card
                        nd = node.SelectSingleNode(".//g-tray-header[contains(@class,'kno-fb-ctx lQckZe')]");//26-02-2021 above line commented and included contains //08-09-2020 commented //21-02-2020 included selector for videos card
                    if (nd != null)
                    {
                        //if (nd.InnerText.Trim() == "Recipes" || nd.InnerText.Trim() == "Recept" || nd.InnerText.Trim() == "Ricette")// 21-11-2019
                        return true;
                    }//04-11-2019"}//14-09-2020                    return false;
                }
                return true;
            }
            nd = node.SelectSingleNode(".//div[@jscontroller='iht5n']");
            //nd = node.SelectSingleNode(".//div[@jsmodel='uIhXXc']");
            if (nd != null)
                return false;
            //24-11-2020 updated selector for evenResults boolean
            nd = node.SelectSingleNode(".//div[@class='tsp-view']|.//div[@class='Y2NmGf']"); //25-10-2021
            if (nd != null)
            {
                return true;
            }
            //end 24-11-2020
            nd = node.SelectSingleNode(".//div[@jscontroller='UrRncd']/div/div/a[@class='C8nzq BmP5tf amp_r']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@jscontroller='UrRncd']/div/div/a[@class='C8nzq BmP5tf']");  // 25-10-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='KJDcUb']/a[@class='C8nzq BmP5tf amp_r']");  // 25-10-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='KJDcUb']/a[@class='C8nzq BmP5tf']");  // 25-10-2019
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='kp-blk Wnoohf OJXvsb']");
            if (nd == null)
                nd = node.SelectSingleNode(".//div[@class='mnr-c']/div"); //10-07-2021
            if (node.SelectNodes(".//div[contains(@class,'aD8dbe')]") != null)//14-09-2020 updated selector return true for empty block
            {
                return true;
            }//14-09-2020
            if (nd == null)
                nd = node.SelectSingleNode(".//div[contains(@class,'khgTR')]");  //16-09-2020 applied contains function
            if (nd != null)
            {
                return false;
            }
            //start 06-08-2019
            nd = node.SelectSingleNode(".//div[@class='f570C']|.//div[@class='Q9mvUc']");//07-01-2022
            if (nd != null && node.SelectSingleNode(".//div[@class='KoYIdc']") == null) //06-05-2022
            {
                if (node.SelectSingleNode(".//table[@class='std']|.//div[@class='di8g3 ChOqnd']") == null)//16-09-2021 ignore wrong url //15-09-2021 ignoring wrong classic links
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
                nd = node.SelectSingleNode(".//div[@class='mnr-c xpd O9g5cc uUPGi']|.//div[@class='mnr-c OH1ZUd xpd O9g5cc uUPGi']|.//div[@class='ytwLQd']");//27-01-2022 classic links//05-06-2020 missing classic links
                if (nd != null && node.SelectSingleNode(".//div[@class='EDblX m8vZ3d']") == null || node.SelectSingleNode(".//div[@jscontroller='KP4k7d']") != null) //12-11-2021 CL   // 16-10-2019
                {
                    //if (node.SelectSingleNode(".//div[contains(@class,'BNeawe')]") != null && node.SelectSingleNode(".//div[contains(@class,'au0C1b')]") == null)//02-03-2021 included contains for classic links
                    if (node.SelectSingleNode(".//div[contains(@class,'BNeawe')]") != null && node.SelectSingleNode(".//div[contains(@class,'au0C1b')]") == null 
                        && node.SelectSingleNode(".//div[contains(@class,'PUrSVb')]") == null && node.SelectSingleNode(".//div[@class='Q9mvUc']") == null 
                        || node.SelectSingleNode(".//div[@class='pXvdUe']") != null && node.SelectSingleNode(".//div[@class='KoYIdc']") == null) //18-05-2022 //11-05-2022//05-01-2022 carousel //25-10-2021
                        return true;
                    return false;
                }
                else if (nd != null && (node.SelectSingleNode(".//div[contains(@class,'KJDcUb')]") != null || node.SelectSingleNode(".//div[@class='U3THc']") != null)) //11-11-2021    //09-09-2021 applied contains // 22-01-2020 included selector for classic links
                {
                    return false;
                }
            }
            if (node.SelectSingleNode(".//div[@class='card-section']") != null && node.SelectSingleNode(".//div[@class='tF2Cxc']") != null)
            {
                nd = node.SelectSingleNode(".//h3[@class='yuRUbf JtG40d V7Sr0']"); //22-04-2021
                if (nd != null) return false; //22-04-2021
            }
            return (!node.HasClass("srg")); // && node.SelectSingleNode(".//div[@class='ZINbbc xpd']") == null);   // block                
        }

        public bool IsOrganic(HtmlNode node)
        {
            return (node.HasClass("srg") || node.SelectSingleNode(".//div[@class='oITGTd aSYQ6c']") != null
                || node.SelectSingleNode(".//div[@class='ZINbbc xpd']") != null
                || node.SelectSingleNode(".//div[@class='mnr-c xpd O9g5cc uUPGi']") != null
                || node.SelectSingleNode(".//div[@class='mnr-c']") != null //05-06-2020
                || node.Attributes["class"]?.Value == "mnr-c xpd O9g5cc uUPGi"
                || node.SelectSingleNode(".//div[@class='vC5Ym']") != null
                || node.SelectSingleNode(".//div[@class='mnr-c OH1ZUd xpd O9g5cc uUPGi']") != null //27-01-2022 classic links
                || node.SelectSingleNode(".//div[@class='kp-blk Wnoohf OJXvsb']") != null
                || (node.SelectSingleNode(".//g-card[@class='XqIXXe']") != null && node.SelectSingleNode(".//g-card[@id='tscffb']") != null)
                || node.SelectSingleNode(".//div[@class='khgTR lWEpfd']") != null  //22-06-2020
                || node.SelectSingleNode(".//div[@class='ywTQJc']") != null //07-08-2020 
                || node.SelectSingleNode(".//div[@class='khgTR R5lVqb']") != null //26-08-2020 selector for missing classic link
                || node.SelectSingleNode(".//div[@class='V1nn0e R5lVqb']") != null //15-12-2020
                || node.SelectSingleNode(".//div[@class='card-section']") != null //15-12-2020
                || node.SelectSingleNode(".//div[contains(@class, 'Z3ngN')]") != null//22-03-2021
                || node.SelectSingleNode(".//div[@class='g card-section']") != null) && node.SelectSingleNode(".//div[@class='BNeawe']") == null //08-10-2021//30-08-2021
                || node.SelectSingleNode(".//video-voyager[@class='LnSx5b']|.//div[@class='lNvPub v5yQqb']") != null//15-03-2023 //16-12-2021
                || (node.SelectSingleNode(".//div[contains(@class,'EtOod pkphOe')]") != null && node.SelectSingleNode(".//div[@class='TOQyFc U48fD']") == null)//04-07-2022 //29-06-2022
                || node.SelectSingleNode(".//div[@class='mnr-c xpd EtOod pkphOe']") != null //10-06-2022
                || node.SelectSingleNode(".//div[@class='mnr-c P5XtRe']") != null //25-03-2022
                || node.SelectSingleNode(".//div[@class='urrG9 v5yQqb jqWpsc']|.//div[@class='lNvPub cP7qLd v5yQqb']") != null//17-05-2023 //31-05-2022
                                || node.SelectSingleNode(".//div[@class='urrG9 v5yQqb jqWpsc']|.//div[@class='lNvPub cP7qLd v5yQqb']|.//div[@class='adXOEf v5yQqb']" +
                "|.//div[@class='T61Aje v5yQqb']|.//div[contains(@class, 'WFyfFf')]") != null;//14-02-2024//23-08-2023 != null//17-05-2023 //31-05-2022
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
        public string GetRedirectedUrl(string url)
        {
            try  //28-09-2020  try catch.
            {
                //21-11-2019
                url = url.Replace("HTTPS://", "https://").Replace("HTTP://", "http://");
                if (string.IsNullOrEmpty(url.Trim())) return string.Empty;

                //24-09-2020 changed LastIndexOf to IndexOf
                if (url.IndexOf("https://") == 0 || url.IndexOf("https://") >= 0)//01-10-2020
                    url = url.Remove(0, url.IndexOf("https://"));
                else if (url.IndexOf("http://") == 0 || url.IndexOf("http://") >= 0) //18-11-2020//14-10-2020 included indexof for http
                    url = url.Remove(0, url.IndexOf("http://"));


                //end 24-09-2020
                Regex rx = new Regex("http[\\w]?://(.*)", RegexOptions.Singleline);
                if (!rx.Match(url).Success && !url.Contains("/aclk?"))
                    if (!url.Contains("://")) // 30-04-2020
                        url = "http://" + url;

                if (url.StartsWith("http:////") || url.StartsWith("https:////")) //18-09-2020 condition applied if appears http:////
                    url = url.Replace("////", "//"); //18-09-2020

                if (url.Contains("&amp;grqid="))
                    url = url.Remove(url.IndexOf("&amp;grqid="));

                //  13-12-2019
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

                if ((url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("ftp://")) && (!url.Contains("/aclk?") && !url.Contains("search?num=100")))  // 30-04-2020 //18-02-2020 and 24-02-2020 included condition
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
            if (url.Contains(@"\x26"))
                url = url.Replace(@"\x26amp;", "&"); //10-03-2022

            if (url.Contains(@"\x27"))
                url = url.Replace(@"\x27", "'"); //11-03-2022
            return url;
        }//12-03-2022 end
        // There are chances method was used for title contains in case any issues in xml applied decode/encode.
        public string SetTitle(string unicodestring)
        {
            //return WebUtility.HtmlEncode(WebUtility.HtmlDecode(unicodestring));
            return WebUtility.HtmlEncode(WebUtility.HtmlDecode(unicodestring)).Replace("\\x27", "'").Replace("\\\\u0026", "&amp;").Replace("\\\\\\x22", "&quot;");

        }

        public string SetUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty; //24-08-2020
            try  //28-09-2020  try catch.
            {
                //21-11-2019
                url = GetRedirectedUrl(WebUtility.HtmlDecode(url).Trim());
                //if (url.ToLower().Contains("%2f") || url.ToLower().Contains("%2e"))
                if (url.Contains("%")) //18-09-2020
                    url = GetRedirectedUrl(WebUtility.UrlDecode(WebUtility.HtmlDecode(url)).Trim());
                return WebUtility.HtmlEncode(SanitizeXmlString(url).Replace("\x00", "%00")).Replace("\\\\u003d", "=").Replace('\u0002', ' ').Replace('\u0018', ' ').Replace('\f', ' ').Trim(); //23-09-2020 applied method to URL//22-04-2020

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //27-08-2020
        public string GetRedirectedUrl_TextAds(string url)
        {
            if (string.IsNullOrEmpty(url) || url.StartsWith("#")) return string.Empty; //08-02-2022
            try  //28-09-2020  try catch.
            {
                url = url.Replace("HTTPS://", "https://").Replace("HTTP://", "http://");
                if (string.IsNullOrEmpty(url.Trim())) return string.Empty;

                Regex rx = new Regex("http[\\w]?://(.*)", RegexOptions.Singleline);
                if (!rx.Match(url).Success && !url.Contains("/aclk?") && !url.Contains("/localservices/")) //19-08-2022
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
                    url = WebUtility.UrlDecode(WebUtility.HtmlDecode(url).Trim()); //10-01-2020
                    return WebUtility.HtmlEncode(SanitizeXmlString(url).Replace("\x00", "%00")).Replace("\\\\u003d", "=").Replace('\u0002', ' ').Replace('\u0018', ' ').Replace('\f', ' ').Trim();//23-09-2020 applied method to URL
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
    }
}
