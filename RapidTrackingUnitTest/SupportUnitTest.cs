using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidTrackingLibrary;
using HtmlAgilityPack;
using System.IO;

namespace RapidTrackingUnitTest
{
   
    
    [TestClass]
    public class SupportUnitTest //04-05-2021
    {
        [ClassInitialize] //21-09-2021
        public static void Initialize(TestContext context)
        {
            string iOSPath = "C:\\inetpub\\wwwroot\\test\\htmlsrc.html";
            SupportMethods.iOsHtml = File.ReadAllText(iOSPath);
        }//end 21-09-2021
        [TestMethod]
        public void TestGetiOSBlockException()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetiOSBlock((string)null);
            Assert.IsNull((object)htmlNode);
        }
        [TestMethod]
        public void TestGetDesktopBlockException()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetDesktopBlock((string)null);
            Assert.IsNull((object)htmlNode);
        }
        [TestMethod]//21-09-2021
        public void TestGetiOSTopstoriesBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetiOSBlock("topstories");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }
        [TestMethod]
        public void TestGetiOSImagesBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetiOSBlock("images");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }//end 21-09-2021

    }
}


