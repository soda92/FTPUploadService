using System;
using FluentFTP;
using FluentFTP.Helpers;
using System.Threading.Tasks;

namespace FtpService
{
    public class MyClient
    {
        public static async Task<FtpClient> Get()
        {
            var config = await MyConfig.ReadConfig();
            var ftp = new FtpClient(
                config.server_address, config.port, config.username, config.password);
            return ftp;
        }
    }
    class Program
    {
        public static async Task Main(string[] args)
        {
            var config = await MyConfig.ReadConfig();
            var ftp = await MyClient.Get();
            ftp.RetryAttempts = 3;
            ftp.Connect();
            DateTime start = DateTime.Now;
            FtpTrace.EnableTracing = true;
            FtpTrace.LogToFile = "upload.log";

            await ftp.UploadDirectoryAsync(config.local_folder, config.remote_folder,
                FtpFolderSyncMode.Update, FtpRemoteExists.Resume, FtpVerify.Retry);
            await ftp.DisconnectAsync();
            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            Console.WriteLine(string.Format("It costs {0} seconds.", timeSpan.TotalSeconds));
        }
    }
}
