using System;
using System.Collections.Generic;
using LitJson;

[Config("CustomItem")]
public partial class CustomItem
{
    /// <summary> id注释 </summary>
	public int Id { get; set; }
/// <summary> 名字 </summary>
	public string Name { get; set; }
/// <summary> 测试列表 </summary>
	public List<int> TestList { get; set; }
/// <summary> 测试枚举 </summary>
	public List<ItemType> TestEnumList { get; set; }
/// <summary> 类型 </summary>
	public ItemType Type { get; set; }
/// <summary> 字典 </summary>
	public Dictionary<int, string> TestDic { get; set; }
/// <summary> 枚举字典 </summary>
	public Dictionary<ItemType, int> TestEnumDic { get; set; }
/// <summary> 布尔 </summary>
	public bool TestBool { get; set; }
/// <summary> 布尔 </summary>
	public bool TestBool2 { get; set; }


    public static Dictionary<int, CustomItem> Data { get; private set; }
	    
    private static void Load(string content)
    {
        if (Data != null) return;
        BeginInit();
        Data = new Dictionary<int, CustomItem>();
        var data = JsonMapper.ToObject(content);
        foreach (var jsonData in data.Keys)
        {
            var obj = data[jsonData].ToObject(typeof(CustomItem)) as CustomItem;
            Data[obj.ID] = obj;
        }
        EndInit();
    }

    public static CustomItem GetById(int id)
    {
        if (Data.TryGetValue(id, out var result))
            return result;
        return null;
    }

    static partial void BeginInit();

    static partial void EndInit();
}