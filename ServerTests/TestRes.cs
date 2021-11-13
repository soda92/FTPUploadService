using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ServerTests
{
    [TestClass]
    public class TestRes
    {
        [TestMethod]
        public void TestCleanup()
        {
            return;
        }
        [TestCleanup]
        public void Cleanup()
        {
            Console.WriteLine("Before");
            try
            {
                 Assert.IsTrue(true);
            }
            finally
            {
                Console.WriteLine("After");
            }
        }
    }
}
