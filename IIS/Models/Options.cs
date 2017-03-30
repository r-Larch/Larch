using System.Text;
using CommandLine;
using CommandLine.Text;


namespace IIS.Models {
    internal class Options {
        [ValueOption(0)]
        public string Value { get; set; }

        [Option('l', "list", HelpText = "List using wildcards or regex")]
        public bool List { get; set; }

        [Option('a', "add", HelpText = "Add binding or site")]
        public bool Add { get; set; }

        [Option('r', "remove", HelpText = "Remove binding or site")]
        public bool Remove { get; set; }

        [Option('f', "force", HelpText = "Use force (e.g. force remove)")]
        public bool Force { get; set; }

        [Option('s', "site", HelpText = "Filter by site")]
        public bool Site { get; set; }

        [Option('i', "id", HelpText = "Filter by id")]
        public bool Id { get; set; }

        [Option('t', "state", HelpText = "Filter by state")]
        public bool State { get; set; }

        [Option('R', "regex", HelpText = "Use regex for filter")]
        public bool Regex { get; set; }

        [Option('d', "debug", HelpText = "Enables debuging")]
        public bool Debug { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage() {
            var sb = new StringBuilder();
            sb.AppendLine(" Usage: iis [OPTIONS] VALUE");
            sb.AppendLine(" Shorthand for add: iis VALUE");

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