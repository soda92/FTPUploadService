using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using FtpService;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ServerTests
{
    [TestClass]
    public class TestFtpService
    {
        public static readonly string Root = @"D:\Data\Gen";
        public static void CreateTestData()
        {
            var newRoot = Path.Join(Root, "lamp_sample");
            if (Directory.Exists(newRoot))
            {
                Directory.Delete(newRoot, true);
            }
            var file1 = Path.Join(newRoot, "folder1", "file1.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(file1));
            File.WriteAllText(file1, "test file 1\n");

            var file2 = Path.Join(newRoot, "folder2", "file2.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(file2));
            File.WriteAllText(file1, "FILE2\n");

            var file3 = Path.Join(newRoot, "file2.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(file3));
            File.WriteAllText(file1, "FILE3\n");
        }
        [TestInitialize]
        public async Task SetupAsync()
        {
            await Server.StartServerAsync();
        }
        [TestMethod]
        public void TestUploadDirs()
        {
            CreateTestData();
            Directory.CreateDirectory(TestConfig.DataLocation);
            Directory.CreateDirectory(TestConfig.ServerStartLocation);
            var data = new Config
            {
                ServerAddress = "127.0.0.1",
                Port = 2222,
                Username = "user",
                Password = "12345",
                ServerRoot = "Upload",
                LocalRoot = Root,
                HostName = "board",
                Paths = new List<PathMapping>
                {
                    new PathMapping{ src = "lamp_sample", dst = "lamp_sample" },
                },
            };

            var serverDir = Path.Join(
                TestConfig.ServerStartLocation, data.ServerRoot, data.HostName);
            Directory.CreateDirectory(serverDir);
            if (Directory.Exists(serverDir))
            {
                Directory.Delete(serverDir, true);
            }

            _ = MyClient.Upload(data);

            bool result = CompareFolder(Root, serverDir);
            Assert.IsTrue(result);
        }
        [TestCleanup]
        public async Task CleanupAsync()
        {
            await Server.StopServerAsync();
        }
        public string[] GetFolderNames(string[] paths)
        {
            return (from dirEntry in paths select new DirectoryInfo(dirEntry).Name).ToArray();
        }
        public string[] GetFileNames(string[] files)
        {
            return (from file in files select Path.GetFileName(file)).ToArray();
        }
        bool CompareFolder(string LeftSide, string RightSide)
        {
            if (!Directory.Exists(LeftSide))
            {
                return false;
            }
            if (!Directory.Exists(RightSide))
            {
                return false;
            }
            string[] leftFiles = GetFileNames(Directory.GetFiles(LeftSide));
            string[] rightFiles = GetFileNames(Directory.GetFiles(RightSide));
            if (leftFiles.Length != rightFiles.Length)
            {
                return false;
            }
            if (!Enumerable.SequenceEqual(leftFiles.OrderBy(t => t), rightFiles.OrderBy(t => t))) { return false; }
            foreach (var filename in leftFiles)
            {
                var leftFilePath = Path.Join(LeftSide, filename);
                var rightFilePath = Path.Join(RightSide, filename);
                if (!CompareContent(leftFilePath, rightFilePath))
                {
                    return false;
                }
            }

            string[] listingLeftDirs = GetFolderNames(Directory.GetDirectories(LeftSide));
            string[] listingRightDirs = GetFolderNames(Directory.GetDirectories(RightSide));
            if (listingLeftDirs.Length != listingRightDirs.Length)
            {
                return false;
            }
            if (!Enumerable.SequenceEqual(listingLeftDirs.OrderBy(t => t), listingRightDirs.OrderBy(t => t))) { return false; }
            foreach (string dir in listingRightDirs)
            {
                string name = Path.GetDirectoryName(dir);
                var newLeftSide = Path.Join(LeftSide, dir);
                var newRightSide = Path.Join(RightSide, dir);
                if (!CompareFolder(newLeftSide, newRightSide))
                {
                    return false;
                }
            }
            return true;
        }
        bool CompareContent(string left, string right)
        {
            var hash1 = TestUpload.SHA256CheckSum(left);
            var hash2 = TestUpload.SHA256CheckSum(right);
            return hash1 == hash2;
        }
    }
}
