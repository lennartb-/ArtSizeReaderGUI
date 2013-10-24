using System;
using System.IO;
using System.Text;

namespace ArtSizeReader {
    /// <summary>
    /// Represents a playlist object.
    /// </summary>
    internal class Playlist {
        private string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="Playlist" /> class.
        /// </summary>
        /// <param name="path">The path to write the playlist to (including filename and extension).</param>
        public Playlist(string path) {
            this.path = path;
            try {
                // Try to create a file there.
                File.AppendAllText(path, String.Empty, Encoding.UTF8);
            }
            catch {
                // Bad luck. We handle it in the calling class.
                throw;
            }
        }

        /// <summary>
        /// Appends a line to the playlist.
        /// </summary>
        /// <param name="entry">The string to append, without a newline.</param>        
        public void Write(string entry) {
            File.AppendAllText(path, "\n" + entry, Encoding.UTF8);
        }
    }
}