using System;

namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Searching file in...");
            Console.ReadKey();

            // save the current cursor position
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            // build a format string to establish the maximum width do display
            var maxWidth = 60;
            var fmt = String.Format("{{0,-{0}}}", maxWidth);
            var dirList = new string[]{ "aaa", "bbb" };
            foreach (var dir in dirList)
            {
                Console.ReadKey();
                Console.Write(fmt, dir);

                // do some work
            }
            Console.WriteLine();
        }
    }
}
