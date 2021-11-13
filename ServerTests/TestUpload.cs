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
        public static int datasize_mb = 1024;
        public FtpClient client;
        [TestInitialize]
        public async Task Setup()
        {
            Directory.CreateDirectory(TestConfig.data_location);
            Directory.CreateDirectory(TestConfig.server_start_location);
            await Server.StartServerAsync();
            client = await MyClient.Get();
            await client.ConnectAsync();

            CreateFileWithSizeMB(datasize_mb);
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
            DateTime start = DateTime.Now;
            FtpTrace.EnableTracing = true;
            FtpTrace.LogToConsole = true;

            client.UploadFile(file_path, filename, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);

            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }
        [TestMethod]
        public void TestSkipping()
        {
            if (File.Exists(remote_path))
            {
                File.Delete(remote_path);
            }
            File.Copy(file_path, remote_path);

            DateTime start = DateTime.Now;
            client.UploadFile(file_path, filename, FtpRemoteExists.Skip, true, FtpVerify.Throw);

            DateTime end = DateTime.Now;
            Assert.IsTrue(File.Exists(remote_path));
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }
        [TestMethod]
        public void TestHalf()
        {
            using FileStream fileStream = File.OpenRead(file_path);
            using FileStream fileStream1 = File.Create(remote_path);
            for (int i = 0; i < datasize_mb / 2; i++)
            {
                byte[] data = new byte[1024 * 1024];
                fileStream.Read(data);
                fileStream1.Write(data);
            }
            fileStream.Close();
            fileStream1.Close();

            long size = client.GetFileSize(filename);
            var attributes = new FileInfo(file_path);
            long local_size = attributes.Length;

            DateTime start = DateTime.Now;
            if (size != local_size)
            {
                client.UploadFile(file_path, filename, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);
            }

            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }

        [TestMethod]
        public void TestHalfWithRetryLengthNotEqual()
        {
            using FileStream fileStream = File.OpenRead(file_path);
            using FileStream fileStream1 = File.Create(remote_path);
            for (int i = 0; i < datasize_mb / 2; i++)
            {
                byte[] data = new byte[1024 * 1024];
                fileStream.Read(data);
                fileStream1.Write(data);
            }
            fileStream.Close();
            fileStream1.Close();

            DateTime start = DateTime.Now;

            client.RetryAttempts = 3;
            FtpCompareResult result = client.CompareFile(file_path, remote_path);

            if (result != FtpCompareResult.Equal)
            {
                client.UploadFile(file_path, filename, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);
            }

            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }

        [TestMethod]
        public void TestHalfWithRetryLengthEqualDataNotEqual()
        {
            using FileStream fileStream1 = File.Create(remote_path);
            for (int i = 0; i < datasize_mb; i++)
            {
                byte[] data = new byte[1024 * 1024];
                fileStream1.Write(data);
            }
            fileStream1.Close();

            DateTime start = DateTime.Now;

            client.RetryAttempts = 3;
            FtpCompareResult result = client.CompareFile(file_path, remote_path);

            if (result != FtpCompareResult.Equal)
            {
                client.UploadFile(file_path, filename, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);
            }

            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }

        [TestMethod]
        public void TestTimeout()
        {

        }

        [TestCleanup]
        public async Task Cleanup()
        {
            string checksum_origin = SHA256CheckSum(file_path);
            string checksum_remote = SHA256CheckSum(remote_path);
            try
            {
                Assert.IsTrue(client.FileExists(Path.Join("/", filename)));
                Assert.AreEqual(checksum_origin, checksum_remote);
                Assert.IsTrue(File.Exists(remote_path));
            }
            finally
            {
                if (File.Exists(remote_path))
                {
                    File.Delete(remote_path);
                }
                await client.DisconnectAsync();
                await Server.StopServerAsync();
            }
        }
    }
}
