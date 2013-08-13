using CommandLine;
using CommandLine.Text;


namespace ArtSizeReader {
    class Options {
        [Option('i', "input", Required = true,
          HelpText = "A file or directory to analyze.")]
        public string InputFile { get; set; }

        [Option('t', "threshold", Required = false,
          HelpText = "Cover sizes above this threshold (in pixels) will be ignored. Format example: 400x400")]
        public string Threshold { get; set; }

        [Option('v', "verbose", DefaultValue = true,
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('l', "logfile",
          HelpText = "Writes output into the specified file. If no directory is given, the current directory will be used.")]
        public string Logfile { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
