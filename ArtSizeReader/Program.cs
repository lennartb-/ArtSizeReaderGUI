using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace ArtSizeReader {
    class Program {
        private static bool loggingEnabled = false;
        private static string loggingPath = String.Empty;
        static void Main(string[] args) {

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("en-us");

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
                ar.create();
                ar.getAlbumArt();               
                
                Console.ReadLine();
            }
        }

        


    }


}
