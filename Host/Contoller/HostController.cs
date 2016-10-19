using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Host {
    internal class HostController {
        private static readonly string HostsFilePath = Environment.ExpandEnvironmentVariables(@"%WINDIR%\System32\drivers\etc\hosts");

        public HostController() {
        }

        public void Add(string host) {
            host = host.Trim();

            using (var file = File.Open(HostsFilePath, FileMode.Append, FileAccess.Write)) {
                using (var fw = new StreamWriter(file)) {
                    fw.Write($"127.0.0.0 {host}");
                }
            }
        }

        public void Edit(string s) {
            var editor = new Editor(
                file: HostsFilePath,
                x: 0,
                y: 0,
                width: Console.WindowWidth,
                height: Console.WindowHeight
                );

            editor.Init();
        }

        public void SearchHost(string s) {
            var w = Console.WindowWidth;
            var h = Console.WindowHeight;

            Console.WriteLine($"width: {w}");
            Console.WriteLine($"height: {h}");
        }

        public void SearchIp(string s) {
            Console.WriteLine("SearchIp");
        }
    }
}