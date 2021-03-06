﻿using CommandLine;
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

        [Option('t', "threshold",
          HelpText = "Cover sizes above this threshold (in pixels) will be ignored. Required if -s/--size is not used. Format example: 400x400.")]
        public string Threshold { get; set; }

        [Option('s', "size",
          HelpText = "Cover file sizes below this threshold (in kilobytes) will be ignored. Useful to check for large image files. Required if -t/--threshold is not used. Format example: 1000 (equals 1 Megabyte).")]
        public double? Size { get; set; }

        [Option('p', "playlist",
          HelpText = "Creates a M3U playlist with all scanned tracks below the threshold. Use to quickly load all affected files into your favorite tag editor or media player.")]
        public string Playlist { get; set; }

        [Option('r', "ratio",
          HelpText = "Additionally restrict the cover size to a 1:1 aspect ratio. If enabled e.g 400x350 will cause an error while 400x400 won't.")]
        public bool Ratio { get; set; }

        [Option('m', "max-threshold",
          HelpText = "Can be used together with -t to define a maximum upper limit for covers, i.e. covers larger than this resolution will be reported. Format example: 1000x1000.")]
        public string MaxThreshold { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}