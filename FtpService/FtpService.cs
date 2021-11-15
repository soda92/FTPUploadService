using System;
using System.IO;
using FluentFTP;
using FluentFTP.Helpers;
using System.Threading.Tasks;

namespace FtpService
{
    public class MyClient
    {
        public static void UploadFile(FtpClient ftp, string src, string dst)
        {
            if (ftp.FileExists(dst))
            {
                var size = ftp.GetFileSize(dst);
                var size2 = new FileInfo(src).Length;
                if (size == size2)
                {
                    return;
                }
            }

            ftp.UploadFile(src, dst, FtpRemoteExists.Overwrite, true);
        }
        public static void UploadFolder(FtpClient ftp, string src, string dst)
        {
            ftp.CreateDirectory(dst);
            string[] directories = Directory.GetDirectories(src);
            foreach (string d in directories)
            {
                var dstFolder = Path.Join(dst, new DirectoryInfo(d).Name);
                UploadFolder(ftp, d, dstFolder);
            }
            string[] files = Directory.GetFiles(src);
            foreach (string f in files)
            {
                var dstFile = Path.Join(dst, Path.GetFileName(f));
                UploadFile(ftp, f, dstFile);
            }
        }
        public static async Task<double> Upload(Config config)
        {
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
                string dst = Path.Join(config.ServerRoot, config.HostName, path.dst);
                UploadFolder(ftp, src, dst);
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

            var config = await MyConfig.ReadConfig();
            var time = await MyClient.Upload(config);

            Console.WriteLine(string.Format("It costs {0} seconds.", time));
        }
    }
}
