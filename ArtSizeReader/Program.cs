using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace ArtSizeReader {
    public class Program {
        private const int UncaughtException = 5;
        [STAThread]
        private static void Main(string[] args) {
#if DEBUG
            // Translates Exceptions and other messages to english.
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
#endif
            // Install global unhandled exception trapper
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            if (ParseOptions(args)) {
                Console.WriteLine("\nFinished!");
            }
            else {
                Console.WriteLine("\nFinished with errors!");

                // Wait for user input/keep cmd window open.
                //// Console.ReadLine();
            }
#if DEBUG
            // Wait for user input/keep cmd window open.
            Console.ReadLine();
#endif
        }

        /// <summary>
        /// Handles the parsing of the supplied program arguments.
        /// </summary>
        /// <param name="args">Main method's arguments.</param>
        /// <returns>True if everything went well, false if an error occured.</returns>
        private static bool ParseOptions(string[] args) {
            // Get command line parser
            Options options = new Options();
            if (Parser.Default.ParseArguments(args, options)) {
                ArtReader ar;

                // If either one or both options are present, continue.
                if (!(options.Size == null && options.Threshold == null)) {
                    ar = new ArtReader();
                }

                // If neither of the options are present, we can't continue. Show the error and the helpscreen.
                else {
                    HelpText t = HelpText.AutoBuild(options, (HelpText current) => HelpText.DefaultParsingErrorsHandler(options, current));
                    t.AdditionalNewLineAfterOption = true;
                    t.AddPreOptionsLine("ERROR(S):\n  -t/--threshold and/or -s/--size are required.");
                    Console.WriteLine(t.ToString());
                    return false;
                }

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
        /// Event is raised when CLR is not able to find referenced assemblies.
        /// Used to call the libraries that were compiled into the assembly (no reliance on DLL files).
        /// Source: <a href="http://sanganakauthority.blogspot.co.uk/2012/03/creating-single-exe-that-depends-on.html" />.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns>Assembly to be loaded from embeded resource.</returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            // This handler is called only when the common language runtime tries to bind to the assembly and fails.
            // Retrieve the list of referenced assemblies in an array of AssemblyName.
            Assembly objExecutingAssemblies;
            byte[] assemblyData = null;

            objExecutingAssemblies = Assembly.GetExecutingAssembly();
            AssemblyName[] arrReferencedAssmbNames = objExecutingAssemblies.GetReferencedAssemblies();

            // Loop through the array of referenced assembly names.
            foreach (AssemblyName strAssmbName in arrReferencedAssmbNames) {
                // Check for the assembly names that have raised the "AssemblyResolve" event.
                if (strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) == args.Name.Substring(0, args.Name.IndexOf(","))) {
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArtSizeReader." + new AssemblyName(args.Name).Name + ".dll");

                    assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    break;
                }
            }

            // Return the loaded assembly.
            return Assembly.Load(assemblyData);
        }

        /// <summary>
        /// Gracefully handle any unforseen exceptions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e) {
            Console.WriteLine("An unhandled exception occured! Information: ");
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Can not continue, press any key to quit.");
            Console.ReadLine();
            Environment.Exit(UncaughtException);
        }
    }
}