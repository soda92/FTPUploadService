using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FtpService;
using FluentFTP;
using FluentFTP.Helpers;
using System.Security.Cryptography;

namespace ServerTests
{
    [TestClass]
    public class TestUpload
    {
        public static string filename = "DATA";
        public static string file_path = Path.Combine(TestConfig.data_location, filename);
        public static string remote_path = Path.Join(TestConfig.server_start_location, filename);
        public FtpClient client;
        [TestInitialize]
        public async Task Setup()
        {
            Directory.CreateDirectory(TestConfig.data_location);
            Directory.CreateDirectory(TestConfig.server_start_location);
            await Server.StartServerAsync();
            client = await MyClient.Get();
            await client.ConnectAsync();
        }
        public static void CreateFileWithSizeMB(int size)
        {
            using FileStream fileStream = File.Create(file_path);
            for (int i = 0; i < size; ++i)
            {
                byte[] buffer = new byte[1024 * 1024];
                fileStream.Write(buffer);
            }
        }
        public string SHA256CheckSum(string filePath)
        {
            using (SHA256 SHA256 = SHA256.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                    return Convert.ToBase64String(SHA256.ComputeHash(fileStream));
            }
        }
        [TestMethod]
        public void TestUploadFile()
        {
            CreateFileWithSizeMB(512);

            DateTime start = DateTime.Now;
            FtpTrace.EnableTracing = true;

            client.UploadFile(file_path, filename, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);

            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));

            Assert.IsTrue(client.FileExists(Path.Join("/", filename)));
            string checksum_origin = SHA256CheckSum(file_path);
            string checksum_remote = SHA256CheckSum(remote_path);
            Assert.AreEqual(checksum_origin, checksum_remote);

            File.Delete(remote_path);
        }
        [TestCleanup]
        public async Task Cleanup()
        {
            await client.DisconnectAsync();
            await Server.StopServerAsync();
        }
    }
}
