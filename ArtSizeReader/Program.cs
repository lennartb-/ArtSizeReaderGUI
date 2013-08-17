using System;
using CommandLine;

namespace ArtSizeReader {

    public class Program {
        private static void Main(string[] args) {
            // Translates Exceptions and other messages to english.
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("en-us");

            // Install global unhandled exception trapper
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            // Get command line parser
            var options = new Options();

            if (Parser.Default.ParseArguments(args, options)) {
                ArtReader ar = new ArtReader();

                // Check if we either have a single file...
                if (options.InputFile != null) {
                    ar.ToRead(options.InputFile);
                }

                // Check if a resolution limit is set.
                if (options.Threshold != null) {
                    ar.WithThreshold(options.Threshold);
                }

                // Check if output will be logged to file.
                if (options.Logfile != null) {
                    ar.WithLogfile(options.Logfile);
                }

                try {
                    // Create object and start analyzing the files.
                    ar.Create().GetAlbumArt();
                }
                catch (ArgumentException ae) {
                    Console.WriteLine("Error: "+ae.Message);
                }

                // Wait for user input/keep cmd window open.
                Console.ReadLine();
            }
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e) {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Can not continue, press any key to quit.");
            Console.ReadLine();
            Environment.Exit(5);
        }
    }
}