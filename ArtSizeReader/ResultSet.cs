using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtSizeReader {
    public class ResultSet {

        public enum Type {Resolution, FileSize, Error};

        public string Filename { get; private set; }
        public Dictionary<Type, string> Results { get; private set; }
        

        public void AddFileSize(string size) {
            Results.Add(Type.FileSize, size);
        }

        public void AddResolution(string resolution) {
            Results.Add(Type.Resolution, resolution);
        }

        public void AddError(string error) {
            Results.Add(Type.Error, error);
        }

        public ResultSet(string filename) {
            this.Filename = filename;
            Results = new Dictionary<Type, string>();
        }
    }
}
