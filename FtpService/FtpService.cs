using System;
using System.IO;
using FluentFTP;
using FluentFTP.Helpers;
using System.Threading.Tasks;

namespace FtpService
{
    public class MyClient
    {
        public static void UploadFolder(FtpClient ftp, string src, string dst)
        {
            //File
        }
        public static async Task<double> Upload()
        {
            var config = await MyConfig.ReadConfig();
            var ftp = new FtpClient(
                config.ServerAddress, config.Port, config.Username, config.Password);

            ftp.RetryAttempts = 3;
            ftp.Connect();
            DateTime start = DateTime.Now;
            FtpTrace.EnableTracing = true;
            FtpTrace.LogToFile = "upload.log";

            foreach (var path in config.Paths)
            {
                string src = Path.Join(config.LocalRoot, path.src);
                string dst = Path.Join(config.ServerRoot, path.dst);
                // TODO
                await ftp.UploadDirectoryAsync(src, dst,
                FtpFolderSyncMode.Update, FtpRemoteExists.Resume, FtpVerify.Retry);
            }
            await ftp.DisconnectAsync();
            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            return timeSpan.TotalSeconds;
        }
    }
    class Program
    {
        public static async Task Main(string[] args)
        {
            if (!MyConfig.Exists())
            {
                MyConfig.CreateExampleConfig();
                return;
            }

            var time = await MyClient.Upload();

            Console.WriteLine(string.Format("It costs {0} seconds.", time));
        }
    }
}
