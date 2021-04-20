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
    public class UnitTest
    {
        readonly static string seid = "102";
        readonly static string keyword = "surfing videos";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Generate generate = new Generate();
            generate.GenerateXml(seid, keyword);
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
        public void TestReadConnection()
        {
            string s = Common.ReadConnection();
            Assert.AreEqual<string>("Data Source=82.136.42.2;User ID=sa;Password = brisbane007;initial catalog = TrackingTrending;", s);
        }
        [TestMethod]
        public void TestAnsweredCard()
        {
            var desktop = new Desktop();
            var doc = new HtmlDocument();
            string path = "C:\\inetpub\\wwwroot\\html\\6789935237666199553_surfing videos.html";
            doc.Load(path);
            HtmlNode node = doc.DocumentNode.SelectSingleNode(".//div[@class='yuRUbf']/a");
            string s = desktop.GetAnswerCard(node);
            Assert.AreEqual<string>(".//div[@class='yuRUbf']/a", s);
        }
        [TestMethod]
        public void TestDesktopSanitizeXmlString()
        {
            var desktop = new Desktop();
            string s = desktop.SanitizeXmlString("https://www.dixons.co.uk");
            Assert.AreEqual("https://www.dixons.co.uk", s);
        }
        [TestMethod]
        public void TestpeoplealsoaskIOS()
        {
            var ios = new iOS();
            var doc = new HtmlDocument();
            string path = "C:\\inetpub\\wwwroot\\html\\6790211090757209089_sales vacancies.html";
            doc.Load(path);
            HtmlNode node = doc.DocumentNode.SelectSingleNode(".//div[@jsname='F79BRe']");
            string s = ios.PeopleAlsoAsk(node);
            XElement newnode = XDocument.Parse(s).Root;
            string xmlpath = "C:\\inetpub\\wwwroot\\html\\selector.xml";
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlpath);
            XmlNodeList xnList1 = xml.SelectNodes("/searchResult/section/block/item/@title");
            if (xnList1.Count > 0)
            {
                foreach (XmlNode xn1 in xnList1)
                {
                    if (newnode.LastAttribute.Value == xn1.InnerText)
                    {
                        Assert.AreEqual<string>(newnode.LastAttribute.Value, xn1.InnerText);
                        return;
                    }
                }
                Assert.Fail();
            }
            else
                Assert.IsTrue(xnList1.Count > 0);
        }
    }
}
