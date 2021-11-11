using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MyConfigNamespace;
using System.Threading.Tasks;

namespace ConfigTestNamespace
{
    [TestClass]
    public class ConfigTest
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