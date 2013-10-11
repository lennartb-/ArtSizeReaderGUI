using System;
using CommandLine;

namespace ArtSizeReader {

    public class Program {

        private const int UNCAUGHT_EXCEPTION = 5;

        private static void Main(string[] args) {
            // Translates Exceptions and other messages to english.
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("en-US");

            // Install global unhandled exception trapper
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            if (ParseOptions(args)) {
                Console.WriteLine("\nFinished!");
            }
            else {
                Console.WriteLine("\nFinished with errors!");
                // Wait for user input/keep cmd window open.
                //Console.ReadLine();
            }

            // Wait for user input/keep cmd window open.
            Console.ReadLine();
        }

        private static bool ParseOptions(string[] args) {
            // Get command line parser
            var options = new Options();

            if (Parser.Default.ParseArguments(args, options)) {
                ArtReader ar = new ArtReader();

                // Check if we have a target.
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

                // Check if output will be logged to playlist.
                if (options.Playlist != null) {
                    ar.WithPlaylist(options.Playlist);
                }

                // Check if the covers should be checked for a 1:1 ratio.
                if (options.Ratio) {
                    ar.WithRatio(true);
                }

                // Check if a maximum file size is set.
                if (options.Size != null) {
                    ar.WithSize(options.Size);
                }

                // Check if the have a maximum threshold.
                if (options.MaxThreshold != null) {
                    ar.WithMaxThreshold(options.MaxThreshold);
                }

                try {
                    // Create object and start analyzing the files.
                    ar.Create().GetAlbumArt();
                    return true;
                }
                catch (ArgumentException ae) {
                    Console.WriteLine("Error: " + ae.Message);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Gracefully handle any unforseen exceptions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e) {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Can not continue, press any key to quit.");
            Console.ReadLine();
            Environment.Exit(UNCAUGHT_EXCEPTION);
        }
    }
}