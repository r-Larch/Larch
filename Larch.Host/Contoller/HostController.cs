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

            using (new Watch("read file")) {
                hosts = _hostsFile.GetHosts().OrderBy(_ => _.Domain).ToList();
            }

            var startsWith = new List<HostsFileLine>();
            var contains = new List<HostsFileLine>();
            using (new Watch("filter")) {
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

            using (new Watch("read file")) {
                hosts = _hostsFile.GetHosts().OrderBy(_ => _.Domain).ToList();
            }

            var startsWith = new List<HostsFileLine>();
            var contains = new List<HostsFileLine>();
            using (new Watch("filter")) {
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
            using (new Watch("print")) {
                ConsoleEx.PrintWithPaging(
                    list: filterd.SelectMany(x => x),
                    line: (host, nr) => $"{host.LineNumber,6}| {host.Ip}   {host.Domain}",
                    search: search,
                    countAll: hosts.Count
                    );
            }
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