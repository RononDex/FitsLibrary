namespace FitsLibrary.Serialization
{
    public abstract class BaseSerializer<T>
    {
        public abstract byte[] Serialize(T objToSerialize);
    }
}
