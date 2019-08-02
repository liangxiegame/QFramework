public interface IUFSerializable
{
    string Identifier { get; }
    void Write(ISerializerStream stream);
    void Read(ISerializerStream stream);
}