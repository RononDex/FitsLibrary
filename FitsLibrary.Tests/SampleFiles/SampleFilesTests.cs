using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FitsLibrary.Tests.SampleFiles
{
    public class SampleFilesTests
    {
        [Test]
        public async Task OpenFitsFile_WithFOCFile_ReadsFileAsync()
        {
            Console.WriteLine("Reading sample file");
            var startTime = DateTime.Now;

            var reader = new FitsDocumentReader();
            var document = await reader.ReadAsync("/home/cobra/test.fits");

            var endTime = DateTime.Now;
            Console.WriteLine($"Sample file read in {(endTime - startTime).TotalSeconds}s");
        }
    }
}
