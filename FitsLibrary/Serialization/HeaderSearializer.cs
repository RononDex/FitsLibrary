using FitsLibrary.DocumentParts;

namespace FitsLibrary.Serialization
{
    public abstract class HeaderSerializer : BaseSerializer<Header>
    {
        public override byte[] Serialize(Header objToSerialize)
        {
            throw new System.NotImplementedException();
        }
    }
}
