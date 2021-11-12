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
            string startupPath = Directory.GetCurrentDirectory();
            string iOSPath = startupPath + @"\test\htmlsrc.html";
            SupportMethods.iOsHtml = File.ReadAllText(iOSPath);
            //08-11-2021
            string desktopPath = startupPath + @"\test\desktop\htmlsrc2.html";
            SupportMethods.DesktopHtml = File.ReadAllText(desktopPath);
            //end //08-11-2021
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
            //08-11-2021
            string startupPath = Directory.GetCurrentDirectory();
            string iOSPath = startupPath + @"\test\htmlsrc3.html";
            //end 08-11-2021
            SupportMethods.iOsHtml = File.ReadAllText(iOSPath);
            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetiOSBlock("answercard");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }
        [TestMethod]
        public void TestGetiOSAppsBlock()
        {
            //08-11-2021
            string startupPath = Directory.GetCurrentDirectory();
            string iOSPath = startupPath + @"\test\htmlsrc3.html";
            SupportMethods.iOsHtml = File.ReadAllText(iOSPath);
            //end 08-11-2021
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
        [TestMethod]//27-09-2021
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
            //string startupPath = Directory.GetCurrentDirectory();

            HtmlNode htmlNode;
            htmlNode = SupportMethods.GetDesktopBlock("answercard");
            Assert.IsInstanceOfType(htmlNode, typeof(HtmlNode));
        }//end 27-09-2021
    }
}


