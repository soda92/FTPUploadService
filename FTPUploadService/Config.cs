using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace FtpService
{
    public class MyConfig
    {
        private static string getRuntimePath()
        {
            return System.AppContext.BaseDirectory;
        }
        private static bool Exists()
        {
            var filename = Path.Join(getRuntimePath(), "config.json");
            return File.Exists(filename);
        }
        public static string GetConfigFullPath()
        {
            return Path.Join(getRuntimePath(), "config.json");
        }
        private static async Task CreateConfig()
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
                WriteIndented = true,
            };
            using FileStream createStream = File.Create(GetConfigFullPath());
            await JsonSerializer.SerializeAsync(createStream, model, options);
            await createStream.DisposeAsync();
        }

        public static async Task<DataModel> ReadConfig()
        {
            if (!Exists())
            {
                // throw new System.Exception("Config not exists");
                await CreateConfig();
            }
            using FileStream openStream = File.OpenRead(GetConfigFullPath());
            DataModel data = await JsonSerializer.DeserializeAsync<DataModel>(openStream);
            return data;
        }
    }
}
