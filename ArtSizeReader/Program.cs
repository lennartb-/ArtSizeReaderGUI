using System;
using CommandLine;

namespace ArtSizeReader {
    class Program {
        static void Main(string[] args) {

            // Translates Exceptions and other messages to english.
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("en-us");

            // Get command line parser
            var options = new Options();

            if (Parser.Default.ParseArguments(args, options)) {

                ArtReader ar = new ArtReader();
                // Check if we either have a single file...
                if (options.InputFile != null) {
                    ar.toRead(options.InputFile);
                }
                // Check if a resolution limit is set.
                if (options.Threshold != null) {
                    ar.withThreshold(options.Threshold);
                }
                // Check if output will be logged to file.
                if (options.Logfile != null) {
                    ar.withLogfile(options.Logfile);
                }
                // Create object and start analyzing the files.
                ar.create();
                ar.getAlbumArt();

                // Wait for user input/keep cmd window open.
                Console.ReadLine();
            }
        }




    }


}
