using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidTrackingLibrary;
namespace RapidTrackingUnitTest
{
    [TestClass]
    public class SourceUnitTest
    {
        /*[TestMethod]
        [PexGeneratedBy(typeof(SourceUnitTest))]
       
        public void TestGetHTMLException()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                Task<ArrayList> task;
                task = source.GetHTML((string)null, 0);
                disposables.Add((IDisposable)task);
                //disposables.Dispose();
                Assert.IsNotNull((object)task);
                Assert.AreEqual<TaskStatus>(TaskStatus.Faulted, ((Task)task).Status);
                Assert.AreEqual<bool>(false, ((Task)task).IsCanceled);
                Assert.IsNull(((Task)task).AsyncState);
                Assert.AreEqual<bool>(true, ((Task)task).IsFaulted);
            }
        }
        [TestMethod] //04-05-2021
        public void TestGetOxylabsWebDataSourcesException()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                Task<ArrayList> task;
                task = source.GetOxylabsWebDataSources((SearchProperties)null);
                disposables.Add((IDisposable)task);
                //disposables.Dispose();
                Assert.IsNotNull((object)task);
                Assert.AreEqual<TaskStatus>(TaskStatus.Faulted, ((Task)task).Status);
                Assert.AreEqual<bool>(false, ((Task)task).IsCanceled);
                Assert.IsNull(((Task)task).AsyncState);
                Assert.AreEqual<bool>(true, ((Task)task).IsFaulted);
            }
        }*/
        [TestMethod]
        public void TestGetOxylabsWebDataSources()
        {
            var src = source.GetHTML("testing", 1);
            Assert.AreEqual(src.Count, 1);
        }
    }
}

