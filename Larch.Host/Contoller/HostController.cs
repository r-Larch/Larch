using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Larch.Host.Parser;


namespace Larch.Host.Contoller {
    internal class HostController {
        private readonly HostsFile _hostsFile;

        public HostController(HostsFile hostsFile) {
            _hostsFile = hostsFile;
        }

        public void Add(string host) {
            string line;
            using (new Watch("add")) {
                line = _hostsFile.Append(new FileLine() {
                    Ip = "127.0.0.1",
                    Domain = host.Trim()
                });
            }

            Console.WriteLine($"added successfully '{line}'");
            Console.WriteLine();
        }

        public void Edit() {
            Executor.OpenEditor(new FileInfo(_hostsFile.FilePath)).StartNormal();
            Console.WriteLine("editor is starting...");
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


        public void Remove(string s, bool force) {
            int line;
            if (!int.TryParse(s, out line)) {
                line = -1;
            }

            List<HostsFileLine> hosts;
            using (new Watch("filter")) {
                hosts = _hostsFile.GetHosts()
                    .Where(x => x.Domain == s || x.LineNumber == line)
                    .OrderBy(x => x.Domain)
                    .ToList();
            }

            if (!force) {
                Console.WriteLine($"found {hosts.Count} to remove\r\n");
                hosts = ConsoleEx.AskYesOrNo(hosts, x => $"remove '{HostsFile.CreateTextLine(x)}'?");
            }

            if (hosts.Count == 0) {
                Console.WriteLine($"-- nothing to remove");
                return;
            }

            using (new Watch("delete")) {
                _hostsFile.Remove(hosts);
            }
            hosts.ForEach(x => Console.WriteLine(HostsFile.CreateTextLine(x)));
        }
    }
}