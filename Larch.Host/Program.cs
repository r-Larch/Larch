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
                if (CommandLine.Parser.Default.ParseArguments(args, options)) {
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

            // list
            if (options.List) {
                if (options.Ip) {
                    host.SearchIp(options.Value);
                    return;
                }

                host.SearchHost(options.Value);
                return;
            }

            // handle empty value
            if (string.IsNullOrEmpty(options.Value)) {
                Console.WriteLine(options.GetUsage());
                return;
            }

            // remove value
            if (options.Remove) {
                host.Remove(options.Value, options.Force);
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