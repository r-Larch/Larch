using System;
using System.Collections.Generic;
using System.Linq;
using Larch.Host.Parser;


namespace Larch.Host {
    internal class ConsoleEx {
        public static void PrintException(string message, Exception e) {
            var width = Console.WindowWidth;
            Console.ForegroundColor = ConsoleColor.Red;

            Repeat("─", width);
            Console.WriteLine($"{e.GetType().Name}: {message}");
            Console.WriteLine(e.StackTrace);
            Repeat("─", width);

            Console.ResetColor();
        }

        public static void Repeat(string sign, int num) {
            for (var i = 0; i < num; i++) {
                Console.Write(sign);
            }
        }

        public static void PrintHighlighted(string line, string s, ConsoleColor color = ConsoleColor.Red) {
            if (s == null) {
                Console.WriteLine(line);
                return;
            }

            var start = line.IndexOf(s, StringComparison.InvariantCultureIgnoreCase);
            var end = start + s.Length;
            var chars = line.ToCharArray();
            var reset = false;

            for (var i = 0; i < chars.Length; i++) {
                if (i == start) {
                    Console.ForegroundColor = color;
                    reset = true;
                }
                if (i == end) {
                    Console.ResetColor();
                    reset = false;
                }
                Console.Write(chars[i]);
            }

            if (reset) {
                Console.ResetColor();
            }
            Console.Write(Environment.NewLine);
        }

        public static bool AskForYes(string question) {
            Console.Write(question + " (Y/N) ");
            ConsoleKey key;
            while (true) {
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.Y || key == ConsoleKey.N) {
                    break;
                }
            }

            return key == ConsoleKey.Y;
        }

        public static List<T> AskYesOrNo<T>(List<T> hosts, Func<T, string> question) {
            var toRemove = new List<T>();
            foreach (var host in hosts) {
                if (AskForYes(question(host))) {
                    toRemove.Add(host);
                }
                Console.WriteLine();
            }

            return toRemove;
        }

        public static void PrintWithPaging<T>(IEnumerable<T> list, Func<T, int, string> line, string search = null, int countAll = -1) {
            var li = list as T[] ?? list.ToArray();

            var found = li.Length;
            var height = Console.WindowHeight;
            var pages = found/height;
            var page = 1;

            Console.WriteLine($"found: {found}" + (countAll != -1 ? $" matchs in {countAll} entries" : ""));

            if (found >= height) {
                if (!AskForYes($"Are you sure you wand show {pages} pages full text?")) {
                    return;
                }
            }

            if (found == 0) {
                return;
            }

            Console.WriteLine();

            var count = 0;
            var lineNumber = 0;
            Console.WriteLine("Line  |");
            foreach (var x in li) {
                ConsoleEx.PrintHighlighted(line(x, lineNumber++), search);
                count++;

                if (count < height) continue;

                Console.Write($" -- page: {page++}/{pages} -- ");
                count = 0;
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) {
                    break;
                }
            }

            Console.WriteLine();
            if (found >= height) {
                Console.WriteLine();
                Console.WriteLine($"found: {found}" + (countAll != -1 ? $" matchs in {countAll} entries" : ""));
            }
        }
    }
}