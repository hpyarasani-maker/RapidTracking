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
    public class iOSUnitTest
    {
        readonly static string seid = "102";
        readonly static string keyword = "movies";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Generate generate = new Generate();
            //generate.GenerateXml(seid, keyword);
            SupportMethods.iOsHtml = generate.GenerateXml(seid, keyword);
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
                if(!list2.Contains(s))
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
                
            foreach (string[] s in missedList ) 
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
        public void TestiOSSanitizeXmlString()
        {
            var ios = new iOS();
            string s = ios.SanitizeXmlString("https://www.dixons.co.uk");
            Assert.AreEqual("https://www.dixons.co.uk", s);
        }
        //01-05-2021
        [TestMethod]
        public void TestiOSGetRedirectedURL()
        {
            string url = "/aclk?url=https://google.com";
            var iOS = new iOS();
            var result = iOS.GetRedirectedUrl(url);
            Assert.AreEqual("https://google.com", result);
        }
        [TestMethod]
        public void TestiOSSetURL()
        {
            string url = "/aclk?url=https://google.com";
            var iOS = new iOS();
            var result = iOS.SetUrl(url);
            Assert.AreEqual("https://google.com", result);
        }
        //01-05-2021
        //Element test methods
        // iOS Tests
        [TestMethod]
        public void TestiOSAnswerCardBlockExisted()
        {
            var node = SupportMethods.GetiOSBlock("AnswerCard");
            var iOS = new iOS();
            var result = node != null ? iOS.GetAnswerCard(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        //03-05-2021
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSAnswerCardBlockException()
        {
            var node = SupportMethods.GetiOSBlock("AnswerCard");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetAnswerCard(node);
            Assert.AreEqual(iOS, s);

        }
        //03-05-2021 ends
        [TestMethod]
        public void TestiOSTopStoriesBlockExisted()
        {
            var node = SupportMethods.GetiOSBlock("TopStories");
            var iOS = new iOS();
            var result = node != null ? iOS.GetTopStories(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSVideosBlockExisted()
        {
            var node = SupportMethods.GetiOSBlock("Videos");
            var iOS = new iOS();
            var result = node != null ? iOS.GetVideos(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSImagesBlockExisted()
        {
            var node = SupportMethods.GetiOSBlock("Images");
            var iOS = new iOS();
            var result = node != null ? iOS.GetImages(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSCarouselBlockExisted()
        {
            var node = SupportMethods.GetiOSBlock("Carousel");
            var iOS = new iOS();
            var result = node != null ? iOS.GetCarousel(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSTwitterCardsBlockExisted()
        {
            var node = SupportMethods.GetiOSBlock("TwitterCards");
            var iOS = new iOS();
            var result = node != null ? iOS.GetTwitterCards(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSVideoCardBlockExisted()
        {
            var node = SupportMethods.GetiOSBlock("VidoeCard");
            var iOS = new iOS();
            var result = node != null ? iOS.GetVideoCard(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSPeopleAlsoAskBlockExisted()
        {
            var node = SupportMethods.GetiOSBlock("PeopleAlsoAsk");
            var iOS = new iOS();
            var result = node != null ? iOS.PeopleAlsoAsk(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSSiteLinksBlockExisted()
        {
            var node = SupportMethods.GetiOSBlock("SiteLinks");
            var iOS = new iOS();
            var result = node != null ? iOS.GetSiteLinks(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSJobsBlockExisted()
        {
            var node = SupportMethods.GetiOSBlock("Jobs");
            var iOS = new iOS();
            var result = node != null ? iOS.GetJobs(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        //28-04-2021
        [TestMethod]
        public void TestiOSTopStuffCarouselExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.iOsHtml;
            doc.LoadHtml(html);
            var ios = new iOS();
            var result = ios.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"carousel"));
        }
        [TestMethod]
        public void TestiOSTopStuffAppsExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.iOsHtml;
            doc.LoadHtml(html);
            var ios = new iOS();
            var result = ios.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"apps"));
        }
        [TestMethod]
        public void TestiOSTopStuffProductListedAdsExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.iOsHtml;
            doc.LoadHtml(html);
            var ios = new iOS();
            var result = ios.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"productlistedads"));
        }
        [TestMethod]
        public void TestiOSTopStuffAdwordsExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.iOsHtml;
            doc.LoadHtml(html);
            var ios = new iOS();
            var result = ios.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"adwords"));
        }
        [TestMethod]
        public void TestiOSTopStuffVideoCardExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.iOsHtml;
            doc.LoadHtml(html);
            var ios = new iOS();
            var result = ios.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"videocard"));
        }
        [TestMethod]
        public void TestiOSTopStuffAnswerCardExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.iOsHtml;
            doc.LoadHtml(html);
            var ios = new iOS();
            var result = ios.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"answercard"));
        }
        [TestMethod]
        public void TestiOSTopStuffKnowledgeGraphExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.iOsHtml;
            doc.LoadHtml(html);
            var ios = new iOS();
            var result = ios.GetTopStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"knowledgegraph"));
        }
        [TestMethod]
        public void TestiOSBottomStuffProductListedAdsExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.iOsHtml;
            doc.LoadHtml(html);
            var ios = new iOS();
            var result = ios.GetBottomStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"productlistedads"));
        }
        [TestMethod]
        public void TestiOSBottomStuffAdwordsExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.iOsHtml;
            doc.LoadHtml(html);
            var ios = new iOS();
            var result = ios.GetBottomStuff(doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("type=\"adwords"));
        }
        [TestMethod]
        public void TestiOSProcessClassicLinksExisted()
        {
            var doc = new HtmlDocument();
            var html = SupportMethods.iOsHtml;
            doc.LoadHtml(html);
            var ios = new iOS();
            var result = ios.ProcessClassicLinks(seid, keyword, doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("<item url=\""));
        }
        [TestMethod]
        public void TestiOSCarouselURLsExisted()
        {
            var ios = new iOS();
            ios.html = SupportMethods.iOsHtml;
            var result = !string.IsNullOrEmpty(ios.html) ? ios.GetCarouselURLs(new ArrayList()) : string.Empty;
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSImageURLsExisted()
        {
            var ios = new iOS();
            ios.html = SupportMethods.iOsHtml;
            var result = !string.IsNullOrEmpty(ios.html) ? ios.GetImageURLs() : string.Empty;
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.Length > 0);
        }
        //28-04-2021 ends
        //04-05-2021 Exception
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSTopstoriesException()
        {
            var node = SupportMethods.GetiOSBlock("topstories");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetTopStories(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSVideosException()
        {
            var node = SupportMethods.GetiOSBlock("videos");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetVideos(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSSiteLinksException()
        {
            var node = SupportMethods.GetiOSBlock("sitelinks");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetSiteLinks(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSImagesException()
        {
            var node = SupportMethods.GetiOSBlock("images");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetImages(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSTwittersException()
        {
            var node = SupportMethods.GetiOSBlock("twitters");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetTwitterCards(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSPeoplealsoaskException()
        {
            var node = SupportMethods.GetiOSBlock("peoplealsoask");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.PeopleAlsoAsk(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSproductlistedadsException()
        {
            var node = SupportMethods.GetiOSBlock("productlistedads");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.ProductListedAds(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSCarouselException()
        {
            var node = SupportMethods.GetiOSBlock("carousel");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetCarousel(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSKnowledgePanelException()
        {
            var node = SupportMethods.GetiOSBlock("knowledgepanel");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetKnowledgePanel(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSvideoCardException()
        {
            var node = SupportMethods.GetiOSBlock("videocard");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetVideoCard(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSAppsException()
        {
            var node = SupportMethods.GetiOSBlock("apps");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetApps(node);
            Assert.AreEqual(iOS, s);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestiOSJobsException()
        {
            var node = SupportMethods.GetiOSBlock("jobs");
            var iOS = new iOS();
            string s;
            iOS.html = (string)null;
            s = iOS.GetJobs(node);
            Assert.AreEqual(iOS, s);
        }

        //end 04-05-2021
    }
}
