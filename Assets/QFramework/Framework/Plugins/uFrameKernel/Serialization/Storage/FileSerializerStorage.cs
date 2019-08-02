#if !UNITY_WEBPLAYER
using System.IO;
public class FileSerializerStorage : ISerializerStorage
{
    public string Filename { get; set; }

    public FileSerializerStorage(string filename)
    {
        Filename = filename;
    }

    public void Load(ISerializerStream stream)
    {
        stream.Load(File.ReadAllBytes(Filename));
    }

    public void Save(ISerializerStream stream)
    {
        File.WriteAllBytes(Filename, stream.Save());
    }
}
#endif