using System;
using FluentFTP;
using FluentFTP.Helpers;
using System.Threading.Tasks;

namespace FtpService
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var config = await MyConfig.ReadConfig();

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
