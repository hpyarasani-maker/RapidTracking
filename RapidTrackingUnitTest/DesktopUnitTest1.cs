using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidTrackingLibrary;
using HtmlAgilityPack;

namespace RapidTrackingUnitTest
{
    [TestClass]
    public class DesktopUnitTest1
    {
        readonly static string seid = "58";
        readonly static string keyword = "borris johnson";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Generate generate = new Generate();
            //generate.GenerateXml(seid, keyword);
            SupportMethods.DesktopHtml = generate.GenerateXml(seid, keyword);
        }


        List<ArrayList> GetProcessedLists()
        {
            string xml1 = "C:\\inetpub\\wwwroot\\selector.xml";
            string xml2 = "C:\\inetpub\\wwwroot\\regex.xml";

            List<ArrayList> al = new List<ArrayList>();
            ArrayList list1 = new ArrayList();
            ArrayList list2 = new ArrayList();

            XmlDocument xml = new XmlDocument();
            xml.Load(xml1);
            XmlNodeList xnList1 = xml.SelectNodes("/searchResult/section/item/@url");
            foreach (XmlNode xn1 in xnList1)
            {
                list1.Add(xn1.InnerText);
            }

            xml.Load(xml2);
            XmlNodeList xnList2 = xml.SelectNodes("/searchResult/section/item/@url");
            foreach (XmlNode xn2 in xnList2)
            {
                list2.Add(xn2.InnerText);
            }

            ArrayList missedList = new ArrayList();
            string[] st = { "", "" };

            foreach (string s in list1)
            {
                if (!list2.Contains(s))
                {
                    st[0] = s;
                    st[1] = "RegEx";
                    missedList.Add(st);
                }
            }
            foreach (string s in list2)
            {
                if (!list1.Contains(s))
                {
                    st[0] = s;
                    st[1] = "Selector";
                    missedList.Add(st);
                }
            }
            if (missedList.Count > 0)
                SaveToXml(missedList);

            al.Add(list1);
            al.Add(list2);

            return al;
        }

