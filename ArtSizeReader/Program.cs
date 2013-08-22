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

                if (options.Logfile != null && options.Silent) {
                    Console.Error.WriteLine("Can not combine --logfile with --silent, they are mutually exclusive.");
                    Environment.Exit(4);
                }

                if (options.Silent) {
                    ar.IsSilent(true);
                }

                // Check if we either a target
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
                    Console.Error.WriteLine("Error: " + ae.Message);
                }
            }
            Console.WriteLine("\nFinished!");
            // Wait for user input/keep cmd window open.
            Console.ReadLine();
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e) {
            Console.Error.WriteLine(e.ExceptionObject.ToString());
            Console.Error.WriteLine("Can not continue, press any key to quit.");
            Console.ReadLine();
            Environment.Exit(5);
        }
    }
}