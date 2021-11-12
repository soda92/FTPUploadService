using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ServerTests
{
    public class TestConfig
    {
        private static string root_folder = @"D:\Data";
        public static string server_start_location = Path.Join(root_folder, "server");
        public static string data_location = Path.Join(root_folder, "data");
    }
}