        private void SaveToXml(ArrayList missedList)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<searchResult searchEngine=\"" + seid + "\" keyword=\"" + System.Net.WebUtility.HtmlEncode(keyword) + "\" date=\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\" >");
            sb.Append("<section col=\"missedLinks\">");

            foreach (string[] s in missedList)
            {
                sb.Append("<item url=\"" + s[0] + "\" missedIn=\"" + s[1] + "\" />");
            }

            sb.Append("</section>");
            sb.Append("</searchResult>");

            string xmlPath = @"C:\inetpub\wwwroot\";
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(sb.ToString());
            xd.Save(xmlPath + "missing.xml");
        }

        //31-03-2021
        ArrayList GetRegExProcessedLists()
        {
            string xml2 = "C:\\inetpub\\wwwroot\\regex.xml";
            ArrayList al = new ArrayList();
            XmlDocument xml = new XmlDocument();
            xml.Load(xml2);
            XmlNodeList xnList2 = xml.SelectNodes("/searchResult/section/item/@url");
            foreach (XmlNode xn2 in xnList2)
            {
                al.Add(xn2.InnerText);
            }
            return al;
        }
        [TestMethod]
        public void TestClassicLinksList()
        {
            List<ArrayList> lst = GetProcessedLists();
            ArrayList list1 = lst[0];
            ArrayList list2 = lst[1];

            list1.Sort();
            list2.Sort();

            CollectionAssert.AreEqual(list2, list1);
        }


        [TestMethod]
        public void TestClassicLinksCount()
        {
            List<ArrayList> lst = GetProcessedLists();
            Assert.AreEqual(lst[0].Count, lst[1].Count);

            //31-03-2021
            //ArrayList lst = GetRegExProcessedLists();
            //Assert.IsTrue (lst.Count >= 95);
        }
        [TestMethod]
        public void TestDesktopSanitizeXmlString()
        {
            var desktop = new Desktop();
            string s = desktop.SanitizeXmlString("https://www.dixons.co.uk");
            Assert.AreEqual("https://www.dixons.co.uk", s);
        }
        //01-05-2021
        [TestMethod]
        public void TestDesktopGetRedirectedURL()
        {
            string url = "/aclk?url=https://google.com";
            var desktop = new Desktop();
            var result = desktop.GetRedirectedUrl(url);
            Assert.AreEqual("https://google.com", result);
        }
        [TestMethod]
        public void TestDesktopSetURL()
        {
            string url = "/aclk?url=https://google.com";
            var desktop = new Desktop();
            var result = desktop.SetUrl(url);
            Assert.AreEqual("https://google.com", result);
        }
        //01-05-2021
        //Element test methods
        [TestMethod]
        public void TestDektopAnswerCardBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("AnswerCard");//It is correct
            var desktop = new Desktop();
            var result = node != null ? desktop.GetAnswerCard(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        //03-05-2021
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestDesktopAnswerCardBlockException()
        {
            var node = SupportMethods.GetDesktopBlock("AnswerCard");
            var desktop = new Desktop();
            string s;
            desktop.html = (string)null;
            s = desktop.GetAnswerCard(node);
            Assert.AreEqual(desktop, s);

        }
        //03-05-2021 ends
        [TestMethod]
        public void TestDektopTopStoriesBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("TopStories");
            var desktop = new Desktop();
            var result = node != null ? desktop.GetTopStories(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestDektopVideosBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("Videos");
            var desktop = new Desktop();
            var result = node != null ? desktop.GetVideos(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestDesktopImagesBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("Images");
            var desktop = new Desktop();
            var result = node != null ? desktop.GetImages(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestDesktopCarouselBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("Carousel");
            var desktop = new Desktop();
            var result = node != null ? desktop.GetCarousel(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestDesktopTwitterCardsBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("TwitterCards");
            var desktop = new Desktop();
            var result = node != null ? desktop.GetTwitterCards(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestDesktopVideoCardBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("VidoeCard");
            var desktop = new Desktop();
            var result = node != null ? desktop.GetVideoCard(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestDesktopPeopleAlsoAskBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("PeopleAlsoAsk");
            var desktop = new Desktop();
            var result = node != null ? desktop.PeopleAlsoAsk(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestDesktopSiteLinksBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("SiteLinks");
            var desktop = new Desktop();
            var result = node != null ? desktop.GetSiteLinks(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestDesktopJobsBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("Jobs");
            var desktop = new Desktop();
            var result = node != null ? desktop.GetJobs(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        //27-04-2021
        [TestMethod]
        public void TestDesktopTopStuffProductListAdsExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.DesktopHtml;
            doc.LoadHtml(html);
            var desktop = new Desktop();
            var result = desktop.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"productlistedads"));
        }
        [TestMethod]
        public void TestDesktopTopStuffAdwordsExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.DesktopHtml;
            doc.LoadHtml(html);
            var desktop = new Desktop();
            var result = desktop.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"adwords"));
        }
        [TestMethod]
        public void TestDesktopTopStuffCarouselExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.DesktopHtml;
            doc.LoadHtml(html);
            var desktop = new Desktop();
            var result = desktop.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"carousel"));
        }
        [TestMethod]
        public void TestDesktopTopStuffTopStoriesExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.DesktopHtml;
            doc.LoadHtml(html);
            var desktop = new Desktop();
            var result = desktop.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"topstories"));
        }
        [TestMethod]
        public void TestDesktopTopStuffMapsExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.DesktopHtml;
            doc.LoadHtml(html);
            var desktop = new Desktop();
            var result = desktop.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"maps"));
        }
        [TestMethod]
        public void TestDesktopBottomStuffAdwordsExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.DesktopHtml;
            doc.LoadHtml(html);
            var desktop = new Desktop();
            var result = desktop.GetBottomStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"adwords"));
        }
        [TestMethod]
        public void TestDesktopRightStuffProductListAdsExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.DesktopHtml;
            doc.LoadHtml(html);
            var desktop = new Desktop();
            var result = desktop.GetRightStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"productlistedads"));
        }
        [TestMethod]
        public void TestDesktopRightStuffKnowledgGraphExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.DesktopHtml;
            doc.LoadHtml(html);
            var desktop = new Desktop();
            var result = desktop.GetRightStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"knowledgegraph"));
        }
        //27-04-2021 ends
        //28-04-2021
        [TestMethod]
        public void TestDesktopCarouselURLsExisted()
        {
            var desktop = new Desktop();
            desktop.html = SupportMethods.DesktopHtml;
            var result = !string.IsNullOrEmpty(desktop.html) ? desktop.GetCarouselURLs() : string.Empty;
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.Length > 0);
        }
        [TestMethod]
        public void TestDesktopImageURLsExisted()
        {
            var desktop = new Desktop();
            desktop.html = SupportMethods.DesktopHtml;
            var result = !string.IsNullOrEmpty(desktop.html) ? desktop.GetImageURLs() : string.Empty;
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.Length > 0);
        }
        [TestMethod]
        public void TestDesktopProcessClassicLinksExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.DesktopHtml;
            doc.LoadHtml(html);
            var desktop = new Desktop();
            var result = desktop.ProcessClassicLinks(seid, keyword, doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("<item url=\""));
        }
        //28-04-2021
    }
}
