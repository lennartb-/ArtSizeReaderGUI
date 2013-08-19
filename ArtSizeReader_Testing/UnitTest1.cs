using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace ArtSizeReader {
    [TestFixture]
    public class UnitTest1 {
        [Test]
        public void RunMain() {
            string inputFile= @"-i I:\Music";
            string logfile = "-l logfile.txt";
            Program.Main(new string[] { logfile, inputFile });
            ArtReader ar = new ArtReader();
            ar.ToRead(inputFile);
            ar.WithLogfile(logfile);
            ar.Create();            
        }
    }
}
