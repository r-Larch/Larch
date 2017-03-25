using System;
using System.Collections.Generic;
using System.Linq;
using Larch.Host.Contoller;


namespace Larch.Host {
    public class ConsoleEx {
        public static void PrintException(string message, Exception e) {
            if (Console.CursorLeft != 0) {
                Console.WriteLine();
            }

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

        public static bool AskForYes(string question) {
            return AskForYes(ConsoleWriter.Create(question));
        }

        public static bool AskForYes(ConsoleWriter question) {
            question.Flush(newLine: false);
            Console.Write(" (Y/N) ");
            while (true) {
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Y || key == ConsoleKey.N) {
                    return key == ConsoleKey.Y;
                }
            }
        }

        public static List<T> AskYesOrNo<T>(List<T> hosts, Func<T, ConsoleWriter> question) {
            var toRemove = new List<T>();
            foreach (var host in hosts) {
                if (AskForYes(question(host))) {
                    toRemove.Add(host);
                }
                Console.WriteLine();
            }

            return toRemove;
        }

        public static void PrintWithPaging<T>(IEnumerable<T> list, Func<T, int, ConsoleWriter> line, int countAll = -1) {
            var array = list as T[] ?? list.ToArray();

            var found = array.Length;
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
            foreach (var x in array) {
                var writer = line(x, lineNumber++);
                if (writer != null) {
                    writer.Flush();
                } else {
                    Console.WriteLine();
                }

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