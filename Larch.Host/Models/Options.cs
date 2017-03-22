using System.Text;
using CommandLine;
using CommandLine.Text;


namespace Larch.Host.Models {
    internal class Options {
        [ValueOption(0)]
        public string Value { get; set; }

        [Option('e', "edit", HelpText = "Edit the hosts file in editor. set %EDITOR% to use your favorite editor.")]
        public bool Edit { get; set; }

        [Option('l', "list", HelpText = "list host in hosts file")]
        public bool List { get; set; }

        [Option('a', "add", HelpText = "Add to hosts file")]
        public bool Add { get; set; }

        [Option('r', "remove", HelpText = "Remove from hosts file")]
        public bool Remove { get; set; }

        [Option('f', "force", HelpText = "Use force (e.g. force remove)")]
        public bool Force { get; set; }

        [Option('i', "ip", HelpText = "Use Ip address for list")]
        public bool Ip { get; set; }

        [Option('d', "debug", HelpText = "Enables debuging")]
        public bool Debug { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage() {
            var sb = new StringBuilder();
            sb.AppendLine(" Usage: hosts [OPTIONS] VALUE");
            sb.AppendLine(" Shorthand for add: hosts VALUE");

            var helpText = new HelpText() {
                Heading = sb.ToString(),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true,
                Copyright = "Copyright 2017 René Larch"
            };

            helpText.AddOptions(this);

            return helpText;
        }
    }
}