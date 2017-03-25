using System;
using Larch.Host.Contoller;
using Larch.Host.Models;
using Larch.Host.Parser;


namespace Larch.Host {
    public class Program {
        public static void Main(string[] args) {
            try {
                var p = new Program();
                var options = new Options();

                var parser = new CommandLine.Parser(settings => settings.CaseSensitive = true);
                if (parser.ParseArguments(args, options)) {
                    p.Run(options);
                } else {
                    // print help
                    Console.WriteLine(options.GetUsage());
                }

                if (options.Debug) {
                    Watch.PrintTasks();
                }
            } catch (Exception e) {
                ConsoleEx.PrintException(e.Message, e);
            }
        }


        private void Run(Options options) {
            var hostfile = new HostsFile();
            var host = new HostController(hostfile);

            // edit
            if (options.Edit) {
                host.Edit();
                return;
            }

            var filter = new Filter(options.Value,
                options.Regex
                    ? CampareType.Regex
                    : CampareType.WildCard,
                CompareMode.CaseIgnore
                );

            // list
            if (options.List) {
                filter.OnEmptyMatchAll = true;

                if (options.Ip) {
                    host.List(filter, FilterProp.Ip);
                    return;
                }
                if (options.Line) {
                    host.List(filter, FilterProp.Line);
                    return;
                }

                host.List(filter, FilterProp.Domain);
                return;
            }

            // handle empty value
            if (string.IsNullOrEmpty(options.Value)) {
                Console.WriteLine(options.GetUsage());
                return;
            }

            // remove value
            if (options.Remove) {
                if (options.Ip) {
                    host.Remove(filter, FilterProp.Ip, options.Force);
                    return;
                }
                if (options.Line) {
                    host.Remove(filter, FilterProp.Line, options.Force);
                    return;
                }

                host.Remove(filter, FilterProp.Domain, options.Force);
                return;
            }

            // add value
            if (options.Add || !string.IsNullOrEmpty(options.Value)) {
                host.Add(options.Value);
                return;
            }
        }
    }
}