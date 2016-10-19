using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;

namespace Host {
    public class Program {
        private bool _showHelp;
        private string _exeFileName;


        public static void Main(string[] args) {
            var p = new Program();
            p.Run(args);
        }


        private void Run(string[] args) {
            var host = new HostController();

            _exeFileName = Environment.GetCommandLineArgs()[0];
            try {
                var p = new OptionSet() {
                    {"e|edit",      "Edit the hosts file",              host.Edit},
                    {"a|add=",      "Add to hosts file",                host.Add},
                    {"s|search=",   "Search for host in hosts file",    host.SearchHost},
                    {"i|searchIp=", "Search for IP in hosts file",      host.SearchIp},
                    {"h|help|?",    "show this message and exit",        v => _showHelp = v != null}
                };

                var extra = p.Parse(args);
                if (_showHelp || args.Length == 0) {
                    ShowHelp(p);
                    return;
                }
                if (extra != null && extra.Any()) {
                    Console.WriteLine("Unknown args: ");
                    foreach (var e in extra) {
                        Console.WriteLine(e);
                    }
                    Console.WriteLine("Press any key to exit ...");
                    Console.ReadKey();
                    return;
                }
            } catch (OptionException e) {
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine($"Try '{_exeFileName} --help' for more information.");
            }
            
        }


        private void ShowHelp(OptionSet p) {
            Console.WriteLine($"Usage: {_exeFileName} [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}