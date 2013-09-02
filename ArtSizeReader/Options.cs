using CommandLine;
using CommandLine.Text;

namespace ArtSizeReader {

    public class Options {
        [Option('i', "input", Required = true,
          HelpText = "A file or directory to analyze.")]
        public string InputFile { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }
        [Option('l', "logfile",
          HelpText = "Writes output into the specified file. If no directory is given, the current directory will be used.")]
        public string Logfile { get; set; }

        [Option('t', "threshold", Required = true,
          HelpText = "Cover sizes above this threshold (in pixels) will be ignored. Format example: 400x400.")]
        public string Threshold { get; set; }

        [Option('p', "playlist",
          HelpText = "Creates a M3U playlist with all scanned tracks below the threshold. Use to quickly load all affected files into your favorite tag editor or media player.")]
        public string Playlist { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}