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
        readonly static string keyword = "joe biden";

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

        //[TestMethod]
        //public void TestDektopAnswerCardBlockExisted()
        //{
        //    var node = SupportMethods.GetDesktopAnswerCard("AnswerCard");
        //    var desktop = new Desktop();
        //    var result = node != null ? desktop.GetAnswerCard(node) : null;
        //    Assert.IsTrue(result != null && result.Length > 0);
        //}
        [TestMethod]
        public void TestDektopTopStoriesBlockExisted()
        {
            var node = SupportMethods.GetDesktopBlock("TopStories");
            var desktop = new Desktop();
            var result = node != null ? desktop.GetTopStories(node) : null;
            Assert.IsTrue(result != null && result.Length > 0);
        }
        //[TestMethod]
        //public void TestDektopVideosBlockExisted()
        //{
        //    var node = SupportMethods.GetDesktopVideos("Videos");
        //    var desktop = new Desktop();
        //    var result = node != null ? desktop.GetVideos(node) : null;
        //    Assert.IsTrue(result != null && result.Length > 0);
        //}
        //[TestMethod]
        //public void TestiOSImagesBlockExisted()
        //{
        //    var node = SupportMethods.GetiOSImages("Images");
        //    var desktop = new Desktop();
        //    var result = node != null ? desktop.GetImages(node) : null;
        //    Assert.IsTrue(result != null && result.Length > 0);
        //}
    }
}
