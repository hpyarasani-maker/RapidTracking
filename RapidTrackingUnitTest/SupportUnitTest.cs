using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidTrackingLibrary;
using HtmlAgilityPack;

namespace RapidTrackingUnitTest
{
    [TestClass]
    public class SupportUnitTest //04-05-2021
    {
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

    }
}


