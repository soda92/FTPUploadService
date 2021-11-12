using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServerTests
{
    [TestClass]
    // TestUploadData
    // TestHaveEqualData
    // TestHaveHalfData
    // TestHaveHalfErrorData

    // TestDifferentData_ServerBigger

    // TestTransfer_BreakForTwoMinutes
    public class ServerTests
    {
        /*
        [TestMethod]

        public void TestEqualData()
        {
            MyLogger myLogger = MyLogger.Get("D:/logs/ftpserver/log.log");

            // Create Data in TEMP directory
            MyPath path = MyPath.Create("test1");
            DataGeneration.GenreateDataEquals("file.bin", "500MB", path.GetLocal(), path.GetRemote());

            // Start Server
            server = FTPServer.Server(path.GetRemote(), "127.0.0.1", 2222);
            server.start();

            DateTime start = System.DateTime.Now;

            // Upload
            FtpService.Upload(path.GetLocalFile("file.bin"), "127.0.0.1", 2222);

            DateTime end = System.DateTime.Now;
            TimeSpan period = end - start;

            // Compare hash
            string hashLocal = FileComparation.GetSha256(path.GetLocalFile("file.bin"));
            string hashRemote = FileComparation.GetSha256(path.GetRemoteFile("file.bin"));
            try
            {
                AssertEquals(hashLocal, hashRemote);
            }
            finally
            {
                myLogger.WriteLine(string.Format("TestEqualData cost {0} seconds.", period.TotalSeconds));

                // Clean up
                server.stop();
                path.destroy();
            }
        }
        */
    }
}
