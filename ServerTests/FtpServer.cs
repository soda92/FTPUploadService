using System;
using System.IO;
using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using FubarDev.FtpServer.AccountManagement;
using FtpService;

namespace ServerTests
{
    public class Server
    {
        private static IFtpServerHost ftpServerHost;

        public static async Task StartServerAsync()
        {
            Directory.CreateDirectory(TestConfig.DataLocation);
            // Setup dependency injection
            var services = new ServiceCollection();
            services.Configure<DotNetFileSystemOptions>(opt => opt
                .RootPath = TestConfig.ServerStartLocation);
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem()); // Use the .NET file system functionality

            services.AddSingleton<IMembershipProvider, CustomMembershipProvider>();
            // Configure the FTP server
            var config = MyConfig.GetExampleConfig();
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = config.ServerAddress);
            services.Configure<FtpServerOptions>(opt => opt.Port = config.Port);
            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();
            // Initialize the FTP server
            ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();
            // Start the FTP server

            await ftpServerHost.StartAsync();
        }
        public static async Task StopServerAsync()
        {
            await ftpServerHost.StopAsync();
        }

        public static async Task Main(string[] args)
        {
            await StartServerAsync();

            Console.WriteLine("Press ENTER/RETURN to close the test application.");
            Console.ReadLine();
            // Stop the FTP server

            await StopServerAsync();
        }
    }
}
