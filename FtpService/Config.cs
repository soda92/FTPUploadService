using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FtpService
{
    public class PathMapping
    {
        public string src { get; set; }
        public string dst { get; set; }
    }
    public class Config
    {
        public string ServerAddress { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ServerRoot { get; set; }
        public string LocalRoot { get; set; }
        public string HostName { get; set; }
        public List<PathMapping> Paths { get; set; }
    }
    public class MyConfig
    {
        public static void CreateExampleConfig()
        {
            CreateConfigUsingData(GetExampleConfig());
        }

        public static void CreateConfigUsingData(Config data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string fileName = GetConfigFullPath();
            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(fileName, jsonString);
        }

        public static string GetSerializedData(Config data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            var json = JsonSerializer.Serialize(data, options);
            return json;
        }

        public static Config GetExampleConfig()
        {

            var data = new Config
            {
                ServerAddress = "127.0.0.1",
                Port = 2222,
                Username = "user",
                Password = "12345",
                ServerRoot = "/Data/Upload",
                LocalRoot = "/home/toybrick/",
                HostName = "board",
                Paths = new List<PathMapping>
                {
                    new PathMapping{ src = "lamp_sample", dst = "lamp_sample" },
                    new PathMapping{ src = "Multicast_rk", dst = "other/Multicast_rk"},
                    new PathMapping{ src = "code/lamp_detect", dst = "code_lamp_detect"},
                },
            };
            return data;
        }

        public static async Task<Config> ReadConfig()
        {
            using FileStream openStream = File.OpenRead(GetConfigFullPath());
            Config data = await JsonSerializer.DeserializeAsync<Config>(openStream);
            return data;
        }

        public static string GetRuntimePath()
        {
            return System.AppContext.BaseDirectory;
        }
        public static bool Exists()
        {
            var filename = Path.Join(GetRuntimePath(), "config.json");
            return File.Exists(filename);
        }
        public static void DeleteConfig()
        {
            var filename = Path.Join(GetRuntimePath(), "config.json");
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }
        public static string GetConfigFullPath()
        {
            return Path.Join(GetRuntimePath(), "config.json");
        }
    }
}
