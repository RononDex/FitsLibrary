using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.Deserialization;
using FluentAssertions;

namespace FitsLibrary.Tests.Desersialization
{
    public class ExtensionDeserializerTests
    {
        public async Task TestAsync()
        {
            var testee = new ExtensionDeserializer();
            var somePipeReader = new PipeReader();

            Action action = () => testee.DeserializeAsync(somePipeReader);
        }
    }
}
