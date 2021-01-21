using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidTrackingLibrary;

namespace RapidTrackingUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string xml1 = "C:\\inetpub\\wwwroot\\xml1.xml";
            string xml2 = "C:\\inetpub\\wwwroot\\xml2.xml";

            ArrayList list1 = new ArrayList();
            ArrayList list2 = new ArrayList();

            XmlDocument xml = new XmlDocument();
            xml.Load(xml1); 
            xml.Load(xml2);
            XmlNodeList xnList1 = xml.SelectNodes("/searchResult/section/item/@url");
            foreach (XmlNode xn1 in xnList1)
            {
                list1.Add(xn1.InnerText);
            }
            list1.Sort();

            XmlNodeList xnList2 = xml.SelectNodes("/searchResult/section/item/@url");
            foreach (XmlNode xn2 in xnList2)
            {
                list2.Add(xn2.InnerText);
            }
            list2.Sort();


            //list1.Add("https://www.dixons.co.uk");
            //list1.Add("https://www.currys.co.uk");



            //list2.Add("https://www.dixons.co.uk");
            //list2.Add("https://www.currys.co.uk");


            CollectionAssert.AreEqual(list2,list1);

        }
    }
}
