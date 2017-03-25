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

        public void List(Filter filter, FilterProp what) {
            List<HostsFileLine> hosts;

            using (new Watch("read file")) {
                hosts = _hostsFile.GetHosts().ToList();
            }

            var matches = Filter(filter, what);

            using (new Watch("print")) {
                ConsoleEx.PrintWithPaging(
                    list: matches,
                    countAll: hosts.Count,
                    line: (match, nr) => {
                        switch (what) {
                            default:
                            case FilterProp.Domain:
                                return new ConsoleWriter().Write($"{match.Model.LineNumber,6}| {match.Model.Ip}   ").Write(match);
                            case FilterProp.Ip:
                                return new ConsoleWriter().Write($"{match.Model.LineNumber,6}| ").Write(match).Write($"   {match.Model.Domain}");
                            case FilterProp.Line:
                                return new ConsoleWriter().Write(match, 6).Write($"| {match.Model.Ip}").Write($"   {match.Model.Domain}");
                        }
                    });
            }
        }

        public void Remove(Filter filter, FilterProp what, bool force) {
            var matches = Filter(filter, what);

            if (!force) {
                Console.WriteLine($"found {matches.Count} to remove\r\n");
                matches = ConsoleEx.AskYesOrNo(matches, x => {
                    switch (what) {
                        default:
                        case FilterProp.Domain:
                            return ConsoleWriter.Create($"remove '").Write(x.Model.Ip).Write("    ").Write(x).Write("'?");
                        case FilterProp.Ip:
                            return ConsoleWriter.Create($"remove '").Write(x).Write("    ").Write(x.Model.Domain).Write("'?");
                        case FilterProp.Line:
                            return ConsoleWriter.Create($"remove '").Write(x).Write("| ").Write(x.Model.Ip).Write("    ").Write(x.Model.Domain).Write("'?");
                    }
                });
            }

            if (matches.Count == 0) {
                Console.WriteLine($"-- nothing to remove");
                return;
            }

            using (new Watch("delete")) {
                _hostsFile.Remove(matches.Select(x => x.Model));
            }
            matches.ForEach(x => Console.WriteLine($"removed: {HostsFile.CreateTextLine(x.Model)}"));
        }

        private List<Match<HostsFileLine>> Filter(Filter filter, FilterProp what) {
            using (new Watch("filter")) {
                return _hostsFile.GetHosts().Select(x => filter.GetMatch(x, _ => {
                    switch (what) {
                        default:
                        case FilterProp.Domain:
                            return _.Domain;
                        case FilterProp.Ip:
                            return _.Ip;
                        case FilterProp.Line:
                            return _.LineNumber.ToString();
                    }
                })).Where(x => x.IsSuccess).ToList();
            }
        }
    }


    internal enum FilterProp {
        Domain,
        Ip,
        Line
    }
}