
namespace ArtSizeReader {
    /// <summary>
    /// Exposes the fluent inteface for ArtReader, which supports the analysis of a file or directory with various options.
    /// </summary>
    public interface IArtReader {
        ArtReader Create();

        IArtReader ToRead(string toRead);

        IArtReader WithLogfile(string logfile);

        IArtReader WithThreshold(string resolution);

        IArtReader WithPlaylist(string playlist);

        IArtReader WithRatio(bool hasRatio);

        IArtReader WithSize(double? size);

        IArtReader WithMaxThreshold(string resolution);
    }
}
