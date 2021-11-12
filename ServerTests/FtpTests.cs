using System;
using System.Text;
using FluentFTP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FtpService;
using System.Threading.Tasks;

namespace ServerTests
{
    [TestClass]
    public class FtpTests
    {
        FtpClient client;
        [TestInitialize]
        public async Task Connect()
        {
            await Server.StartServerAsync();
            DataModel config = await MyConfig.ReadConfig();
            client = new FtpClient(
                config.server_address, config.port, config.username, config.password);
            await client.ConnectAsync();
        }
        [TestMethod]
        public void createDirectoryTest()
        {
            if (!client.DirectoryExists("test"))
            {
                client.CreateDirectory("test");
            }
            foreach(FtpListItem item in client.GetListing())
            {
                if(item.Type == FtpFileSystemObjectType.Directory)
                {
                    Console.Write("Directory: ");
                }
                else if(item.Type == FtpFileSystemObjectType.File)
                {
                    Console.Write("File: ");
                }
                Console.WriteLine(item.FullName);
            }
        }
        [TestMethod]
        public void testCreateFile()
        {
            string Data = "Hello World!\n";
            byte[] data = Encoding.UTF8.GetBytes(Data);
            client.Upload(data, "/test2/file.txt", FtpRemoteExists.AddToEnd, true);
        }
        [TestCleanup]
        public async Task Cleanup()
        {
            await client.DisconnectAsync();
            await Server.StopServerAsync();
        }
    }
}
