using System;


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

        public static ConsoleKey WaitForYesNo() {
            while (true) {
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Y || key == ConsoleKey.N) {
                    return key;
                }
            }
        }

        public static void PrintHighlighted(string line, string s, ConsoleColor color = ConsoleColor.Red) {
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
    }
}