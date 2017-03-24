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
            var reset = false;


            var count = 0;
            var max = line.IndexOf(filter.Value, StringComparison.Ordinal);

            //Console.WriteLine(filter.Value);
            //new Table().Create(1, 1, "matchs", filter.Matches);return;

            var s = $"max: {max} char: '{chars[max]}' ";
            for (var i = 0; i < max; i++) {
                Console.Write(chars[i]);
                count++;
            }

            foreach (var match in filter.Matches) {
                var start = count + match.Start;
                var stop = count + match.Stop;
                for (var i = count; i < start; i++) {
                    Console.Write(chars[i]);
                    count++;
                }

                if (match.IsMatch) {
                    Console.ForegroundColor = color;
                    reset = true;
                }

                s += $"{match.Start}->{match.Stop}=>{start}->{stop} ";
                for (var i = start; i < stop; i++) {
                    Console.Write(chars[i]);
                    count++;
                }

                if (reset) {
                    Console.ResetColor();
                }
            }

            for (var i = count; i < line.Length; i++) {
                Console.Write(chars[i]);
            }

            Console.Write($"\t{s}");

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