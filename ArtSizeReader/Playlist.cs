using System.IO;

namespace ArtSizeReader {
    class Playlist {

        private string path;
        private StreamWriter logger;

        public Playlist(string path) {
            this.path = path;
            FileStream fs = new FileStream(path, FileMode.Append);
            logger = new StreamWriter(fs);
            logger.AutoFlush = true;
        }

        public bool Write(string entry) {
            logger.WriteLine(entry);
            return true;
        }

        public bool WriteBatch(string[] entries) {
            foreach (string entry in entries) {
                logger.WriteLine(entry);
            }
            return true;
        }
    }
}
