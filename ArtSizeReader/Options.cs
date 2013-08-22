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
          HelpText = "Writes output into the specified file. If no directory is given, the current directory will be used. Can't be combined with --silent")]
        public string Logfile { get; set; }

        [Option('t', "threshold", Required = false,
          HelpText = "Cover sizes above this threshold (in pixels) will be ignored. Format example: 400x400")]
        public string Threshold { get; set; }

        [Option('s', "silent", DefaultValue = false,
          HelpText = "Suppresses any output. Can't be combined with --logfile.")]
        public bool Silent { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}