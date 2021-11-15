using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using FtpService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServerTests
{
    public class TestConfig
    {
        private static readonly string RootFolder = @"D:\Data";
        public static readonly string ServerStartLocation = Path.Join(RootFolder, "server");
        public static readonly string DataLocation = Path.Join(RootFolder, "data");
    }

    [TestClass]
    public class TestConf
    {
        [TestMethod]
        public void GetConfig()
        {
            MyConfig.DeleteConfig();
            var data = MyConfig.GetExampleConfig();
            var json = MyConfig.GetSerializedData(data);
            Console.WriteLine(json);
        }
        //[TestMethod]
        //public 
    }
}
