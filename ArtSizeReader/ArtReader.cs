using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HundredMilesSoftware.UltraID3Lib;

namespace ArtSizeReader {

    /// <summary>
    /// Exposes the fluent inteface for ArtReader, which supports the analysis of a file or directory with various options.
    /// </summary>
    public interface IArtReader {
        ArtReader Create();

        IArtReader ToRead(string toRead);

        IArtReader WithLogfile(string logfile);

        IArtReader WithThreshold(string resolution);
    }

    public class ArtReader : IArtReader {

        // Preserve standard Console output
        private static StreamWriter defaultConsoleOutput = new StreamWriter(Console.OpenStandardOutput());

        private bool hasLog = false;
        private bool hasThreshold = false;
        private string logfile;
        private StreamWriter logger = defaultConsoleOutput;
        private uint[] resolution;
        private string targetPath;
        private string threshold;

        /// <summary>
        /// Builds an ArtReader object from the specified parameters and checks if they are valid.
        /// </summary>
        /// <returns>An ArtReader objects with the desired input parameters.</returns>
        /// <exception cref="ArgumentException">Thrown when any of the supplied arguments are invalid.</exception>
        public ArtReader Create() {
            ArtReader reader = new ArtReader();

            // Set up logfile.
            if (this.logfile != null) {
                if (InitialiseLogging()) {
                    reader.logfile = logfile;
                    reader.logger = logger;
                    reader.hasLog = true;
                    Console.WriteLine("Logging enabled, writing log to: " + logfile);
                }
                else throw new ArgumentException("Invalid logfile path: " + logfile);
            }

            // Check if target path is valid.
            if (IsPathValid(targetPath)) {
                reader.targetPath = targetPath;
            }
            else throw new ArgumentException("Invalid target path: " + targetPath);

            // Check and Parse resolution.
            if (this.threshold != null && ParseResolution()) {
                reader.resolution = resolution;
                reader.hasThreshold = true;
                Console.WriteLine("Threshold enabled, selected value: " + resolution[0] + "x" + resolution[1]);
            }
            else throw new ArgumentException("Invalid resolution: " + threshold);

            return reader;
        }

        /// <summary>
        /// Starts fetching the album art from the specified file or directory.
        /// </summary>
        public void GetAlbumArt() {
            // Target is a single file
            if (File.Exists(targetPath)) {
                AnalyzeFile(targetPath);
            }

            // Target is a directory
            else if (Directory.Exists(targetPath)) {
                foreach (string file in ReadFiles(targetPath)) {
                    AnalyzeFile(file);
                }
            }
        }
        #region Interface allocation methods
        /// <summary>
        /// Specifies the file or path that will be analysed.
        /// </summary>
        /// <param name="toRead">The file or path to analyse.</param>
        /// <returns>The instance of the current object.</returns>
        public IArtReader ToRead(string toRead) {
            this.targetPath = toRead;
            return this;
        }

        /// <summary>
        /// Specifies the filename and path of the logfile.
        /// </summary>
        /// <param name="logfile">The path and filename of the logfile.</param>
        /// <returns>The instance of the current object.</returns>
        public IArtReader WithLogfile(string logfile) {
            this.logfile = logfile;
            return this;
        }

        /// <summary>
        /// Specifies the art size threshold in the format WIDHTxHEIGHT.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <returns>The instance of the current object.</returns>
        public IArtReader WithThreshold(string threshold) {
            this.threshold = threshold;
            return this;
        }
        #endregion
        #region Private methods
        /// <summary>
        /// Analyzes a file for album art and handles checking of the size.
        /// </summary>
        /// <param name="file">The file to check.</param>
        private void AnalyzeFile(string file) {
            UltraID3 tags = new UltraID3();

            tags.Read(file);
            ID3FrameCollection covers = tags.ID3v2Tag.Frames.GetFrames(CommonMultipleInstanceID3v2FrameTypes.Picture);
            // Check if there actually is a cover.
            if (covers.Count > 0) {
                ID3v2PictureFrame cover = (ID3v2PictureFrame)covers[0];
                Bitmap image = new Bitmap((Image)cover.Picture);
                if (hasThreshold && !CheckSize(image)) {
                    Console.WriteLine("\nChecked Artwork size for file " + file + " is below limit: " + image.Size.Width + "x" + image.Size.Height);
                }
            }
            // No covers found.
            else {
                Console.WriteLine("\nNo cover found for: " + file);
            }

        }

