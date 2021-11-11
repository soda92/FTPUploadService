using System;
using FluentFTP;
using FluentFTP.Helpers;
using ConfigNamespace;
using System.Threading.Tasks;

namespace FTPUploadService
{
    public class DataModel
    {
        public string server_address { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string local_folder { get; set; }
        public string remote_folder { get; set; }
    }
    class Program
    {
        public static async Task Main(string[] args)
        {
            if (!Config.Exists())
            {
                Console.WriteLine("Creating config file...");
                await Config.CreateConfig();
                return;
            }

            var config = await Config.ReadConfig();

            using (var ftp = new FtpClient(
                config.server_address, config.port, config.username, config.password))
            {
                DateTime start = DateTime.Now;
                FtpTrace.EnableTracing = true;
                FtpTrace.LogToFile = "upload.log";
                ftp.Connect();
                ftp.RetryAttempts = 3;
                await ftp.UploadDirectoryAsync(config.local_folder, config.remote_folder,
                    FtpFolderSyncMode.Update, FtpRemoteExists.Resume, FtpVerify.Retry);
                DateTime end = DateTime.Now;
                TimeSpan timeSpan = end - start;
                Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
            }
        }
    }
}
