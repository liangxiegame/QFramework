using System.Collections.Generic;

public interface ISerializer
{
    IEnumerable<T> ReadArray<T>();

    void WriteArray<T>(T[] objs) where T: IUFSerializable;
    void WriteObject(IUFSerializable obj);


    object ReadObject<T>(ISerializerStream stream);

    void SerializeField<T>(string name, T obj);
    object ReadField(string name);
    T ReadField<T>(string name);
}