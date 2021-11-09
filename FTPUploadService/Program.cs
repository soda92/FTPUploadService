using System;
using FluentFTP;
using FluentFTP.Helpers;

namespace FTPUploadService
{
    class Program
    {
        static void Main(string[] args)
        {
           using(var ftp  = new FtpClient("127.0.0.1",2222, "user", "12345"))
            {
                DateTime start = DateTime.Now;
                FtpTrace.EnableTracing = true;
                FtpTrace.LogToConsole = true;
                ftp.Connect();
                ftp.RetryAttempts = 3;
                ftp.UploadDirectory(@"D:\Data\test-sesrver", @"upload/test_server", 
                    FtpFolderSyncMode.Update, FtpRemoteExists.Resume, FtpVerify.Retry);
                DateTime end = DateTime.Now;
                TimeSpan timeSpan = end - start;
                Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
            }
        }
    }
}
