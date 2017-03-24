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

        public void List(Func<HostsFileLine, string> property, Filter filter) {
            List<HostsFileLine> hosts;

            using (new Watch("read file")) {
                hosts = _hostsFile.GetHosts().ToList();
            }

            List<Match<HostsFileLine>> matches;
            using (new Watch("filter")) {
                matches = hosts.Select(x => filter.GetMatch(property(x), x)).Where(x => x.IsSuccess).ToList();
            }

            Print(matches, hosts.Count);
        }


        private void Print(List<Match<HostsFileLine>> matches, int countAll = -1) {
            using (new Watch("print")) {
                ConsoleEx.PrintWithPaging(
                    matches: matches,
                    line: (host, nr) => $"{host.LineNumber,6}| {host.Ip}   {host.Domain}",
                    countAll: countAll
                    );
            }
        }


        public void Remove(string s, bool force) {
            int line;
            if (!int.TryParse(s, out line)) {
                line = -1;
            }

            // TODO wildcarts and regex
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
            hosts.ForEach(x => Console.WriteLine($"removed: {HostsFile.CreateTextLine(x)}"));
        }
    }
}