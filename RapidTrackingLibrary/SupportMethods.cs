using HtmlAgilityPack;
using System;
using System.IO;

namespace RapidTrackingLibrary
{
    public class SupportMethods
    {
        static public string GetSource(int seid, string keyword)
        {
            // from oxylabs
            //Source source = new Source(seid, keyword);
            //var src = (string[])source.GetSource().GetAwaiter().GetResult()[0];
            //JObject obj = JObject.Parse(src[1]);
            //string html = obj["results"][0]["content"].Value<string>();
            //return html;

            // from local path and WORKS ONLY FOR iOS or Desktop.
            string path = "C:\\inetpub\\wwwroot\\html\\6790352636458568705_joe biden.html";
            string html = File.ReadAllText(path);
            return html;
        }

        static HtmlNode GetiOSNodesBlocksList(string htmlSource, string blockName)
        {
            string ndText = string.Empty;
            iOS iOS = new iOS();
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlSource);

                HtmlNodeCollection nodeCol = doc.DocumentNode.SelectNodes("//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']/div[@class='MUxGbd v0nnCb lyLwlc']|//div[@class='Lgnr0e J88qA vgnU9e BmP5tf']/div/div[@class='MUxGbd v0nnCb lyLwlc']");   //29-04-2020
                if (nodeCol != null)
                    nodeCol = nodeCol[nodeCol.Count - 1].SelectNodes("a/div");  //28-04-2020
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div|//div[@id='rso']/g-card|//div[@id='taw']/div[@class='med']/div[2]/div|//div[@id='rso']/nav");   //28-04-2020
                if (nodeCol != null && nodeCol.Count == 1)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div|//div[@class='vC5Ym DhKAUb']/div");    //17-09-2019
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//*[@id='tscffb']");

                if (nodeCol == null) return null;

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
                                    string s = iOS.GetBlockType(nd);

                                    if (blockName.ToLower() == s.ToLower()) return nd;
                                    ndText += s;
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
                        //sb.Append("<block type=\"finance\" url=\"\"></block>");
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
                            n = node.SelectSingleNode(".//div[@class='Ftghae iirjIb']");//16-09-2019 //

                        if (n != null)
                        {
                            string heading = n.InnerText;
                            //sb.Append("<block type=\"knowledgeGraph\" url=\"\" title=\"" + SetTitle(heading) + "\" />");
                        }

                        continue;
                    }
                    try
                    {
                        if (node.InnerHtml != "")
                        {
                            string s = iOS.GetBlockType(node);
                            if (blockName.ToLower() == s.ToLower()) return node;
                            ndText += s;
                        }
                    }
                    catch
                    { }
                }

                if (string.IsNullOrEmpty(ndText.Trim()))
                {
                    foreach (HtmlNode node in nodeCol)
                    {
                        try
                        {
                            if (node.HasClass("kp-wholepage") || node.SelectNodes(".//div[contains(@class, 'kp-wholepage')]") != null)
                            {
                                //Current 15-12-2020 swapped from bottom HtmlNodeCollection
                                HtmlNodeCollection nc = node.SelectNodes(".//div[@class='WvKfwe']/div|.//div[@class='WvKfwe a3spGf']/div|.//div[@class='ChlgHf']|.//div[contains(@class,'UDZeY')]|.//div[@class='a3spGf WvKfwe']/div");//18-06-2020//|.//div[@class='uxUO1b g0S8Ze mnr-c']"); //17-06-2020 answer card //01-06-2020");  //15-04-2020     
                                if (nc == null)
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
                                            s = iOS.GetBlockType(nd);
                                            if (blockName.ToLower() == s.ToLower()) return nd;
                                            ndText += s;
                                        }
                                        catch { }
                                    }
                                }
                                break;
                            }
                        }
                        catch { }
                    }
                }

            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return null;

        }

        static HtmlNode GetDesktopNodesBlocksList(string htmlSource, string blockName)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlSource);

            HtmlNode htmlNode = doc.DocumentNode.SelectSingleNode("//table[@id='mn']");
            if (htmlNode != null)
            {
                return null;
            }

            Desktop desktop = new Desktop();
            string ndText = "";
            try //28-09-2020  try catch.
            {
                HtmlNodeCollection nodeCol = doc.DocumentNode.SelectNodes("//div[@class='_NId']");
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@class='bkWMgd']");
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='ires']/ol/div");//09-12-2020
                if (nodeCol == null)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='rso']/div|//div[@id='rso']/g-section-with-header");//03-12-2020  //01-05-2020         
                if (nodeCol == null || nodeCol.Count <= 1)
                    nodeCol = doc.DocumentNode.SelectNodes("//div[@id='kp-wp-tab-overview']/div|//div[@class='hlcw0c']/div") ?? nodeCol; //04-12-2020 //09-12-2020 no result issue

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
                            string s = desktop.GetBlockType(node);
                            if (blockName.ToLower() == s.ToLower()) return node;
                            ndText += s;
                        }
                    }
                    catch { }
                }

                // 23-03-2020
                if (string.IsNullOrEmpty(ndText))
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
                                string s = desktop.GetBlockType(node);
                                if (blockName.ToLower() == s.ToLower()) return node;
                                ndText += s;
                            }
                        }
                        catch { }
                    }
                }
                // 23-03-2020

                if (nodeCol == null) throw new Exception("No block found.");

            }
            catch { }
            return null;

        }




        // Desktop methods

        public static HtmlNode GetDesktopAnswerCard(int seid, string keyword)
        {
            var html = GetSource(seid, keyword);
            return GetDesktopNodesBlocksList(html, "AnswerCard");
        }

        public static HtmlNode GetDesktopTopStories(int seid, string keyword)
        {
            var html = GetSource(seid, keyword);
            return GetDesktopNodesBlocksList(html, "TopStories");
        }
        public static HtmlNode GetDesktopVideos(int seid, string keyword)
        {
            var html = GetSource(seid, keyword);
            return GetDesktopNodesBlocksList(html, "Videos");
        }




        // iOS methods

        public static HtmlNode GetiOSAnswerCard(int seid, string keyword)
        {
            var html = GetSource(seid, keyword);
            return GetiOSNodesBlocksList(html, "AnswerCard");
        }

        public static HtmlNode GetiOSTopStories(int seid, string keyword)
        {
            var html = GetSource(seid, keyword);
            return GetiOSNodesBlocksList(html, "TopStories");
        }

        public static HtmlNode GetiOSVideos(int seid, string keyword)
        {
            var html = GetSource(seid, keyword);
            return GetiOSNodesBlocksList(html, "Videos");
        }

        public static HtmlNode GetiOSImages(int seid, string keyword)
        {
            var html = GetSource(seid, keyword);
            return GetiOSNodesBlocksList(html, "Images");
        }

    }
}
