using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.Objects;
using FitsLibrary.Serialization;
using NUnit.Framework;

namespace FitsLibrary.Tests.Serialization
{
    public class HeaderSerializationTests
    {
        [Test]
        public async Task Serialize_WithEmptyHeader_Creates2880ByteLongBlockWithEndKeyword()
        {
            var testee = new HeaderSerializer();
            using var memory = new MemoryStream();

            await testee.SerializeAsync(new Header(), PipeWriter.Create(memory)).ConfigureAwait(false);
            var actual = Encoding.ASCII.GetString(memory.ToArray());

            Assert.That(actual, Has.Length.EqualTo(2880));
            Assert.That(actual[0..3], Is.EqualTo("END"));
        }

        [Test]
        public async Task Serialize_WithOneEntryWithCommentProperties_CreatesHeaderBlock()
        {
            var testee = new HeaderSerializer();
            using var memory = new MemoryStream();
            var header = new Header([new HeaderEntry("Entry1", "Value1", "Comment1")]);

            await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
            var actual = Encoding.ASCII.GetString(memory.ToArray());

            Assert.That(actual, Has.Length.EqualTo(2880));
            Assert.That(actual[0..80], Is.EqualTo("Entry1  = Value1 / Comment1".PadRight(80)));
        }
    }
}
