using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace expdirapp
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            var directory = args.Any() ? Path.GetFullPath(args.First()) : Environment.CurrentDirectory;
            if (!Directory.Exists(directory))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This folder does not exist.");
                Console.ResetColor();
                return;
            }
            var selection = 0;
            var finished = false;
            for (int i = 0; i < 15; i++)
                Console.WriteLine();
            var startIndex = Console.CursorTop - 15;
            var folders = Directory.GetDirectories(directory).ToList();
            if (directory != "/")
                folders.Insert(0, "..");
            var maxSize = folders.Any() ? folders.Max(f => f.Length) : 0;
            void clear()
            {
                Console.SetCursorPosition(0, startIndex);
                Console.WriteLine(new string(' ', directory.Length + 20));
                for (int i = 0; i < 14; i++)
                    Console.WriteLine(new string(' ', maxSize));
            }
            while (!finished)
            {
                Console.ResetColor();
                Console.SetCursorPosition(0, startIndex);
                Console.Write("Current directory : ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(directory);
                Console.ResetColor();
                Console.WriteLine();
                var indexMap = new Dictionary<int, int>();
                var toDisplay = folders.Where((f, i) =>
                {
                    var min = Math.Max(0, selection - 5);
                    if (min > folders.Count - 11)
                        min = folders.Count - 11;
                    var max = Math.Min(min + 10, folders.Count - 1);
                    return i >= min && i <= max;
                }).ToList();
                for (int i = 0; i < toDisplay.Count; ++i)
                {
                    Console.Write(new string(' ', maxSize));
                    Console.CursorLeft = 0;
                    if (directory != ":root")
                        Console.WriteLine(Path.GetFileName(toDisplay[i]));
                    else
                        Console.WriteLine(toDisplay[i]);
                    indexMap.Add(folders.IndexOf(toDisplay[i]), Console.CursorTop - 1);
                }
                Console.SetCursorPosition(0, indexMap[selection]);
                var background = Console.BackgroundColor;
                var foreground = Console.ForegroundColor;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    background = ConsoleColor.Gray;
                    foreground = ConsoleColor.DarkBlue;
                }
                Console.BackgroundColor = foreground;
                Console.ForegroundColor = background;
                if (directory != ":root")
                    Console.WriteLine(Path.GetFileName(folders[selection]));
                else
                    Console.WriteLine(folders[selection]);
                Console.ResetColor();
                Console.SetCursorPosition(0, startIndex + 14);
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write("^O");
                Console.ResetColor();
                if (directory != ":root")
                    Console.Write(" : Open         ");
                else
                {
                    Console.Write(" : ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Open         ");
                    Console.ResetColor();
                }
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write("^X");
                Console.ResetColor();
                Console.WriteLine(" : Cancel");
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.DownArrow && selection < folders.Count - 1)
                {
                    Console.SetCursorPosition(0, indexMap[selection]);
                    Console.WriteLine(Path.GetFileName(folders[selection]));
                    selection++;
                }
                else if (key.Key == ConsoleKey.UpArrow && selection > 0)
                {
                    Console.SetCursorPosition(0, indexMap[selection]);
                    Console.WriteLine(Path.GetFileName(folders[selection]));
                    selection--;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    clear();
                    if (folders[selection] != "..")
                    {
                        directory = folders[selection];
                        folders = Directory.GetDirectories(directory).ToList();
                        folders.Insert(0, "..");
                        maxSize = folders.Any() ? folders.Max(f => f.Length) : 0;
                    }
                    else if (directory == Directory.GetDirectoryRoot(directory) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        directory = ":root";
                        folders = DriveInfo.GetDrives().Select(d => d.RootDirectory.FullName).ToList();
                        maxSize = folders.Any() ? folders.Max(f => f.Length) : 0;
                    }
                    else
                    {
                        directory = Directory.GetParent(directory).FullName;
                        folders = Directory.GetDirectories(directory).ToList();
                        if (directory != "/")
                            folders.Insert(0, "..");
                        maxSize = folders.Any() ? folders.Max(f => f.Length) : 0;
                    }
                    selection = 0;
                }
                else if (key.Key == ConsoleKey.X && key.Modifiers == ConsoleModifiers.Control)
                    return;
                else if (key.Key == ConsoleKey.O && key.Modifiers == ConsoleModifiers.Control && directory != ":root")
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        File.WriteAllText("script.ps1", $"cd {directory}");
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        File.WriteAllText("location", directory);
                    return;
                }
            }
        }

        #endregion Private Methods
    }
}