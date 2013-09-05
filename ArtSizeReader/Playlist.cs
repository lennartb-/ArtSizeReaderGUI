using System;
using System.IO;
using System.Text;

namespace ArtSizeReader {

    internal class Playlist {
        private string path;

        public Playlist(string path) {
            this.path = path;
            try {
                File.AppendAllText(path, String.Empty, Encoding.UTF8);
            }
            catch {
                throw;
            }
        }

        public bool Write(string entry) {
            File.AppendAllText(path, "\n" + entry, Encoding.UTF8);
            return true;
        }

        public bool WriteBatch(string[] entries) {
            File.AppendAllLines(path, entries);
            return true;
        }
    }
}