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
        public static readonly string FileName = "DATA";
        public static readonly string FilePath = Path.Combine(TestConfig.DataLocation, FileName);
        public static readonly string RemotePath = Path.Join(TestConfig.ServerStartLocation, FileName);
        public static int DataSizeInMB = 20;
        public FtpClient client;
        [TestInitialize]
        public async Task Setup()
        {
            Directory.CreateDirectory(TestConfig.DataLocation);
            Directory.CreateDirectory(TestConfig.ServerStartLocation);
            await Server.StartServerAsync();
            var config = MyConfig.GetExampleConfig();
            client = new FtpClient(
                config.ServerAddress, config.Port, config.Username, config.Password);

            await client.ConnectAsync();
            client.RetryAttempts = 3;
            CreateFileWithSizeMB(DataSizeInMB);
        }
        public static void CreateFileWithSizeMB(int size)
        {
            using FileStream fileStream = File.Create(FilePath);
            for (int i = 0; i < size; ++i)
            {
                byte[] buffer = new byte[1024 * 1024];
                fileStream.Write(buffer);
            }
        }
        public static string SHA256CheckSum(string filePath)
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
            DateTime start = DateTime.Now;
            FtpTrace.EnableTracing = true;
            FtpTrace.LogToConsole = true;

            client.UploadFile(FilePath, FileName, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);

            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }
        [TestMethod]
        public void TestSkipping()
        {
            if (File.Exists(RemotePath))
            {
                File.Delete(RemotePath);
            }
            File.Copy(FilePath, RemotePath);

            DateTime start = DateTime.Now;
            client.UploadFile(FilePath, FileName, FtpRemoteExists.Skip, true, FtpVerify.Throw);

            DateTime end = DateTime.Now;
            Assert.IsTrue(File.Exists(RemotePath));
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }
        [TestMethod]
        public void TestHalf()
        {
            using FileStream fileStream = File.OpenRead(FilePath);
            using FileStream fileStream1 = File.Create(RemotePath);
            for (int i = 0; i < DataSizeInMB / 2; i++)
            {
                byte[] data = new byte[1024 * 1024];
                fileStream.Read(data);
                fileStream1.Write(data);
            }
            fileStream.Close();
            fileStream1.Close();

            long size = client.GetFileSize(FileName);
            var attributes = new FileInfo(FilePath);
            long local_size = attributes.Length;

            DateTime start = DateTime.Now;
            if (size != local_size)
            {
                client.UploadFile(FilePath, FileName, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);
            }

            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }

        [TestMethod]
        public void TestHalfWithRetryLengthNotEqual()
        {
            using FileStream fileStream = File.OpenRead(FilePath);
            using FileStream fileStream1 = File.Create(RemotePath);
            for (int i = 0; i < DataSizeInMB / 2; i++)
            {
                byte[] data = new byte[1024 * 1024];
                fileStream.Read(data);
                fileStream1.Write(data);
            }
            fileStream.Close();
            fileStream1.Close();

            DateTime start = DateTime.Now;

            client.RetryAttempts = 3;
            FtpCompareResult result = client.CompareFile(FilePath, RemotePath);

            if (result != FtpCompareResult.Equal)
            {
                client.UploadFile(FilePath, FileName, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);
            }

            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }

        [TestMethod]
        public void TestHalfWithRetryLengthEqualDataNotEqual()
        {
            using FileStream fileStream1 = File.Create(RemotePath);
            for (int i = 0; i < DataSizeInMB; i++)
            {
                byte[] data = new byte[1024 * 1024];
                fileStream1.Write(data);
            }
            fileStream1.Close();

            DateTime start = DateTime.Now;

            client.RetryAttempts = 3;
            FtpCompareResult result = client.CompareFile(FilePath, RemotePath);

            if (result != FtpCompareResult.Equal)
            {
                client.UploadFile(FilePath, FileName, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);
            }

            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            string checksum_origin = SHA256CheckSum(FilePath);
            string checksum_remote = SHA256CheckSum(RemotePath);
            try
            {
                Assert.IsTrue(client.FileExists(Path.Join("/", FileName)));
                Assert.AreEqual(checksum_origin, checksum_remote);
                Assert.IsTrue(File.Exists(RemotePath));
            }
            finally
            {
                if (File.Exists(RemotePath))
                {
                    File.Delete(RemotePath);
                }
                await client.DisconnectAsync();
                await Server.StopServerAsync();
            }
        }
    }
}
