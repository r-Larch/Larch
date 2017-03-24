using System;
using System.Collections.Generic;
using System.Linq;
using Larch.Host.Parser;


namespace Larch.Host {
    public class ConsoleEx {
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

        public static void PrintHighlighted<T>(string line, Match<T> filter, ConsoleColor color = ConsoleColor.Red) {
            if (filter == null) {
                Console.WriteLine(line);
                return;
            }

            var chars = line.ToCharArray();

            var count = 0;
            var max = line.IndexOf(filter.Value, StringComparison.Ordinal);

            while (count < max) {
                Console.Write(chars[count++]);
            }

            var last = filter.Matches.Max(x => x.Stop);
            var c = Console.ForegroundColor;
            for (var i = 0; i < last; i++) {
                var part = filter.Matches.LastOrDefault(x => x.IsMatch && i >= x.Start && i < x.Stop);
                if (part != null) {
                    var col = GetColor(part.Num);
                    Console.ForegroundColor = col;
                } else {
                    Console.ForegroundColor = c;
                }
                Console.Write(chars[count++]);
            }
            Console.ForegroundColor = c;

            while (count < line.Length) {
                Console.Write(chars[count++]);
            }

            Console.Write(Environment.NewLine);
        }

        private static ConsoleColor GetColor(int num) {
            var t = Enum.GetValues(typeof(ConsoleColor));
            return (ConsoleColor) t.GetValue(t.Length - num - 2);
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

        public static void PrintWithPaging<T>(IEnumerable<Match<T>> matches, Func<T, int, string> line, int countAll = -1) {
            var mes = matches as Match<T>[] ?? matches.ToArray();

            var found = mes.Length;
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
            foreach (var match in mes) {
                ConsoleEx.PrintHighlighted(line(match.Model, lineNumber++), match);
                count++;

                if (count < height) continue;

                Console.Write($" -- page: {page++}/{pages} -- ");
                count = 0;
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) {
                    break;
                }

                Console.CursorLeft = 0;
            }

            Console.WriteLine();
            if (found >= height) {
                Console.WriteLine();
                Console.WriteLine($"found: {found}" + (countAll != -1 ? $" matchs in {countAll} entries" : ""));
            }
        }
    }
}