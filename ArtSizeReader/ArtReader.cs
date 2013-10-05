using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HundredMilesSoftware.UltraID3Lib;

namespace ArtSizeReader {

    public class ArtReader : IArtReader {

        // Preserve standard Console output
        private static StreamWriter defaultConsoleOutput = new StreamWriter(Console.OpenStandardOutput());

        private bool hasRatio = false;
        private bool hasSizeLimit = false;
        private bool isLoggingEnabled = false;
        private bool isPlaylistEnabled = false;
        private string logfilePath;
        private StreamWriter logfileWriter = defaultConsoleOutput;
        private Playlist playlist;
        private string playlistPath;
        private uint[] resolution;
        private string targetPath;
        private string threshold;
        private double size;

        /// <summary>
        /// Builds an ArtReader object from the specified parameters and checks if they are valid.
        /// </summary>
        /// <returns>An ArtReader objects with the desired input parameters.</returns>
        /// <exception cref="ArgumentException">Thrown when any of the supplied arguments are invalid.</exception>
        public ArtReader Create() {
            // Set up logfile.
            if (this.logfilePath != null) {
                if (InitialiseLogging()) {
                    Console.WriteLine("Logging enabled, writing log to: " + logfilePath);
                }
                else throw new ArgumentException("Invalid logfile path: " + logfilePath);
            }

            // Check if target path is valid.
            if (IsPathValid(targetPath)) {
                Console.WriteLine("Analyzing file(s) in " + targetPath);
            }
            else throw new ArgumentException("Invalid target path: " + targetPath);

            // Check and Parse resolution.
            if (this.threshold != null && ParseResolution()) {
                Console.WriteLine("Threshold enabled, selected value: " + resolution[0] + "x" + resolution[1]);
            }
            else throw new ArgumentException("Invalid resolution: " + threshold);

            if (hasRatio) {
                Console.WriteLine("Checking for proper ratio is enabled.");
            }

            if (this.size != 0.0) {
                hasSizeLimit = true;
                Console.WriteLine("File size threshold enabled, reporting files above " + size / 1000 + " MB");
            }

            // Set up playlist output.
            if (this.playlistPath != null) {
                if (InitialisePlaylist()) {
                    Console.WriteLine("Playlist output enabled, writing to: " + playlistPath);
                }
                else throw new ArgumentException("Invalid playlist path: " + playlistPath);
            }

            return this;
        }

        /// <summary>
        /// Starts fetching the album art from the specified file or directory.
        /// </summary>
        /// <returns>True if analysing succeeded, false if the file or path could not be found.</returns>
        public bool GetAlbumArt() {
            // Target is a single file
            if (File.Exists(targetPath)) {
                AnalyzeFile(targetPath);
                return true;
            }

            // Target is a directory
            else if (Directory.Exists(targetPath)) {
                // Search for files in the directory, but filter out inaccessible folders before.
                foreach (string dir in GetDirectories(targetPath))
                    foreach (string file in ReadFiles(dir)) {
                        AnalyzeFile(file);
                    }
                return true;
            }
            return false;
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
            this.logfilePath = logfile;
            return this;
        }

        /// <summary>
        /// Specifies the filename and path of the playlist.
        /// </summary>
        /// <param name="playlist">The filename and path of the playlist.</param>
        /// <returns>The instance of the current object.</returns>
        public IArtReader WithPlaylist(string playlist) {
            this.playlistPath = playlist;
            return this;
        }

        /// <summary>
        /// Specifies the art size threshold in the format WIDHTxHEIGHT.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <returns>The instance of the current object.</returns>
        public IArtReader WithRatio(bool hasRatio) {
            this.hasRatio = hasRatio;
            return this;
        }

