using System;
using IIS.Controller;
using IIS.Models;
using LarchConsole;
using Microsoft.Web.Administration;


namespace IIS {
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
                Environment.ExitCode = e.HResult == 0 ? -1 : e.HResult;
            }
        }


        private void Run(Options options) {
            var iisManager = new ServerManager();
            var iis = new IisController(iisManager);

            var filter = new Filter(options.Value,
                options.Regex
                    ? CampareType.Regex
                    : CampareType.WildCard,
                CompareMode.CaseIgnore
                );

            var filterProp = FilterProp.Binding;
            if (options.Site) {
                filterProp = FilterProp.Name;
            }

            if (options.Id) {
                filterProp = FilterProp.Id;
            }

            if (options.State) {
                filterProp = FilterProp.State;
            }


            // list
            if (options.List) {
                filter.OnEmptyMatchAll = true;
                iis.List(filter, filterProp);
                return;
            }

            // handle empty value
            if (string.IsNullOrEmpty(options.Value)) {
                Console.WriteLine(options.GetUsage());
                return;
            }

            // remove value
            if (options.Remove) {
                iis.Remove(filter, filterProp, options.Force);
                return;
            }

            // add value
            if (options.Add || !string.IsNullOrEmpty(options.Value)) {
                iis.Add(options.Value, filterProp);
                return;
            }
        }
    }
}