using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServerTests
{
    [TestClass]
    public class TestSize
    {
        [TestMethod]
        public void TestGetDirectories()
        {
            var dirs = Directory.GetDirectories(@"D:\code");
            foreach (var dir in dirs)
            {
                Console.WriteLine(dir);
            }
        }
        [TestMethod]
        public void TestGetFiles()
        {
            var dirs = Directory.GetFiles(@"D:\code\matlab_test");
            foreach (var dir in dirs)
            {
                Console.WriteLine(dir);
            }
        }
    }
}
