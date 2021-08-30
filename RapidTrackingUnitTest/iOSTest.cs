using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using RapidTrackingLibrary;
using HtmlAgilityPack;
using System.IO;
using System.Collections;

namespace RapidTrackingUnitTest
{
    [TestClass]
    public class iOSTest
    {
        readonly static string seid = "106";
        readonly static string keyword = "movies";
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
        [TestMethod]
        public void TestiOSTopStuffAdwordsExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\tsadwords.txt";
            doc.Load(path);
            var result = iOS.GetTopStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSCarouselURLsExisted()
        {
            var html = @"C:\inetpub\wwwroot\test\htmlsrc.html";
            var ios = new iOS();
            using (StreamReader sr = new StreamReader(html))
            {
                ios.html = sr.ReadToEnd();
            }
            var result = ios.GetCarouselURLs(new ArrayList());
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSImageURLsExisted()
        {
            var html = @"C:\inetpub\wwwroot\test\htmlsrc.html";
            var ios = new iOS();
            using (StreamReader sr = new StreamReader(html))
            {
                ios.html = sr.ReadToEnd();
            }
            var result = ios.GetImageURLs();
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.Length > 0);
        }
        [TestMethod]
        public void TestiOSProcessClassicLinksExisted()
        {
            var html = @"C:\inetpub\wwwroot\test\htmlsrc.html";
            var doc = new HtmlDocument();
            doc.Load(html);
            var ios = new iOS();
            var result = ios.ProcessClassicLinks(seid, keyword, doc);
            Assert.IsTrue(!string.IsNullOrEmpty(result) && result.ToLower().Contains("<item url=\""));
        }
        [TestMethod]
        public void TestiOSVideoCardBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\videocard.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetVideoCard(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSIsBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\twittercard.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.IsBlock(node);
            Assert.AreEqual(result, true);
        }
        [TestMethod]
        public void TestiOSIsBlockNotExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\classiclink.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.IsBlock(node);
            Assert.AreEqual(result, false);
        }
        [TestMethod]
        public void TestiOSIsOrganicBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\classiclink.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.IsOrganic(node);
            Assert.AreEqual(result, true);
        }
        [TestMethod]
        public void TestiOSProductListedAdsBlockExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\productlistedads.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.ProductListedAds(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSSiteLinksBlock2Existed()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\sitelinks1.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetSiteLinks(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSNoBlockTypeExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("<div><div>Some junk text</div></div>");
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.ProcessBlock(node);
            Assert.AreEqual(string.IsNullOrEmpty(result), true);
        }
        //04-08-2021 Block type code coverage methods
        [TestMethod]
        public void TestiOSTSBlockTypeExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\topstories.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.ProcessBlock(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSVideosBlockTypeExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\videos.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.ProcessBlock(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSTwitterBlockTypeExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\twittercard.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.ProcessBlock(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSAnswerCardBlockTypeExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\answercard.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.ProcessBlock(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSPeopleAlsoAskBlockTypeExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\peoplealsoask.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.ProcessBlock(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSImagesBlockTypeExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\images.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.ProcessBlock(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSSitelinksBlockTypeExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\sitelinks.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.ProcessBlock(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSJobsBlockTypeExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\jobs.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.ProcessBlock(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSKnowledgepanelBlockTypeExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\knowledgegraph.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.ProcessBlock(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        //13-08-2021
        [TestMethod]
        public void TestiOSGetBottomStuffProductListedAdsExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\bsplads1.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.GetBottomStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSBottomStuffAdwordsBlockExisted1()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\BottomStuffAdwords1.txt";
            doc.Load(path);
            var result = ios.GetBottomStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSBottomStuffAdwordsBlockExisted2()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\BottomStuffAdwords2.txt";
            doc.Load(path);
            var result = ios.GetBottomStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSBottomStuffAdwordsBlockExisted3()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\BottomStuffAdwords3.txt";
            doc.Load(path);
            var result = ios.GetBottomStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSTopStuffAdwordsBlockExisted1()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\TopStuffAdwords1.txt";
            doc.Load(path);
            var result = ios.GetTopStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSTopStuffAdwordsBlockExisted2()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\TopStuffAdwords2.txt";
            doc.Load(path);
            var result = ios.GetTopStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSTopStuffAdwordsBlockExisted3()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\TopStuffAdwords3.txt";
            doc.Load(path);
            var result = ios.GetTopStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }

        [TestMethod]
        public void TestiOSTopStuffAnswercardBlockExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\TopStuffAnswercard.txt";
            doc.Load(path);
            var result = ios.GetTopStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        //end 13-08-2021
        //26-08-2021
        [TestMethod]
        public void TestiOSImagesBlockExisted2()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\images2.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetImages(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSProcessNodes1()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\answercard.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.ProcessNode(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSProcessNodes2()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\classiclink.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.ProcessNode(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSCarouselBlockExisted3()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\carousel\carousel3.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetCarousel(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        //end 26-08-2021
        [TestMethod] //27-08-2021
        public void TestiOSGetProductListedUrls1()
        {
            var iOS = new iOS();
            string url = @"http://www.google.com";
            var result = iOS.GetProductListedUrls(url);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSGetProductListedUrls2()
        {
            var iOS = new iOS();
            string url = @"www.google.com";
            var result = iOS.GetProductListedUrls(url);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSGetProductListedUrls3()
        {
            var iOS = new iOS();
            string url = @"google.com";
            var result = iOS.GetProductListedUrls(url);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSGetProductListedUrls4()
        {
            var iOS = new iOS();
            string url = @"http://google.com";
            var result = iOS.GetProductListedUrls(url);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        //end 27-08-2021
        //30-08-2021
        [TestMethod]
        public void TestiOSTopStoriesBlockExisted3()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();

            string path = @"C:\inetpub\wwwroot\test\ts\tsblock3.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.GetTopStories(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSMapsBlockTypeExisted()
        {
            var ios = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\maps.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = ios.ProcessBlock(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSvideoBlock1Existed()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\videoblock1.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.ProcessNode(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSvideoBlock2Existed()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\videoblock2.txt";
            doc.Load(path);
            var node = doc.DocumentNode.SelectSingleNode("/");
            var result = iOS.ProcessNode(node);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }
        [TestMethod]
        public void TestiOSTopStuffknowledgeGraphExisted()
        {
            var iOS = new iOS();
            HtmlDocument doc = new HtmlDocument();
            string path = @"C:\inetpub\wwwroot\test\tsknowledgeGraph.txt";
            doc.Load(path);
            var result = iOS.GetTopStuff(doc);
            Assert.AreEqual(!string.IsNullOrEmpty(result), true);
        }//end 30-08-2021

    }
}
