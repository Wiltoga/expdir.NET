using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace expdirapp
{
    internal static class Program
    {
        #region Private Methods

        private static bool Hidden(this string path)
        {
            if (Path.GetFileName(path).StartsWith('.'))
                return true;
            else if (File.Exists(path))
                return new FileInfo(path).Attributes.HasFlag(FileAttributes.Hidden);
            else if (Directory.Exists(path))
                return new DirectoryInfo(path).Attributes.HasFlag(FileAttributes.Hidden);
            else
                return false;
        }

        private static void Main(string[] args)
        {
            var directory = Environment.CurrentDirectory;
            var displayHidden = false;
            var displayFiles = false;
            foreach (var arg in args)
                switch (arg)
                {
                    case "-a":
                        displayHidden = true;
                        break;

                    case "-f":
                        displayFiles = true;
                        break;

                    case "--help":
                        Console.WriteLine(
@"Usage :

    expdir [<options>]

options :
    -a          displays hidden files & folders
    -f          displays files
    <path>      start the browser in this directory");
                        return;

                    default:
                        directory = arg;
                        break;
                }
            if (directory != ":root")
                directory = Path.GetFullPath(directory);
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (directory != ":root" && !Directory.Exists(directory))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This path does not exist or is not a folder.");
                Console.ResetColor();
                return;
            }
            var selection = 0;
            for (int i = 0; i < 15; i++)
                Console.WriteLine();
            var startIndex = Console.CursorTop - 15;
            var folders = directory == ":root" ?
                DriveInfo.GetDrives().Select(d => d.RootDirectory.FullName).ToList()
                : Directory.GetDirectories(directory).Where(f => !f.Hidden() || displayHidden).ToList();
            if (directory != "/" && directory != ":root")
                folders.Insert(0, "..");
            var dirCount = folders.Count;
            if (displayFiles)
                folders.AddRange(Directory.GetFiles(directory).Where(f => !f.Hidden() || displayHidden).ToList());
            var maxSize = folders.Any() ? folders.Max(f => Path.GetFileName(f).Length) : 0;
            void clear()
            {
                Console.SetCursorPosition(0, startIndex);
                Console.WriteLine(new string(' ', directory.Length + 20));
                Console.SetCursorPosition(0, startIndex + 1);
                for (int i = 0; i < 14; i++)
                    Console.WriteLine(new string(' ', maxSize));
            }
            var letterHistory = "";
            var folderHistory = new Stack<string>();
            if (folders.First() == "..")
                foreach (var parent in directory.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries))
                    folderHistory.Push(parent);
            while (true)
            {
                Console.ResetColor();
                Console.SetCursorPosition(0, startIndex);
                Console.Write("Current directory : ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(directory);
                Console.ResetColor();
                var indexMap = new Dictionary<int, int>();
                var toDisplay = folders.Where((f, i) =>
                {
                    var min = Math.Max(0, selection - 5);
                    if (min > folders.Count - 11)
                        min = folders.Count - 11;
                    var max = Math.Min(min + 10, folders.Count - 1);
                    return i >= min && i <= max;
                }).ToList();
                Console.SetCursorPosition(0, startIndex + 2);
                for (int i = 0; i < toDisplay.Count; ++i)
                {
                    var trueIndex = folders.IndexOf(toDisplay[i]);
                    Console.Write(new string(' ', maxSize));
                    Console.CursorLeft = 0;
                    if (trueIndex >= dirCount)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    if (directory != ":root")
                        Console.WriteLine(Path.GetFileName(toDisplay[i]));
                    else
                        Console.WriteLine(toDisplay[i]);
                    Console.ResetColor();
                    indexMap.Add(trueIndex, Console.CursorTop - 1);
                }
                Console.SetCursorPosition(0, indexMap[selection]);
                var background = Console.BackgroundColor;
                var foreground = Console.ForegroundColor;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    background = ConsoleColor.Black;
                    foreground = ConsoleColor.Gray;
                }
                if (selection >= dirCount)
                    background = ConsoleColor.DarkRed;
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
                    Console.Write(" : Open   ");
                else
                {
                    Console.Write(" : ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Open   ");
                    Console.ResetColor();
                }
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write("^X");
                Console.ResetColor();
                Console.Write(" : Cancel   ");
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("^R");
                Console.ResetColor();
                Console.Write(" : Refresh   ");
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Tab");
                Console.ResetColor();
                if (folders.First() == "..")
                    Console.Write(" : Parent");
                else
                {
                    Console.Write(" : ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Parent");
                    Console.ResetColor();
                }
                Console.WriteLine();
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.DownArrow && selection < folders.Count - 1)
                {
                    Console.SetCursorPosition(0, indexMap[selection]);
                    if (selection >= dirCount)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(Path.GetFileName(folders[selection]));
                    Console.ResetColor();
                    selection++;
                }
                else if (key.Key == ConsoleKey.UpArrow && selection > 0)
                {
                    Console.SetCursorPosition(0, indexMap[selection]);
                    if (selection >= dirCount)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(Path.GetFileName(folders[selection]));
                    Console.ResetColor();
                    selection--;
                }
                else if ((key.Key == ConsoleKey.Enter && selection < dirCount) || (key.Key == ConsoleKey.Tab && folders.First() == ".."))
                {
                    if (key.Key == ConsoleKey.Tab)
                        selection = 0;
                    clear();
                    if (folders[selection] != "..")
                    {
                        directory = folders[selection];
                        folders = Directory.GetDirectories(directory).Where(f => !f.Hidden() || displayHidden).ToList();
                        folders.Insert(0, "..");
                        dirCount = folders.Count;
                        if (displayFiles)
                            folders.AddRange(Directory.GetFiles(directory).Where(f => !f.Hidden() || displayHidden).ToList());
                        maxSize = folders.Any() ? folders.Max(f => Path.GetFileName(f).Length) : 0;
                        letterHistory = "";
                        selection = dirCount > 1 ? 1 : 0;
                        folderHistory.Push(Path.GetFileName(directory));
                    }
                    else if (directory == Directory.GetDirectoryRoot(directory) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        directory = ":root";
                        folders = DriveInfo.GetDrives().Select(d => d.RootDirectory.FullName).ToList();
                        dirCount = folders.Count;
                        maxSize = folders.Any() ? folders.Max(f => Path.GetFileName(f).Length) : 0;
                        letterHistory = "";
                        selection = 0;
                        if (folderHistory.Any())
                        {
                            var parent = folderHistory.Pop();
                            int index = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                                folders.FindIndex(folder => Path.GetFileName(folder).ToLower() == parent.ToLower())
                                : folders.FindIndex(folder => Path.GetFileName(folder) == parent);
                            if (index != -1)
                                selection = index;
                        }
                    }
                    else
                    {
                        directory = Directory.GetParent(directory).FullName;
                        folders = Directory.GetDirectories(directory).Where(f => !f.Hidden() || displayHidden).ToList();
                        if (directory != "/")
                            folders.Insert(0, "..");
                        dirCount = folders.Count;
                        if (displayFiles)
                            folders.AddRange(Directory.GetFiles(directory).Where(f => !f.Hidden() || displayHidden).ToList());
                        maxSize = folders.Any() ? folders.Max(f => Path.GetFileName(f).Length) : 0;
                        letterHistory = "";
                        selection = 0;
                        if (folderHistory.Any())
                        {
                            var parent = folderHistory.Pop();
                            int index = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                                folders.FindIndex(folder => Path.GetFileName(folder).ToLower() == parent.ToLower())
                                : folders.FindIndex(folder => Path.GetFileName(folder) == parent);
                            if (index != -1)
                                selection = index;
                        }
                    }
                }
                else if (key.Key == ConsoleKey.X && key.Modifiers == ConsoleModifiers.Control)
                    return;
                else if (key.Key == ConsoleKey.O && key.Modifiers == ConsoleModifiers.Control && directory != ":root")
                {
                    File.WriteAllText("location", directory);
                    return;
                }
                else if (key.Key == ConsoleKey.R && key.Modifiers == ConsoleModifiers.Control)
                {
                    folders = directory == ":root" ?
                        DriveInfo.GetDrives().Select(d => d.RootDirectory.FullName).ToList()
                        : Directory.GetDirectories(directory).Where(f => !f.Hidden() || displayHidden).ToList();
                    if (directory != "/" && directory != ":root")
                    {
                        folders.Insert(0, "..");
                        dirCount = folders.Count;
                        if (displayFiles)
                            folders.AddRange(Directory.GetFiles(directory).Where(f => !f.Hidden() || displayHidden).ToList());
                    }
                    maxSize = folders.Any() ? folders.Max(f => Path.GetFileName(f).Length) : 0;
                }
                else if (key.KeyChar != 0 && key.Key != ConsoleKey.Tab && key.Key != ConsoleKey.Enter)
                {
                    letterHistory += char.ToLower(key.KeyChar);
                    var folder = folders
                        .Score(f =>
                        {
                            f = f == ".." ? f : Path.GetFileName(f).ToLower();
                            var best = 0;
                            for (int i = letterHistory.Length - 1; i >= 0; --i)
                            {
                                if (f.StartsWith(letterHistory.Substring(i)))
                                    best = letterHistory.Length - i;
                            }
                            return best;
                        });
                    if (folder != null)
                    {
                        Console.SetCursorPosition(0, indexMap[selection]);
                        Console.WriteLine(Path.GetFileName(folders[selection]));
                        selection = folders.IndexOf(folder);
                    }
                }
            }
        }

        private static T Score<T>(this IEnumerable<T> list, Func<T, double> calculator)
        {
            (T, double)? best = null;
            foreach (var item in list)
            {
                var score = calculator(item);
                if (!best.HasValue || best.Value.Item2 < score)
                    best = (item, score);
            }
            if (best.HasValue)
                return best.Value.Item1;
            else
                return default;
        }

        private static IEnumerable<T> ShiftLeft<T>(this IEnumerable<T> list, int count)
        {
            if (count == 0)
                return list;
            else if (count > 0)
                return list.Skip(count).Concat(list.Take(count));
            else
                return list.Take(-count).Concat(list.Skip(-count));
        }

        private static IEnumerable<T> ShiftRight<T>(this IEnumerable<T> list, int count)
            => list.ShiftLeft(-count);

        #endregion Private Methods
    }
}