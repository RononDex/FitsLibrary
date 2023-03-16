
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests
{
    public class FitsDocumentHelperTests
    {
        [Test]
        public async Task ReadHeaderAsync_WithSampleFile_ReturnsHeader() {
            var header = await FitsDocumentHelper.ReadHeaderAsync("SampleFiles/FOCx38i0101t_c0f.fits");

            header.Should().NotBeNull();
        }

        [Test]
        public async Task GetDocumentContentType_WithSampleFile_ReturnsContentType() {
            var dataType = await FitsDocumentHelper.GetDocumentContentType("SampleFiles/FOCx38i0101t_c0f.fits");

            dataType.Should().Be(DataContentType.FLOAT);
        }
    }
}
