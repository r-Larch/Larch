using System;
using CommandLine;
using Larch.Host.Contoller;
using Larch.Host.Models;


namespace Larch.Host {
    public class Program {
        public static void Main(string[] args) {
            try {
                var p = new Program();
                var options = new Options();
                if (Parser.Default.ParseArguments(args, options)) {
                    p.Run(options);
                } else {
                    // print help
                    Console.WriteLine(options.GetUsage());
                }
            } catch (Exception e) {
                ConsoleEx.PrintException(e.Message, e);
            }
        }


        private void Run(Options options) {
            var host = new HostController();

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
                if (options.Force) {
                    host.RemoveForce(options.Value);
                    return;
                }

                host.Remove(options.Value);
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