        /// <summary>
        /// Specifies the file size threshold.
        /// </summary>
        /// <param name="size">The size in kilobytes.</param>
        /// <returns>The instance of the current object.</returns>
        public IArtReader WithSize(double size) {
            this.size = size;
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

        #endregion Interface allocation methods

        #region Private methods

        /// <summary>
        /// Analyzes a file for album art and handles checking of the size.
        /// </summary>
        /// <param name="file">The file to check.</param>
        private void AnalyzeFile(string file) {
            UltraID3 tags = new UltraID3();
            String message = String.Empty;
            // Reader tags from file and get the content of the cover tag
            tags.Read(file);
            ID3FrameCollection covers = tags.ID3v2Tag.Frames.GetFrames(CommonMultipleInstanceID3v2FrameTypes.Picture);

            // Check if there actually is a cover.
            if (covers.Count > 0) {
                ID3v2PictureFrame cover = (ID3v2PictureFrame)covers[0];

                if (hasSizeLimit) {
                    double imagesize = GetImageSize(cover.Picture);
                    if (imagesize > this.size / 1000) {
                        message += "Artwork filesize is " + imagesize + " MByte. ";
                    }
                }
                if (!IsWellFormedImage(cover.Picture, hasRatio)) {
                    message += "Artwork image size is " + cover.Picture.Size.Width + "x" + cover.Picture.Size.Height;
                }
            }

            // No covers found.
            else {
                message += "No cover found.";
            }

            // If one of the checks failed, write it to console.
            if (!message.Equals(String.Empty)) {
                if (!isLoggingEnabled) Console.Write("\r");
                Console.WriteLine(file + ": " + message);
                if (isPlaylistEnabled) playlist.Write(file);
            }
        }

        /// <summary>
        /// Gets all subdirectories of a given folder and filters out inaccessible directories.
        /// </summary>
        /// <param name="directory">The directory to analyse.</param>
        /// <returns>An enumerator that returns one subdirectory at a time.</returns>
        private IEnumerable<string> GetDirectories(string directory) {
            IEnumerable<string> subDirectories = null;
            try {
                subDirectories = Directory.EnumerateDirectories(directory, "*.*", SearchOption.TopDirectoryOnly);
            }
            catch (UnauthorizedAccessException) {
                /* Directory can't be accessed, most likely because it's a system directory.
                 * We can't do anything about it, so just ignore it and move on. */
            }

            if (subDirectories != null) {
                foreach (string subDirectory in subDirectories) {
                    yield return subDirectory;
                }
            }
        }

        /// <summary>
        /// Manages the initialisation of the logfile.
        /// </summary>
        /// <returns>true if the path is valid, false when not.</returns>
        private bool InitialiseLogging() {
            try {
                string fullLogfilePath = Path.GetFullPath(logfilePath);
                bool validDir = Directory.Exists(Path.GetDirectoryName(fullLogfilePath));
                if (validDir) {
                    FileStream fs = new FileStream(fullLogfilePath, FileMode.Append);
                    logfileWriter = new StreamWriter(fs);
                    logfileWriter.AutoFlush = true;
                    Console.SetOut(logfileWriter);
                    isLoggingEnabled = true;
                    return true;
                }
                else return false;
            }
            catch (Exception e) {
                Console.WriteLine("Could not create logfile: " + e.Message + "(" + e.GetType().Name + ")");
                return false;
            }
        }

        /// <summary>
        /// Manages the initialisation of the playlist.
        /// </summary>
        /// <returns>true if the path to the playlist is valid, false when not.</returns>
        private bool InitialisePlaylist() {
            try {
                string fullPlaylistPath = Path.GetFullPath(playlistPath);
                bool validDir = Directory.Exists(Path.GetDirectoryName(fullPlaylistPath));
                if (validDir) {
                    isPlaylistEnabled = true;
                    playlist = new Playlist(playlistPath);
                    return true;
                }
                else return false;
            }
            catch (Exception e) {
                Console.WriteLine("Could not create playlist: " + e.Message + "(" + e.GetType().Name + ")");
                return false;
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
        /// Checks whether the size of an image is below the global threshold.
        /// </summary>
        /// <param name="image">The image to check.</param>
        /// <param name="withRatio">Whether to additionally check if the image has a 1:1 aspect ratio.</param>
        /// <returns>false if the image is below the limit or has no 1:1 ratio, true if not.</returns>
        private bool IsWellFormedImage(Bitmap image, bool withRatio) {
            if (image.Size.Width < resolution[0] || image.Size.Height < resolution[1]) {
                return false;
            }
            if (withRatio) {
                if (image.Size.Width != image.Size.Height) {
                    return false;
                }
                else return true;
            }

            else return true;
        }

        /// <summary>
        /// Calculates the file size of an image.
        /// </summary>
        /// <param name="image">The image to check.</param>
        /// <returns>The file size in bytes.</returns>

        private double GetImageSize(Bitmap image) {
            using (var ms = new MemoryStream(image.Size.Width * image.Size.Height * 4)) { // 4 is probably way too much
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                return ((double)(ms.Length >> 10)) / 1000;
            }
        }


        /// <summary>
        /// Parses the resolution from a WIDTHxHEIGHT string into an array.
        /// </summary>
        /// <returns>true if the resolution is valid, false when not.</returns>
        private bool ParseResolution() {
            try {
                resolution = threshold.Split('x').Select(uint.Parse).ToArray();
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
                musicFiles = Directory.GetFiles(directory, "*.mp3", SearchOption.AllDirectories);
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
                if (isLoggingEnabled) {
                    Console.SetOut(defaultConsoleOutput);
                    Console.Write("\r{0} of {1} ({2}%) finished.{3}", ++i, numOfFiles, ((float)i / (float)numOfFiles) * 100, new string(' ', 10));
                    Console.SetOut(logfileWriter);
                }
                else {
                    /* Print out progress. Argument {3} ensures that any text right of the progress is cleared,
                     * otherwise old chars are not removed, since the number of decimal places of the percentage may vary.*/
                    Console.Write("\r{0} of {1} ({2}%) finished.{3}", ++i, numOfFiles, ((float)i / (float)numOfFiles) * 100, new string(' ', 10));
                }
                yield return currentFile;
            }
        }

        #endregion Private methods
    }
}