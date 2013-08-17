using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HundredMilesSoftware.UltraID3Lib;

namespace ArtSizeReader {

    /// <summary>
    /// Exposes the ArtReader, which supports the analysis of a file or directory with various options.
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
        public ArtReader Create() {
            ArtReader reader = new ArtReader();
            try {
                // Set up logfile.
                if (logfile != null) {
                    Console.WriteLine("Logging enabled, writing log to: " + logfile);
                    logger = InitialiseLogging();
                    Console.SetOut(logger);
                    reader.logfile = logfile;
                    hasLog = true;
                }

                // Check if target path is valid.
                reader.targetPath = targetPath;
                validatePath(reader.targetPath);

                // Check and Parse resolution.
                if (this.threshold != null) {
                    reader.resolution = ParseResolution();
                    hasThreshold = true;
                    Console.WriteLine("Threshold enabled, selected value: " + threshold);
                }
            }
            catch (Exception e) {
                throw new ArgumentException("One or more parameters are invalid: " + e.Message);
            }
            return reader;
        }

        private void validatePath(string targetPath) {
            try {
                targetPath = Path.GetFullPath(targetPath);
            }
            catch (Exception e) {
                Console.WriteLine("Could not find target path: " + e.Message);
                Console.WriteLine("for path " + targetPath);
                throw new ArgumentException("Invalid target path");
            }
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

        /// <summary>
        /// Specifies the file or path that will be analysed.
        /// </summary>
        /// <param name="toRead">The file or path to analyse.</param>
        /// <returns></returns>
        public IArtReader ToRead(string toRead) {
            this.targetPath = toRead;
            return this;
        }

        /// <summary>
        /// Specifies the filename and path of the logfile.
        /// </summary>
        /// <param name="logfile">The path and filename of the logfile</param>
        /// <returns></returns>
        public IArtReader WithLogfile(string logfile) {
            this.logfile = logfile;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public IArtReader WithThreshold(string threshold) {
            this.threshold = threshold;
            return this;
        }

        /// <summary>
        /// Analyzes a file for album art and handles checking of the size.
        /// </summary>
        /// <param name="file">The file to check.</param>
        private void AnalyzeFile(string file) {
            UltraID3 tags = new UltraID3();
            try {
                tags.Read(file);
                ID3FrameCollection covers = tags.ID3v2Tag.Frames.GetFrames(CommonMultipleInstanceID3v2FrameTypes.Picture);
                ID3v2PictureFrame cover = (ID3v2PictureFrame)covers[0];
                Bitmap image = new Bitmap((Image)cover.Picture);
                if (hasThreshold && !CheckSize(image)) {
                    Console.WriteLine("Checked Artwork size for file " + file + " is below limit: " + image.Size.Width + "x" + image.Size.Height);
                }
            }
            catch (Exception e) {
                Console.WriteLine("No cover found for: " + file);
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
        /// <returns>A StreamWriter targeting the path of the logfile.</returns>
        private StreamWriter InitialiseLogging() {
            try {
                string checkedPath = Path.GetFullPath(logfile);
                StreamWriter writer = new StreamWriter(logfile, true);
                return writer;
            }
            catch (Exception e) {
                Console.WriteLine("Could not create logfile: " + e.Message);
                Console.WriteLine("for path " + logfile);
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Parses the resolution from a WIDTHxHEIGHT string into an array.
        /// </summary>
        /// <returns>A uint[2] array containing the width in the first and height in the second field.</returns>
        private uint[] ParseResolution() {
            try {
                return threshold.Split('x').Select(uint.Parse).ToArray();
            }
            catch (Exception e) {
                // Resolution is < 0 or doesn't fit into the uint Array
                Console.Error.WriteLine("Can not parse resolution, must be in format e.g.: 300x300");
                throw new ArgumentException("Invalid resolution string");
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
                    Console.Write("\r{0} of {1} ({2}%) finished.", ++i, numOfFiles, ((float)i / (float)numOfFiles) * 100);
                    Console.SetOut(logger);
                }
                else {
                    Console.Clear();
                    Console.Write("\r{0} of {1} ({2}%) finished.", ++i, numOfFiles, ((float)i / (float)numOfFiles) * 100);
                }
                yield return currentFile;
            }
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
                Console.WriteLine("Could not create logfile: " + e.Message);
                Console.WriteLine("for path " + logfile);
                throw new ArgumentException("Unable to create logfile");
            }
        }
    }
}