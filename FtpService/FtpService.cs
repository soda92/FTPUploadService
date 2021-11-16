using System;
using System.IO;
using System.Linq;
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
            if (!ftp.DirectoryExists(dst))
            {
                ftp.CreateDirectory(dst);
            }
            string[] directories = Directory.GetDirectories(src);
            directories = directories.OrderBy(t => t).ToArray();
            foreach (string d in directories)
            {
                var dstFolder = Path.Join(dst, new DirectoryInfo(d).Name);
                UploadFolder(ftp, d, dstFolder);
            }
            string[] files = Directory.GetFiles(src);
            files = files.OrderBy(t => t).ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                var dstFile = Path.Join(dst, Path.GetFileName(files[i]));
                var info = string.Format(
                    "Uploading {0} of {1}: {2}", i, files.Length, files[i]);
                // build a format string to establish the maximum width do display
                var maxWidth = 80;
                var fmt = string.Format("{{0,-{0}}}", maxWidth);
                var cursorLeft = Console.CursorLeft;
                var cursorTop = Console.CursorTop;
                Console.SetCursorPosition(1, 0);
                Console.WriteLine(fmt, info);
                UploadFile(ftp, files[i], dstFile);
            }
        }
        public static double Upload(Config config)
        {
            var ftp = new FtpClient(
                config.ServerAddress, config.Port, config.Username, config.Password);

            ftp.RetryAttempts = 3;
            ftp.Connect();
            DateTime start = DateTime.Now;
            // FtpTrace.EnableTracing = true;
            // FtpTrace.LogToConsole = true;
            //FtpTrace.LogToFile = "upload.log";

            foreach (var path in config.Paths)
            {
                string src = Path.Join(config.LocalRoot, path.src);
                string dst = Path.Join(config.ServerRoot, config.HostName, path.dst);
                //ftp.UploadDirectory(src, dst, FtpFolderSyncMode.Update, FtpRemoteExists.Skip,FtpVerify.None,null,);
                UploadFolder(ftp, src, dst);
            }
            ftp.Disconnect();
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
            Console.Clear();
            var config = await MyConfig.ReadConfig();
            var time = MyClient.Upload(config);
            Console.WriteLine();
            Console.WriteLine(string.Format("It costs {0} seconds.", time));
        }
    }
}
