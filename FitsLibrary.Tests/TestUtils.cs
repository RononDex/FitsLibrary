using System.IO;
using System.Text;

namespace FitsLibrary.Tests
{
    public static class TestUtils
    {
        public static Stream ByteArrayToStream(byte[] data)
        {
            return new MemoryStream(data);
        }

        public static Stream StringToStream(string data)
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            return ByteArrayToStream(bytes);
        }
    }
}
