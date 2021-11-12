using System;
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
            // Setup dependency injection
            var services = new ServiceCollection();

            services.Configure<DotNetFileSystemOptions>(opt => opt
                .RootPath = @"D:\Data\upload");
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem()); // Use the .NET file system functionality

            services.AddSingleton<IMembershipProvider, CustomMembershipProvider>();
            // Configure the FTP server
            var config = await MyConfig.ReadConfig();
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = config.server_address);
            services.Configure<FtpServerOptions>(opt => opt.Port = config.port);
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