        /// <summary>
        /// Checks whether the size of an image is below the global threshold.
        /// </summary>
        /// <param name="image">The image to check.</param>
        /// <returns>false if the image is below the limit, true if not.</returns>
        private bool CheckSize(Bitmap image) {
            if (image.Size.Width < resolution[0] || image.Size.Height < resolution[1]) {
                return false;
            }
            else return true;
        }

        /// <summary>
        /// Manages the initialisation of the logfile.
        /// </summary>
        /// <returns>true if the path is valid, false when not.</returns>
        private bool InitialiseLogging() {
            try {
                string logfilePath = Path.GetFullPath(logfile);
                bool validDir = Directory.Exists(Path.GetDirectoryName(logfilePath));
                if (validDir) {                    
                    FileStream fs = new FileStream(logfilePath, FileMode.Append);
                    logger = new StreamWriter(fs);
                    logger.AutoFlush = true;
                    Console.SetOut(logger);
                    hasLog = true;
                    return true;
                }
                else return false;
            }
            catch (Exception e) {
                Console.WriteLine("Could not create logfile: " + e.Message);
                Console.WriteLine("for path " + logfile);
                return false;
            }
        }

        /// <summary>
        /// Parses the resolution from a WIDTHxHEIGHT string into an array.
        /// </summary>
        /// <returns>true if the resolution is valid, false when not.</returns>
        private bool ParseResolution() {
            try {
                resolution = threshold.Split('x').Select(uint.Parse).ToArray();
                hasThreshold = true;
                return true;
            }
            catch (FormatException fe) {
                // Resolution is < 0 or doesn't fit into the uint Array                
                Console.WriteLine("Can not parse resolution, must be in format e.g.: 300x300");
                Console.WriteLine(fe.Message);
                return false;
            }
        }

        /// <summary>
        /// Enumerates the files in a certain directory and returns one file at a time.
        /// </summary>
        /// <param name="directory"> The directory to check.</param>
        /// <returns>An IEnumerable containing all found files.</returns>
        private IEnumerable<string> ReadFiles(string directory) {
            IEnumerable<string> musicFiles;
            int numOfFiles;

            // Get all files in the directory.
            try {
                musicFiles = Directory.EnumerateFiles(directory, "*.mp3", SearchOption.AllDirectories);
                numOfFiles = musicFiles.Count();
            }
            catch (UnauthorizedAccessException uae) {
                Console.WriteLine(uae.Message);
                yield break;
            }
            catch (PathTooLongException ptle) {
                Console.WriteLine(ptle.Message);
                yield break;
            }

            int i = 0;
            foreach (string currentFile in musicFiles) {
                // If logging to file is enabled, print out the progress to console anyway.
                if (hasLog) {
                    Console.SetOut(defaultConsoleOutput);
                    Console.Write("\r{0} of {1} ({2}%) finished.{3}", ++i, numOfFiles, ((float)i / (float)numOfFiles) * 100, new String(' ', 10));
                    Console.SetOut(logger);
                }
                else {
                    /* Print out progress. Argument {3} ensures that any text right of the progress is cleared,
                     * otherwise old chars are not removed, since the number of decimal places of the percentage may vary.*/
                    Console.Write("\r{0} of {1} ({2}%) finished.{3}", ++i, numOfFiles, ((float)i / (float)numOfFiles) * 100, new String(' ', 10));
                }
                yield return currentFile;
            }
        }

        /// <summary>
        /// Checks if the given path is a valid Windows path.
        /// </summary>
        /// <param name="targetPath">The path to check.</param>
        /// <returns>true if the path is valid, false when not.</returns>
        private bool IsPathValid(string targetPath) {
            if (!Directory.Exists(targetPath) && !File.Exists(targetPath)) {
                Console.WriteLine("Could not find target path: " + targetPath);
                return false;
            }
            else return true;
        }

        /// <summary>
        /// Writes a line to into the logfile.
        /// </summary>
        /// <param name="line">The line to write.</param>
        private void WriteToLogFile(string line) {
            try {
                Console.WriteLine(line);
            }
            catch (Exception e) {
                Console.WriteLine("Could not write to logfile: " + e.Message);
                Console.WriteLine("for path " + logfile);
                throw new ArgumentException("Unable to write to logfile");
            }
        }
        #endregion
    }
}