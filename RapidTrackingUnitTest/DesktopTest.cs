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
        [TestMethod]
        public void TestDesktopTopStuffProductListAdsExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\tsproductlistedads.txt";
            doc.Load(path);
            var result = desktop.GetTopStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopTopStuffAdwordsExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\textads.txt";
            doc.Load(path);
            var result = desktop.GetTopStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopSiteLinksBlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\sitelinks.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.GetSiteLinks(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopCarousel1BlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\carousel1.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.GetCarousel(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopCarousel2BlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\carousel2.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.GetCarousel(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopTopStuffCarouselExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\tscarousel.txt";
            doc.Load(path);
            var result = desktop.GetTopStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopJobsBlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\jobs.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.GetJobs(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopRightStuffProductListAdsExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\rightproductslistedads.txt";
            doc.Load(path);
            var result = desktop.GetRightStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestDesktopVideoCardBlockExisted()
        {
            var desktop = new Desktop();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\desktop\videocard.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = desktop.GetVideoCard(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
    }
}
