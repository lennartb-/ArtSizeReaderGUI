using System.IO;

namespace ArtSizeReader {
    class Playlist {

        private string path;        

        public Playlist(string path) {
            this.path = path;
        }

        public bool Write(string entry) {            
            File.AppendAllText(path, "\n"+entry);
            return true;
        }

        public bool WriteBatch(string[] entries) {
            File.AppendAllLines(path, entries);
            return true;
        }
    }
}
