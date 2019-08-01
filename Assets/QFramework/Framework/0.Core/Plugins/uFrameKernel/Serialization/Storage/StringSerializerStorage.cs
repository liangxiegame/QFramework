using System.Text;

public class StringSerializerStorage : ISerializerStorage
{
    public string Result { get; set; }

    public void Load(ISerializerStream stream)
    {
        stream.Load(Encoding.UTF8.GetBytes(Result));
    }

    public void Save(ISerializerStream stream)
    {
        var streamSave = stream.Save();
        Result = Encoding.UTF8.GetString(streamSave,0,streamSave.Length);
    }

    public override string ToString()
    {
        return Result;
    }
}