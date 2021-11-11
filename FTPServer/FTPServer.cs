using System;
using System.IO;
using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using FubarDev.FtpServer.AccountManagement;
using MyConfigNamespace;

namespace FtpServer
{
    public class Class1
    {
        public static async Task Main(string[] args)
        {

            // Setup dependency injection
            var services = new ServiceCollection();

            // use %TEMP%/TestFtpServer as root folder
            services.Configure<DotNetFileSystemOptions>(opt => opt
                .RootPath = @"D:\Data\upload");

            // Add FTP server services
            // DotNetFileSystemProvider = Use the .NET file system functionality
            // AnonymousMembershipProvider = allow only anonymous logins
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem()); // Use the .NET file system functionality
                                         // .EnableAnonymousAuthentication()); // allow anonymous logins

            services.AddSingleton<IMembershipProvider, CustomMembershipProvider>();
            // Configure the FTP server
            var config = await MyConfig.ReadConfig();
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = config.server_address);

            // Build the service provider
            using (var serviceProvider = services.BuildServiceProvider())
            {
                // Initialize the FTP server
                var ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();

                // Start the FTP server
                await ftpServerHost.StartAsync();

                Console.WriteLine("Press ENTER/RETURN to close the test application.");
                Console.ReadLine();

                // Stop the FTP server
                await ftpServerHost.StopAsync();
            }

        }
    }
}
