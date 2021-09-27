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
        [TestMethod] //22-09-2021
        public void TestGetiOSVideosBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetiOSBlock("videos");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }
        [TestMethod]
        public void TestGetiOSPeopleAlsoAskBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetiOSBlock("peoplealsoask");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }
        [TestMethod]
        public void TestGetiOSCarouselBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetiOSBlock("carousel");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }
        [TestMethod]
        public void TestGetiOSAnswerCardBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetiOSBlock("answercard");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }
        [TestMethod]
        public void TestGetiOSAppsBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetiOSBlock("apps");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }//end 22-09-2021

        [TestMethod] //24-09-2021
        public void TestGetDesktopKnowledgegraphBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetDesktopBlock("knowledgegraph");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }//end 24-09-2021

        [TestMethod]
        public void TestGetDesktopVideosBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetDesktopBlock("videos");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }
        [TestMethod]
        public void TestGetDesktopImagesBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetDesktopBlock("images");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }
        [TestMethod]
        public void TestGetDesktopCarouselBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetDesktopBlock("carousel");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }
        [TestMethod]
        public void TestGetDesktopAnswerCardBlock()
        {
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetDesktopBlock("answercard");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }
    }
}


