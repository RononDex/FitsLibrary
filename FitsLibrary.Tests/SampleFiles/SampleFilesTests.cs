
using System.IO;
using NUnit.Framework;

namespace FitsLibrary.Tests.SampleFiles
{
    public class SampleFilesTests
    {
        [Test]
        public void OpenFitsFile_WithFOCFile_ReadsFile()
        {
            var testee = new FitsDocument(File.OpenRead("SampleFiles/FOCx38i0101t_c0f.fits"));
        }
    }
}
