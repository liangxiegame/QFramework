using System.Collections.Generic;
using QFramework;
using UnityEngine;

public interface ISerializerStream
{
    IQFrameworkContainer DependencyContainer { get; set; }

    void SerializeArray<T>(string name, IEnumerable<T> items);

    void SerializeObjectArray(string name, IEnumerable<object> items);

    //void SerializeObject<T>(string name, T value);
    void SerializeObject(string name, object value);

    void SerializeInt(string name, int value);

    void SerializeBool(string name, bool value);

    void SerializeString(string name, string value);

    void SerializeVector2(string name, Vector2 value);

    void SerializeVector3(string name, Vector3 value);

    void SerializeQuaternion(string name, Quaternion value);

    void SerializeDouble(string name, double value);

    void SerializeFloat(string name, float value);
    void SerializeColor(string name, Color value);

    //void SerializeEnum(string name, Enum value);
    void SerializeBytes(string name, byte[] bytes);

    IEnumerable<T> DeserializeObjectArray<T>(string name);

    T DeserializeObject<T>(string name);

    object DeserializeObject(string name);

    //T DeserializeViewModel<T>(string name) where T : ViewModel;

    //IEnumerable<T> DeserializeViewModelArray<T>(string name) where T : ViewModel;

    int DeserializeInt(string name);

    bool DeserializeBool(string name);

    string DeserializeString(string name);
    Color DeserializeColor(string color);

    Vector2 DeserializeVector2(string name);

    Vector3 DeserializeVector3(string name);

    Quaternion DeserializeQuaternion(string name);

    double DeserializeDouble(string name);

    float DeserializeFloat(string name);

    //Enum DeserializeEnum(string name);
    byte[] DeserializeBytes(string name);

    void Load(byte[] readAllBytes);

    byte[] Save();

    Dictionary<string, IUFSerializable> ReferenceObjects { get; set; }

    ITypeResolver TypeResolver { get; set; }

    bool DeepSerialize { get; set; }
}