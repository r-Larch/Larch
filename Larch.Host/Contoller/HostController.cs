using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Larch.Host.Models;


namespace Larch.Host.Contoller {
    internal class HostController {
        private static readonly string HostsFilePath = Environment.ExpandEnvironmentVariables(@"%WINDIR%\System32\drivers\etc\hosts");

        public void Add(string host) {
            host = host.Trim();
            var line = $"127.0.0.1 {host}";

            using (var file = File.Open(HostsFilePath, FileMode.Append, FileAccess.Write)) {
                using (var fw = new StreamWriter(file)) {
                    fw.WriteLine(line);
                }
            }

            Console.WriteLine($"added successfully '{line}'");
            Console.WriteLine();
        }

        public void Edit() {
            Executor.OpenEditor(new FileInfo(HostsFilePath)).StartNormal();
        }

        public void SearchHost(string s) {
            List<HostEntry> hosts;

            using (new Watch("GetHosts")) {
                hosts = GetHosts().OrderBy(_ => _.Domain).ToList();
            }

            var startsWith = new List<HostEntry>();
            var contains = new List<HostEntry>();
            using (new Watch("Filter")) {
                foreach (var host in hosts) {
                    if (host.Domain.StartsWith(s, StringComparison.InvariantCultureIgnoreCase)) {
                        startsWith.Add(host);
                        continue;
                    }

                    if (host.Domain.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) != -1) {
                        contains.Add(host);
                    }
                }
            }

            Print(hosts, s, startsWith, contains);
        }


        public void SearchIp(string s) {
            List<HostEntry> hosts;

            using (new Watch("GetHosts")) {
                hosts = GetHosts().OrderBy(_ => _.Domain).ToList();
            }

            var startsWith = new List<HostEntry>();
            var contains = new List<HostEntry>();
            using (new Watch("Filter")) {
                foreach (var host in hosts) {
                    if (host.Ip.StartsWith(s, StringComparison.InvariantCultureIgnoreCase)) {
                        startsWith.Add(host);
                        continue;
                    }

                    if (host.Ip.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) != -1) {
                        contains.Add(host);
                    }
                }
            }

            Print(hosts, s, startsWith, contains);
        }

        public IEnumerable<HostEntry> GetHosts() {
            var regex = new Regex(@"^(\d+.\d+.\d+.\d+)[\s|\t]+(\S+)$");
            var lineNum = 0;
            foreach (var line in GetLines()) {
                lineNum++;
                if (line == null) continue;

                var match = regex.Match(line);
                if (match.Success) {
                    yield return new HostEntry() {
                        Ip = match.Groups[1].Value,
                        Domain = match.Groups[2].Value,
                        Line = lineNum
                    };
                }
            }
        }

        private IEnumerable<string> GetLines() {
            using (var fs = File.Open(HostsFilePath, FileMode.Open, FileAccess.Read)) {
                using (var sr = new StreamReader(fs)) {
                    while (!sr.EndOfStream) {
                        var line = sr.ReadLine()?.Trim();
                        yield return line;
                    }
                }
            }
        }


        private void Print(List<HostEntry> hosts, string search, params List<HostEntry>[] filterd) {
            var found = filterd.Sum(x => x.Count);
            var height = Console.WindowHeight;
            var pages = found/height;
            var page = 1;

            if (found < height) {
                Console.WriteLine($"found: {found} matchs in {hosts.Count} entries");
            }
            if (found == 0) {
                return;
            }

            Console.WriteLine();

            using (new Watch("Print")) {
                var count = 0;

                Console.WriteLine("Line  |");
                foreach (var host in filterd.SelectMany(x => x)) {
                    PrintHighlighted($"{host.Line,6}| {host.Ip}   {host.Domain}", search);
                    count++;

                    if (count < height) continue;

                    Console.Write($" -- page: {page++}/{pages} -- ");
                    count = 0;
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape) {
                        break;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            if (found >= height) {
                Console.WriteLine($"found: {found} matchs in {hosts.Count} entries");
            }

            //Watch.PrintTasks();
        }

        private void PrintHighlighted(string line, string s) {
            var start = line.IndexOf(s, StringComparison.InvariantCultureIgnoreCase);
            var end = start + s.Length;
            var chars = line.ToCharArray();

            for (var i = 0; i < chars.Length; i++) {
                if (i == start) {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                if (i == end) {
                    Console.ResetColor();
                }

                Console.Write(chars[i]);
            }

            Console.ResetColor();
            Console.Write("\r\n");
        }

        public void Remove(string s) {
            int line;
            if (!int.TryParse(s, out line)) {
                line = -1;
            }
            var hosts = GetHosts()
                .Where(x => x.Domain == s || x.Line == line)
                .OrderBy(x => x.Domain)
                .ToList();

            Console.WriteLine($"found {hosts.Count} to remove\r\n");

            var toRemove = new List<int>();
            foreach (var host in hosts) {
                Console.Write($"remove '{host.Ip} {host.Domain}'? (Y/N) ");
                var key = ConsoleEx.WaitForYesNo();
                if (key == ConsoleKey.Y) {
                    toRemove.Add(host.Line);
                }
                Console.WriteLine();
            }

            RemoveLines(toRemove);
        }


        public void RemoveForce(string s) {
            int line;
            if (!int.TryParse(s, out line)) {
                line = -1;
            }
            var hosts = GetHosts()
                .Where(x => x.Domain == s || x.Line == line)
                .OrderBy(x => x.Domain)
                .ToList();

            if (hosts.Count == 0) {
                Console.WriteLine($"-- nothing to remove");
                return;
            }

            RemoveLines(hosts.Select(x => x.Line));
        }

        private void RemoveLines(IEnumerable<int> toRemove) {
            var remove = toRemove as int[] ?? toRemove.ToArray();
            if (remove.Length == 0) {
                Console.WriteLine($"-- nothing to remove");
                return;
            }

            var lineNum = 0;
            var lines = new List<string>();
            foreach (var line in GetLines()) {
                lineNum++;
                if (!remove.Contains(lineNum)) {
                    lines.Add(line);
                    continue;
                }

                Console.WriteLine($"remove line '{line}'");
            }

            // write file
            using (var file = File.Open(HostsFilePath, FileMode.Truncate, FileAccess.Write)) {
                using (var fw = new StreamWriter(file)) {
                    foreach (var line in lines) {
                        fw.WriteLine(line);
                    }
                }
            }
        }
    }
}