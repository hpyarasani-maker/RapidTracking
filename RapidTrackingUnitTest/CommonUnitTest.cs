using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidTrackingLibrary;
namespace RapidTrackingUnitTest
{
    [TestClass]
    public class CommonUnitTest
    {
        [TestMethod]
        public void TestReadConnection()
        {
            string s = Common.ReadConnection();
            Assert.AreEqual<string>("Data Source=10.2.0.4;User ID=sa;Password = Pi*Soft1234;initial catalog = TrackingTrending;", s);
        }

        [TestMethod]
        public void TestGetOxylabsTime()
        {
            var s = Common.GetOxylabsTime();
            Assert.AreEqual(450, s);
        }
        [TestMethod]
        public void TestGetOxylabsCount()
        {
            var s = Common.GetOxylabsCount();
            Assert.AreEqual(20, s);
        }
       
    }
}
