using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ServerTests
{
    [TestClass]
    // TestEqualData
    // TestHalfData
    // TestHalfData_ServerHasError
    // TestDifferentData_ServerBigger
    // TestTransfer_BreakForTwoMinutes
    public class ServerTests
    {
        [TestMethod]
        public void TestEqualData()
        {
            // Create Data in TEMP directory
            Path path = Path.Join(Path.GetTempPath(), "programTesting", "test1");
            DataGeneration.GenreateDataEquals(path, "file.bin", "local", "remote", "500MB");

            // Start Server
            server = FTPServer.Server(Path.Join(path, "remote"), "127.0.0.1", 2222);
            server.StartAsync(CancellationToken.None).Wait();

            DateTime start = System.DateTime.Now;

            // Upload
            await FTPUploadService.UploadAsync(Path.Join(path, "local"),
                                     "file.bin", "127.0.0.1", 2222, "/");

            DateTime end = System.DateTime.Now;
            TimeSpan period = end - start;

            // Compare hash
            string hashLocal = FileComparation.GetSha256("local/file.bin");
            string hashRemote = FileComparation.GetSha256("remote/file.bin");
            try
            {
                AssertEquals(hashLocal, hashRemote);
            }
            finally
            {
                Console.WriteLine(string.Format("Upload cost {0} seconds.", period.TotalSeconds));
                // Clean up
                server.StopAsync(CancellationToken.None).Wait();
                RemovePathRecursive(Path.Join(Path.GetTempPath(), "programTesting"));
            }
        }
    }
}
