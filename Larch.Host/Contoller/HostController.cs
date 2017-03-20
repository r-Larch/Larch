using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Larch.Host.Models;
using Larch.Host.Parser;


namespace Larch.Host.Contoller {
    internal class HostController {
        private readonly HostsFile _hostsFile;

        public HostController(HostsFile hostsFile) {
            _hostsFile = hostsFile;
        }

        public void Add(string host) {
            host = host.Trim();
            var line = $"127.0.0.1 {host}";

            _hostsFile.AppendLine(line);

            Console.WriteLine($"added successfully '{line}'");
            Console.WriteLine();
        }

        public void Edit() {
            Executor.OpenEditor(new FileInfo(_hostsFile.FilePath)).StartNormal();
        }

        public void SearchHost(string s) {
            List<HostsFileLine> hosts;

            using (new Watch("GetHosts")) {
                hosts = _hostsFile.GetHosts().OrderBy(_ => _.Domain).ToList();
            }

            var startsWith = new List<HostsFileLine>();
            var contains = new List<HostsFileLine>();
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
            List<HostsFileLine> hosts;

            using (new Watch("GetHosts")) {
                hosts = _hostsFile.GetHosts().OrderBy(_ => _.Domain).ToList();
            }

            var startsWith = new List<HostsFileLine>();
            var contains = new List<HostsFileLine>();
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




        private void Print(List<HostsFileLine> hosts, string search, params List<HostsFileLine>[] filterd) {
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
                    ConsoleEx.PrintHighlighted($"{host.LineNumber,6}| {host.Ip}   {host.Domain}", search);
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

        public void Remove(string s) {
            int line;
            if (!int.TryParse(s, out line)) {
                line = -1;
            }
            var hosts = _hostsFile.GetHosts()
                .Where(x => x.Domain == s || x.LineNumber == line)
                .OrderBy(x => x.Domain)
                .ToList();

            Console.WriteLine($"found {hosts.Count} to remove\r\n");

            var toRemove = new List<int>();
            foreach (var host in hosts) {
                Console.Write($"remove '{host.Ip} {host.Domain}'? (Y/N) ");
                var key = ConsoleEx.WaitForYesNo();
                if (key == ConsoleKey.Y) {
                    toRemove.Add(host.LineNumber);
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
            var hosts = _hostsFile.GetHosts()
                .Where(x => x.Domain == s || x.LineNumber == line)
                .OrderBy(x => x.Domain)
                .ToList();

            if (hosts.Count == 0) {
                Console.WriteLine($"-- nothing to remove");
                return;
            }

            RemoveLines(hosts.Select(x => x.LineNumber));
        }

        private void RemoveLines(IEnumerable<int> toRemove) {
            var remove = toRemove as int[] ?? toRemove.ToArray();
            if (remove.Length == 0) {
                Console.WriteLine($"-- nothing to remove");
                return;
            }

            // TODO refactor
            //var lineNum = 0;
            //var lines = new List<string>();
            //foreach (var line in _hostsFile.GetLines()) {
            //    lineNum++;
            //    if (!remove.Contains(lineNum)) {
            //        lines.Add(line);
            //        continue;
            //    }

            //    Console.WriteLine($"remove line '{line}'");
            //}

            //// write file
            //using (var file = File.Open(HostsFilePath, FileMode.Truncate, FileAccess.Write)) {
            //    using (var fw = new StreamWriter(file)) {
            //        foreach (var line in lines) {
            //            fw.WriteLine(line);
            //        }
            //    }
            //}
        }
    }
}