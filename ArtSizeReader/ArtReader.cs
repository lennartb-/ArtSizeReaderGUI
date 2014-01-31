using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ArtSizeReader {

    public class ArtReader : IArtReader {

        // Preserve standard Console output
        private static readonly StreamWriter DefaultConsoleOutput = new StreamWriter(Console.OpenStandardOutput());
        private StreamWriter logfileWriter = DefaultConsoleOutput;

        // Optional parameter bools
        private bool hasMaxThreshold = false;
        private bool hasRatio = false;
        private bool hasSizeLimit = false;
        private bool hasThreshold = false;
        private bool withLogfile = false;
        private bool withPlaylist = false;

        // Used for progress bar
        private int analyzedNumberOfFiles;
        private int numberOfFiles;

        // Required parameter values
        private double? size;
        private string targetPath;
        private string threshold;

        // Optional parameter values
        private string logfilePath;
        private string maxThreshold;
        private string playlistPath;

        // Input values converted to other datatypes:
        private Playlist playlist;
        private uint[] maxResolution;
        private uint[] resolution;

        private bool fromGui;
        

        /// <summary>
        /// Builds an ArtReader object from the specified parameters and checks if they are valid.
        /// </summary>
        /// <returns>An ArtReader objects with the desired input parameters.</returns>
        /// <exception cref="ArgumentException">Thrown when any of the supplied arguments are invalid.</exception>
        public ArtReader Create() {
            try {
                if (withLogfile) {
                    ValidateLogfile();
                }

                // Check if target path is valid.
                ValidateTargetPath();

                if (hasThreshold) {
                    ValidateResolution(this.threshold, false);
                    Console.WriteLine("Threshold enabled, selected value: " + resolution[0] + "x" + resolution[1]);
                }

                if (hasRatio) {
                    Console.WriteLine("Checking for 1:1 ratio is enabled.");
                }

                if (hasSizeLimit) {
                    Console.WriteLine("File size threshold enabled, reporting files above " + size + " kB");
                }

                if (hasMaxThreshold) {
                    ValidateResolution(this.maxThreshold, true);
                    Console.WriteLine("Maximum threshold enabled, selected value: " + maxResolution[0] + "x" + maxResolution[1]);
                }

                if (withPlaylist) {
                    ValidatePlaylist();
                }
            }
            catch (ArgumentException) {
                throw;
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
                
                var accessibleDirectories = SafeFileEnumerator.EnumerateDirectories(targetPath, "*.*", SearchOption.AllDirectories);
                IEnumerable<string> temp = new string[] { targetPath };
                accessibleDirectories = accessibleDirectories.Concat(temp);
                numberOfFiles = CountFiles(accessibleDirectories);

                foreach (string dir in accessibleDirectories) {
                    foreach (string file in ReadFiles(dir)) {
                        AnalyzeFile(file);
                    }
                }

                return true;
            }

            return false;
        }

        #region Interface allocation methods

        /// <summary>
        /// Specifies whether the object was created from GUI or CMD.
        /// </summary>
        /// <param name="fromGui">True if from GUI, false if from CMD.</param>
        /// <returns>The instance of the current object.</returns>
        public IArtReader FromGui(bool fromGui) {
            this.fromGui = fromGui;
            return this;
        }

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
            withLogfile = true;
            return this;
        }

        /// <summary>
        /// Specifies the art size maximum threshold in the format WIDTHxHEIGHT.
        /// </summary>
        /// <param name="maxThreshold">The maximum threshold.</param>
        /// <returns>The instance of the current object.</returns>
        public IArtReader WithMaxThreshold(string maxThreshold) {
            this.maxThreshold = maxThreshold;
            this.hasMaxThreshold = true;
            return this;
        }

        /// <summary>
        /// Specifies the filename and path of the playlist.
        /// </summary>
        /// <param name="playlist">The filename and path of the playlist.</param>
        /// <returns>The instance of the current object.</returns>
        public IArtReader WithPlaylist(string playlist) {
            this.playlistPath = playlist;
            withPlaylist = true;
            return this;
        }

        /// <summary>
        /// Specifies the art size threshold in the format WIDTHxHEIGHT.
        /// </summary>
        /// <param name="hasRatio">The threshold.</param>
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
        public IArtReader WithSize(double? size) {
            this.size = size;
            this.hasSizeLimit = true;
            return this;
        }

        /// <summary>
        /// Specifies the art size threshold in the format WIDTHxHEIGHT.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <returns>The instance of the current object.</returns>
        public IArtReader WithThreshold(string threshold) {
            this.threshold = threshold;
            hasThreshold = true;
            return this;
        }

        #endregion Interface allocation methods

        #region Private methods

        /// <summary>
        /// Analyzes a file for album art and handles checking of the size.
        /// </summary>
        /// <param name="file">The file to check.</param>
        private void AnalyzeFile(string file) {
            TagLib.File tag = TagLib.File.Create(file);
            
           
            string message = string.Empty;

            // Reader tags from file and get the content of the cover tag
            try {
////                tags.Read(file);
            }
            catch (Exception e) {
                Console.WriteLine("Unable to read file tags for: " + file+ ", ID3 tags might be corrupt.");
                Console.WriteLine("Exception: "+e.Message + "(" +e.GetType()+")");
                return;
            }            

            // Check if there actually is a cover.
            if (tag.Tag.Pictures.Length > 0) {               
                    
              MemoryStream ms = new MemoryStream(tag.Tag.Pictures[0].Data.Data);
               System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                

                if (hasSizeLimit) {
                    try {
                        double imagesize = GetImageSize(image);
                    
                        if (imagesize > this.size) {
                            message += "Artwork filesize is " + imagesize + " kB. ";
                        }
                    }
                    catch (Exception e) {
                        message += string.Format("Could not get image size from file {0}, Reason: {1} ({2})", file, e.Message, e.GetType().Name);                        
                    }
                }

                if (!IsWellFormedImage(image)) {
                    message += "Artwork image size is " + image.Size.Width + "x" + image.Size.Height;
                }
            }

            // No covers found.
            else {
                message += "No cover found.";
            }

            // If one of the checks failed, write it to console.
            if (!message.Equals(string.Empty)) {
                if (!withLogfile) Console.Write("\r");
                Console.WriteLine(file + ": " + message);
                if (withPlaylist) playlist.Write(file);
            }
        }

        private void HandleResults() {

        }

        /// <summary>
        /// Counts the number of individual MP3 files in an enumeration of directories.
        /// </summary>
        /// <param name="dirs">The enumerated directories.</param>
        /// <returns>The number of MP3 files in the directories.</returns>
        private int CountFiles(IEnumerable<string> dirs) {
            int num = 0;    
            foreach (string dir in dirs) {
                num += SafeFileEnumerator.EnumerateFiles(dir, "*.mp3", SearchOption.TopDirectoryOnly).Count();
            }

            return num;
        }

        /// <summary>
        /// Calculates the file size of an image.
        /// </summary>
        /// <param name="image">The image to check.</param>
        /// <returns>The file size in bytes.</returns>
        private double GetImageSize(Image image) {
            try {
                using (var ms = new MemoryStream()) {
                    image.Save(ms, image.RawFormat);
                    // Convert to kB.                    
                    return (double)(ms.Length >> 10);
                }
            }
            catch (Exception) {
                throw;
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
        /// Checks whether the size of an image is below the global threshold.
        /// </summary>
        /// <param name="image">The image to check.</param>
        /// <returns>false if the image is below the limit or has no 1:1 ratio, true if not.</returns>
        private bool IsWellFormedImage(Image image) {
            // Check if image is below (minimum) threshold.
            if (hasThreshold) {
                if (image.Size.Width < resolution[0] || image.Size.Height < resolution[1]) {
                    return false;
                }
            }

            // Check if image is above maximum threshold.
            if (hasMaxThreshold) {
                if (image.Size.Width > maxResolution[0] || image.Size.Height > maxResolution[1]) {
                    return false;
                }
            }

            // Check for 1:1 ratio.
            if (hasRatio) {
                if (image.Size.Width != image.Size.Height) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Enumerates the files in a certain directory and returns one file at a time.
        /// </summary>
        /// <param name="directory"> The directory to check.</param>
        /// <returns>An IEnumerable containing all found files.</returns>
        private IEnumerable<string> ReadFiles(string directory) {
            IEnumerable<string> musicFiles;

            // Get all files in the directory.            
            musicFiles = SafeFileEnumerator.EnumerateFiles(directory, "*.mp3",SearchOption.TopDirectoryOnly);

            foreach (string currentFile in musicFiles) {
                // If logging to file is enabled, print out the progress to console anyway.
                if (withLogfile) {
                    Console.SetOut(DefaultConsoleOutput);
                    Console.Write("\r{0} of {1} ({2}%) finished.{3}", ++analyzedNumberOfFiles, numberOfFiles, ((float)analyzedNumberOfFiles / (float)numberOfFiles) * 100, new string(' ', Console.LargestWindowWidth));
                    Console.SetOut(logfileWriter);
                }
                else {
                    /* Print out progress. Argument {3} ensures that any text right of the progress is cleared,
                     * otherwise old chars are not removed, since the number of decimal places of the percentage may vary.*/
                    Console.Write("\r{0} of {1} ({2}%) finished.{3}", ++analyzedNumberOfFiles, numberOfFiles, ((float)analyzedNumberOfFiles / (float)numberOfFiles) * 100, new string(' ', Console.LargestWindowWidth));
                }

                yield return currentFile;
            }
        }

        /// <summary>
        /// Checks the logfile path and starts the initialisation of the logfile.
        /// </summary>
        private void ValidateLogfile() {
            if (logfilePath != null) {
                if (InitialiseLogging()) {
                    Console.WriteLine("Logging enabled, writing log to: " + logfilePath);
                }
                else {
                    throw new ArgumentException("Invalid logfile path: " + logfilePath);
                }
            }
        }

        /// <summary>
        /// Manages the initialisation of the playlist.
        /// </summary>
        private void ValidatePlaylist() {
            if (playlistPath != null) {
                try {
                    string fullPlaylistPath = Path.GetFullPath(playlistPath);
                    bool validDir = Directory.Exists(Path.GetDirectoryName(fullPlaylistPath));
                    if (validDir) {
                        playlist = new Playlist(playlistPath);
                        Console.WriteLine("Playlist enabled, writing to " + fullPlaylistPath);
                    }
                    else throw new ArgumentException("Invalid playlist path: " + playlistPath);
                }
                catch (Exception e) {
                    Console.WriteLine("Could not create playlist: " + e.Message + "(" + e.GetType().Name + ")");
                    throw;
                }
            }
        }

        /// <summary>
        /// Checks if the resolution string is valid.
        /// </summary>
        /// <param name="threshold">The string with the resolution.</param>
        /// <param name="isMaxThreshold">True if the parameter is the maximum threshold string, false if it's the normal resolution.</param>
        private void ValidateResolution(string threshold, bool isMaxThreshold) {
            try {
                if (isMaxThreshold) {
                    maxResolution = threshold.Split('x').Select(uint.Parse).ToArray();
                }
                else {
                    resolution = threshold.Split('x').Select(uint.Parse).ToArray();
                }
            }
            catch (FormatException fe) {
                // Resolution is < 0 or doesn't fit into the uint Array
                throw new ArgumentException("Can not parse resolution " + threshold + ", must be in format e.g.: 300x300", fe);
            }
        }

        /// <summary>
        /// Checks if the target path is valid and exists.
        /// </summary>
        private void ValidateTargetPath() {
            if (Directory.Exists(targetPath)) {
                Console.WriteLine("Analyzing file(s) in " + targetPath);
            }
            else if (File.Exists(targetPath)) {
                Console.WriteLine("Analyzing file " + targetPath);
            }
            else {
                throw new ArgumentException("Invalid target path: " + targetPath);
            }
        }

        #endregion Private methods
    }
}