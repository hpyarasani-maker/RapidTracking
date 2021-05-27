using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using RapidTrackingLibrary;
using HtmlAgilityPack;

namespace RapidTrackingUnitTest
{
    [TestClass]
    public class iOSTest
    {
        [TestMethod]
        public void TestiOSAnswerCardBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\answercard.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetAnswerCard(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);

        }

        [TestMethod]
        public void TestiOSTopStoriesBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\ts\tsblock1.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetTopStories(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSTopStoriesBlockExisted2()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\ts\tsblock2.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetTopStories(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSAppsBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\apps\appsblock1.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetApps(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSAppsBlockExisted2()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\apps\appsblock2.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetApps(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSCarouselBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\carousel\carousel1.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetCarousel(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSCarouselBlockExisted2()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\carousel\carousel2.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetCarousel(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSTwitterCardsBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\twittercard.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetTwitterCards(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSPeopleAlsoAskBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\peoplealsoask.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.PeopleAlsoAsk(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSImagesBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\images.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetImages(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSVideosBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\videos\videosblock1.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetVideos(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSSiteLinksBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\sitelinks.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetSiteLinks(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSKnowledgeGraphExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\knowledgegraph.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetKnowledgePanel(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSJobsBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\jobs.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetJobs(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSTopStuffProductListedAdsExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\tsplads.txt";
            doc.Load(path);
            var result = iOS.GetTopStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
    }
}
