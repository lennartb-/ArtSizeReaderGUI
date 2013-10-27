using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArtSizeReader {
    /// <summary>
    /// Originally from <a href="http://stackoverflow.com/a/13954763/368354">StackOverflow</a>.
    /// Allows EnumerateDirectories() and EnumerateFiles() to skip inaccessible directories or files
    /// without ending the enumeration.
    /// </summary>
    public static class SafeFileEnumerator {
        /// <summary>
        /// Enumerates the subdirectories in a given directory. Skips inaccessible directories
        /// instead of failing and immediately returning.
        /// </summary>
        /// <param name="parentDirectory">The directory to search.</param>
        /// <param name="searchPattern">The search string to match against the names of directories
        /// in path.</param>
        /// <param name="searchOpt">One of the values of the SearchOption enumeration that specifies
        /// whether the search operation should include only the current directory or should include
        /// all subdirectories.</param>
        /// <returns>An enumerable collection of directory names in the directory specified by path
        /// and that match searchPattern and searchOption.</returns>
        public static IEnumerable<string> EnumerateDirectories(string parentDirectory, string searchPattern, SearchOption searchOpt) {
            try {
                var directories = Enumerable.Empty<string>();
                if (searchOpt == SearchOption.AllDirectories) {
                    directories = Directory.EnumerateDirectories(parentDirectory)
                        .SelectMany(x => EnumerateDirectories(x, searchPattern, searchOpt));
                }

                return directories.Concat(Directory.EnumerateDirectories(parentDirectory, searchPattern));
            }
            catch (UnauthorizedAccessException) {
                return Enumerable.Empty<string>();
            }
            catch (PathTooLongException) {
                Console.WriteLine("(Warning) Path too long: " + parentDirectory);
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Enumerates the files in a given directory. Skips inaccessible files
        /// instead of failing and immediately returning.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <param name="searchPattern">The search string to match against the names of directories
        /// in path.</param>
        /// <param name="searchOpt">One of the values of the SearchOption enumeration that specifies
        /// whether the search operation should include only the current directory or should include
        /// all subdirectories.</param>
        /// <returns>An enumerable collection of file names in the directory specified by path
        /// and that match searchPattern and searchOption.</returns>
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOpt) {
            try {
                var dirFiles = Enumerable.Empty<string>();
                if (searchOpt == SearchOption.AllDirectories) {
                    dirFiles = Directory.EnumerateDirectories(path)
                                        .SelectMany(x => EnumerateFiles(x, searchPattern, searchOpt));
                }

                return dirFiles.Concat(Directory.EnumerateFiles(path, searchPattern));
            }
            catch (UnauthorizedAccessException) {                
                return Enumerable.Empty<string>();
            }
            catch (PathTooLongException) {
                Console.WriteLine("(Warning) Filename and Path too long: " + path);
                return Enumerable.Empty<string>();
            }
        }
    }
}