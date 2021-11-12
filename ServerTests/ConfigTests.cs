using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using FtpService;

namespace ConfigTestNamespace
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        public void testconfig()
        {
            Console.WriteLine(MyConfig.GetConfigFullPath());
        }
        [TestMethod]
        public async Task testreadconf()
        {
            await MyConfig.ReadConfig();
        }
    }
}