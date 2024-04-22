using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts.Objects;
using FitsLibrary.Serialization.Header;
using NUnit.Framework;

namespace FitsLibrary.Tests.Serialization.Header;

public class HeaderSerializationTests
{
    [Test]
    public async Task Serialize_WithEmptyHeader_Creates2880ByteLongBlockWithEndKeyword()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();

        await testee.SerializeAsync(new FitsLibrary.DocumentParts.Header(), PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..3], Is.EqualTo("END"));
    }

    [Test]
    public async Task Serialize_WithOneEntryWithCommentProperties_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", "Value1", "Comment1")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  = 'Value1' / Comment1".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithSingleQuoteInValue_EscapesSingleQuoteWithTwoSingleQuotes()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", "Value'1", "Comment1")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  = 'Value''1' / Comment1".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithVeryLongString_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", null)]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiu&'".PadRight(80)));
        Assert.That(actual[80..160], Is.EqualTo("CONTINUE  'smod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad&'".PadRight(80)));
        Assert.That(actual[160..240], Is.EqualTo("CONTINUE  ' minim veniam, quis nostrud exercitation ullamco laboris nisi ut al&'".PadRight(80)));
        Assert.That(actual[240..320], Is.EqualTo("CONTINUE  'iquip ex ea commodo consequat.'".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithVeryLongStringAndLongComment_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry(
                    "Entry1",
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiu&'".PadRight(80)));
        Assert.That(actual[80..160], Is.EqualTo("CONTINUE  'smod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad&'".PadRight(80)));
        Assert.That(actual[160..240], Is.EqualTo("CONTINUE  ' minim veniam, quis nostrud exercitation ullamco laboris nisi ut al&'".PadRight(80)));
        Assert.That(actual[240..320], Is.EqualTo("CONTINUE  'iquip ex ea commodo consequat.&' / Lorem ipsum dolor sit amet, consec".PadRight(80)));
        Assert.That(actual[320..400], Is.EqualTo("CONTINUE  '&' / tetur adipiscing elit, sed do eiusmod tempor incididunt ut labor".PadRight(80)));
        Assert.That(actual[400..480], Is.EqualTo("CONTINUE  '&' / e et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud ".PadRight(80)));
        Assert.That(actual[480..560], Is.EqualTo("CONTINUE  '&' / exercitation ullamco laboris nisi ut aliquip ex ea commodo conse".PadRight(80)));
        Assert.That(actual[560..640], Is.EqualTo("CONTINUE  '' / quat.".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithOneEntryWithoutValueButComment_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", null, "Comment1")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  Comment1".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithBooleanValueBeingTrue_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", true, "Comment1")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  = T / Comment1".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithBooleanValueBeingFalse_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", false, "Comment1")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  = F / Comment1".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithIntegerValue_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", 20, "Comment1")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  =                   20 / Comment1".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithNegativeIntegerValue_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", int.MinValue, "Comment1")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  =          -2147483648 / Comment1".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithLongValue_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", long.MaxValue, "Comment1")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  =  9223372036854775807 / Comment1".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithFloatValue_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", 1.03E10f, "Comment1")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  =             1.03E+10 / Comment1".PadRight(80)));
    }

    [Test]
    public async Task Serialize_WithDoubleValue_CreatesHeaderBlock()
    {
        var testee = new HeaderSerializer();
        using var memory = new MemoryStream();
        var header = new FitsLibrary.DocumentParts.Header([new HeaderEntry("Entry1", 1.03E10, "Comment1")]);

        await testee.SerializeAsync(header, PipeWriter.Create(memory)).ConfigureAwait(false);
        var actual = Encoding.ASCII.GetString(memory.ToArray());

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0..80], Is.EqualTo("Entry1  =          10300000000 / Comment1".PadRight(80)));
    }
}
