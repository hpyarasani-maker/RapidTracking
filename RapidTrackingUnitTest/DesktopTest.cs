using System;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidTrackingLibrary;

namespace RapidTrackingUnitTest
{
    [TestClass]
    public class DesktopTest
    {
        [TestMethod]
        public void TestDektopTopStoriesBlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\topstories.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.GetTopStories(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopTwitterCardsBlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\twittercard.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.GetTwitterCards(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDektopVideosBlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\videos.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.GetVideos(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopImagesBlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\images.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.GetImages(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopPeopleAlsoAskBlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\peoplealsoask.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.PeopleAlsoAsk(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDektopAnswerCardBlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\answercard.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.GetAnswerCard(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
    }
}
