using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidTrackingLibrary;

namespace RapidTrackingUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ArrayList list1 = new ArrayList();
            ArrayList list2 = new ArrayList();

            list1.Add("https://www.dixons.co.uk");
            list1.Add("https://www.currys.co.uk");
            list1.Sort();

            
            list2.Add("https://www.dixons.co.uk");
            list2.Add("https://www.currys.co.uk");
            list2.Sort();
            CollectionAssert.AreEqual(list2,list1);

        }
    }
}
