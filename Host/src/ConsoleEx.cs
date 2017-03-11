using System;


namespace Host {
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
    }
}