using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using FTPUploadService;
using System.Text;

namespace DirTest
{
    [TestClass]
    public class DirTest
    {
        public string getRuntimeDir()
        {
            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            while (dir.Name != appName)
            {
                dir = Directory.GetParent(dir.FullName);
            }
            return dir.FullName;
        }
        [TestMethod]
        public void testdir()
        {
            Console.WriteLine(getRuntimeDir());
        }

        [TestMethod]
        public void getRuntimePath()
        {
            Console.WriteLine(System.AppContext.BaseDirectory);
        }

        [TestMethod]
        public void GenConfig()
        {
            var model = new DataModel
            {
                server_address = "127.0.0.1",
                port = 2222,
                username = "user",
                password = "12345",
                local_folder = @"D:\Data\test-sesrver",
                remote_folder = @"upload/test_server"
            };

            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                AllowTrailingCommas = true,
            };
            string serialized = JsonSerializer.Serialize(model, options);
            string filename = getRuntimeDir() + "/config.json";
            var bytes = Encoding.UTF8.GetBytes(serialized);
            using FileStream createStream = File.Create(filename);
            createStream.Write(bytes);
            createStream.Dispose();
        }

        [TestMethod]
        public void TestPath2()
        {
            var path = Path.Join(getRuntimeDir(), "config.json");
            Console.WriteLine(path);
        }

        [TestMethod]
        public void GenConfig2()
        {
            var model = new DataModel
            {
                server_address = "127.0.0.1",
                port = 2222,
                username = "user",
                password = "12345",
                local_folder = @"D:\Data\test-sesrver",
                remote_folder = @"upload/test_server"
            };

            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                AllowTrailingCommas = true,
            };
            string serialized = JsonSerializer.Serialize(model, options);
            var filename = Path.Join(getRuntimeDir(), "config.json");
            var bytes = Encoding.UTF8.GetBytes(serialized);
            using FileStream createStream = File.Create(filename);
            createStream.Write(bytes);
            createStream.Dispose();
        }

        [TestMethod]
        public void DeleteConfigFile()
        {
            var filename = Path.Join(getRuntimeDir(), "config.json");
            if (File.Exists(filename))
            {
                Console.WriteLine("File exists! deleting...");
                File.Delete(filename);
            }
        }
    }
}
