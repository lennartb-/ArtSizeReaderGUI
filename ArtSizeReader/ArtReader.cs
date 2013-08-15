using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HundredMilesSoftware.UltraID3Lib;

namespace ArtSizeReader {
    public class ArtReader : IArtReader {

        private string targetPath;
        private string logfile;
        private uint[] resolution;
        private string threshold;

        // Log to console per default        
        private const StreamWriter defaultConsoleOutput = new StreamWriter(Console.OpenStandardOutput());
        private StreamWriter logger = defaultConsoleOutput;

        private bool hasLog = false;
        private bool hasThreshold = false;

        public IArtReader toRead(string toRead) {
            this.targetPath = toRead;
            return this;
        }

        public IArtReader withLogfile(string logfile) {
            this.logfile = logfile;
            return this;
        }
        public IArtReader withThreshold(string threshold) {
            this.threshold = threshold;
            return this;
        }

        public ArtReader create() {
            ArtReader reader = new ArtReader();
            if (logfile != null) {
                Console.WriteLine("Logging enabled, writing log to: " + logfile);
                logger = initialiseLogging();
                Console.SetOut(logger);
                hasLog = true;
            }
            try {
                reader.targetPath = Path.GetFullPath(targetPath);
            }

            catch (Exception e) {
                Console.WriteLine("Could not find target path: " + e.Message);
                Console.WriteLine("for path " + targetPath);
                throw new ArgumentException();
            }
            if (threshold != null) {
                reader.resolution = parseResolution();
                hasThreshold = true;
                Console.WriteLine("Threshold enabled, selected value: " + threshold);
            }

            return reader;

        }

        /// <summary>
        /// Parses the resolution from a WIDTHxHEIGHT string into an array.
        /// </summary>
        /// <param name="toParse">The string to parse.</param>
        /// <returns>A uint[2] array containing the width in the first and height in the second field.</returns>

        private uint[] parseResolution() {
            try {
                return threshold.Split('x').Select(uint.Parse).ToArray();
            }
            catch (Exception e) {
                // Resolution is < 0 or doesn't fit into the uint Array
                Console.WriteLine("Can not parse Resolution, must be in format e.g.: 300x300");
                throw new InvalidCastException();
            }
        }

        /// <summary>
        /// Starts fetching the album art from the specified file or directory.
        /// </summary>

        public void getAlbumArt() {

            // Target is a single file
            if (File.Exists(targetPath)) {
                analyzeFile(targetPath);
            }

            // Target is a directory
            else if (Directory.Exists(targetPath)) {
                foreach (string file in readFiles(targetPath)) {
                    analyzeFile(file);
                }
            }
        }

        /// <summary>
        /// Enumerates the files in a certain directory and returns one file at a time.
        /// </summary>
        /// <param name="directory"> The directory to check.</param>
        /// <returns>An IEnumerable containing all found files.</returns>

        private IEnumerable<string> readFiles(string directory) {
            IEnumerable<string> musicFiles;
            int numOfFiles;
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
                Console.SetOut(defaultConsoleOutput);
                Console.Write("\r{0} of {1} ({2}%) finished.", i++, numOfFiles, ((float)i / (float)numOfFiles) * 100);
                Console.SetOut(logger);
                yield return currentFile;
            }
        }

        /// <summary>
        /// Checks whether the size of an image is below the global threshold.
        /// </summary>
        /// <param name="image">The image to check.</param>
        /// <returns>false if the image is below the limit, true if not.</returns>

        private bool checkSize(Bitmap image) {
            if (image.Size.Width < resolution[0] || image.Size.Height < resolution[1]) {
                return false;
            }
            else return true;
        }

        /// <summary>
        /// Analyzes a file for album art and handles checking of the size.
        /// </summary>
        /// <param name="file">The file to check.</param>

        private void analyzeFile(string file) {
            UltraID3 tags = new UltraID3();
            try {
                String infoLine;
                tags.Read(file);
                ID3FrameCollection covers = tags.ID3v2Tag.Frames.GetFrames(CommonMultipleInstanceID3v2FrameTypes.Picture);
                ID3v2PictureFrame cover = (ID3v2PictureFrame)covers[0];
                Bitmap image = new Bitmap((Image)cover.Picture);
                if (hasThreshold && !checkSize(image)) {
                    infoLine = "Checked Artwork size for file " + file + " is below limit: " + image.Size.Width + "x" + image.Size.Height;
                    Console.WriteLine(infoLine);
                }
                //Console.WriteLine("Artwork size is: " + image.Size.Width + "x" + image.Size.Height);

            }
            catch (Exception e) {
                Console.WriteLine("No cover found for: " + file);
                Console.WriteLine(e.Message);
            }
        }


        private void writeToLogFile(string line) {

            try {
                Console.WriteLine(line);
            }
            catch (Exception e) {
                Console.WriteLine("Could not create logfile: " + e.Message);
                Console.WriteLine("for path " + logfile);
                throw new ArgumentException();
            }
        }

        private StreamWriter initialiseLogging() {
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
    }


    /// <summary>
    /// Exposes the ArtReader, which supports the analysis of a file or directory with various options.
    /// </summary>

    public interface IArtReader {
        IArtReader toRead(string toRead);
        IArtReader withThreshold(string resolution);
        IArtReader withLogfile(string logfile);
        ArtReader create();
    }
}