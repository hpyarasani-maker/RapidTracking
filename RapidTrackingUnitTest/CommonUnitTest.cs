using System;
using System.IO;
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
        [TestMethod] //27-09-2021
        [ExpectedException(typeof(FileNotFoundException), "could not found file")]
        public void TestReadConnectionFile()
        {
            string path = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";
            var s = Common.ReadConnection();
            if (File.Exists(path))
            {
                Assert.IsTrue(true, s);
            }
            else
            {
                Assert.ThrowsException<FileNotFoundException>(() => s);
            }
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
