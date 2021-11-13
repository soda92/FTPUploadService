using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace ServerTests
{
    [TestClass]
    public class TestLocation
    {
        [TestMethod]
        public void TestGetCurrLocation()
        {
            Console.WriteLine(AppContext.BaseDirectory);
        }
        [TestMethod]
        public void TestGetSelfProjectLocation()
        {
            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            while (dir.Name != appName)
            {
                dir = Directory.GetParent(dir.FullName);
            }
            Console.WriteLine(dir.FullName);
        }

        public string GetSelfProjectLocation()
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
        public void TestGetCurrFiles()
        {
            string CurrentLocation = GetSelfProjectLocation();
            string[] files = Directory.GetFiles(CurrentLocation);
            foreach (string file in files)
            {
                Console.WriteLine(file);
            }
        }

        public string[] GetCurrFiles()
        {
            string CurrentLocation = GetSelfProjectLocation();
            string[] files = Directory.GetFiles(CurrentLocation);
            return files;
        }
        [TestMethod]
        public void TestGetAllUsing()
        {
            foreach (var file in GetCurrFiles())
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.StartsWith("using"))
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
            }
        }
        public string[] GetAllUsing()
        {
            List<string> AllUsing = new List<string>();
            foreach (var file in GetCurrFiles())
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.StartsWith("using"))
                        {
                            AllUsing.Add(line);
                        }
                    }
                }
            }
            return AllUsing.ToArray();
        }
        [TestMethod]
        public void TestGetUniqueImports()
        {
            HashSet<string> UniqueImports = new HashSet<string>();
            foreach (var import in GetAllUsing())
            {
                UniqueImports.Add(import);
            }
            foreach (var import in UniqueImports)
            {
                Console.WriteLine(import);
            }
        }
        public string[] GetUniqueImports()
        {
            var UniqueImports = new HashSet<string>();
            foreach (var import in GetAllUsing())
            {
                UniqueImports.Add(import);
            }
            var imports = new List<string>();
            foreach (string import in UniqueImports)
            {
                imports.Add(import);
            }
            return imports.ToArray();
        }
        public string[] GetUniqueImportsNoSystem()
        {
            var uniqueImports = new List<string>();
            foreach (var import in GetUniqueImports())
            {
                var builder = new StringBuilder(import);
                builder.Replace("using", "");
                builder.Replace(";", "");
                builder.Replace(" ", "");
                var import2 = builder.ToString();
                if (import2.StartsWith("System") || import2.StartsWith("Microsoft"))
                {
                    continue;
                }
                uniqueImports.Add(import2);
            }
            return uniqueImports.ToArray();
        }
        [TestMethod]
        public void TestGetUniqueImportsNoSystem()
        {
            foreach (var ui in GetUniqueImportsNoSystem())
            {
                Console.WriteLine(ui);
            }
        }
        [TestMethod]
        public void TestGetParentDirs()
        {
            var p = Directory.GetParent(GetSelfProjectLocation()).FullName;
            string[] dirs = Directory.GetDirectories(p);
            foreach (var dir in dirs)
            {
                Console.WriteLine(dir);
            }
        }
        [TestMethod]
        public void TestGetParentDirNames()
        {
            var p = Directory.GetParent(GetSelfProjectLocation()).FullName;
            string[] dirs = Directory.GetDirectories(p);
            foreach (var dir in dirs)
            {
                var info = new DirectoryInfo(dir);
                Console.WriteLine(info.Name);
            }
        }
        public string[] GetParentDirNames()
        {
            var names = new List<string>();
            var p = Directory.GetParent(GetSelfProjectLocation()).FullName;
            string[] dirs = Directory.GetDirectories(p);
            foreach (var dir in dirs)
            {
                var info = new DirectoryInfo(dir);
                names.Add(info.Name);
            }
            return names.ToArray();
        }
        [TestMethod]
        public void TestGetExistingFolderNames()
        {
            var names = GetParentDirNames();
            foreach (var ui in GetUniqueImportsNoSystem())
            {
                string first = ui.Split(".")[0];
                if (names.Contains(first))
                {
                    Console.WriteLine(first);
                }
            }
        }
        public string[] GetExistingFolderNames()
        {
            var ExistingNames = new List<string>();
            var names = GetParentDirNames();
            foreach (var ui in GetUniqueImportsNoSystem())
            {
                string first = ui.Split(".")[0];
                if (names.Contains(first))
                {
                    ExistingNames.Add(first);
                }
            }
            return ExistingNames.ToArray();
        }
        [TestMethod]
        public void TestGetOtherDirs()
        {
            var CURR = GetSelfProjectLocation();
            var PARENT = Directory.GetParent(CURR);
            var names = GetExistingFolderNames();
            foreach (var name in names)
            {
                var path = Path.Join(PARENT.FullName, name);
                var exists = Directory.Exists(path);
                Assert.IsTrue(exists);
            }
        }
        public string[] GetOtherDirs()
        {
            var OtherDirs = new List<string>();
            var CURR = GetSelfProjectLocation();
            var PARENT = Directory.GetParent(CURR);
            var names = GetExistingFolderNames();
            foreach (var name in names)
            {
                var path = Path.Join(PARENT.FullName, name);
                OtherDirs.Add(path);
            }
            return OtherDirs.ToArray();
        }
    }
}
