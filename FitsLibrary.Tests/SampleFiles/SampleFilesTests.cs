using System.Threading.Tasks;
using NUnit.Framework;

namespace FitsLibrary.Tests.SampleFiles
{
    public class SampleFilesTests
    {
        [Test]
        public async Task OpenFitsFile_WithFOCFile_ReadsFileAsync()
        {
            var reader = new FitsDocumentReader();
            var document = await reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits");
        }
    }
}
