﻿using System;
using System.IO;
using System.Threading;
using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;

using Microsoft.Extensions.DependencyInjection;

namespace FTPServer
{
    public class Class1
    {
        static void Main(string[] args)
        {
            // Setup dependency injection
            var services = new ServiceCollection();

            // use %TEMP%/TestFtpServer as root folder
            services.Configure<DotNetFileSystemOptions>(opt => opt
                .RootPath = Path.Combine(Path.GetTempPath(), "TestFtpServer"));

            // Add FTP server services
            // DotNetFileSystemProvider = Use the .NET file system functionality
            // AnonymousMembershipProvider = allow only anonymous logins
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem() // Use the .NET file system functionality
                .EnableAnonymousAuthentication()); // allow anonymous logins

            // Configure the FTP server
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = "127.0.0.1");

            // Build the service provider
            using (var serviceProvider = services.BuildServiceProvider())
            {
                // Initialize the FTP server
                var ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();

                // Start the FTP server
                // ftpServerHost.StartAsync(CancellationToken.None).Wait();

                // Console.WriteLine("Press ENTER/RETURN to close the test application.");
                // Console.ReadLine();

                // // Stop the FTP server
                // ftpServerHost.StopAsync(CancellationToken.None).Wait();

                string path = System.AppContext.BaseDirectory;
                Console.WriteLine(path);
                Console.WriteLine(Path.GetTempPath());
            }

        }
    }
}
