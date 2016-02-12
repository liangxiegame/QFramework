using UnityEngine;
using System.Collections;
//using SLua;

// 封装MonoBehaviour的生命周期及事件，分发到Lua
//[CustomLuaClass]
public class LuaBehaviour : MonoBehaviour 
{
//    public LuaTable table = null;
//    public string className = null;
//
//    void Awake()
//    {
//        // 这个接口将无法回调到Lua，因为在AddComponent<LuaBehaviour>()之后就立即调用了Awake()
//
//    }
//
//    void Start()
//    {
//        // 获取类名
//        LuaFunction toStrFunc = (LuaFunction)this.table["ClassName"];
//        this.className = toStrFunc.call(this.table).ToString();
//
//        LuaFunction func = (LuaFunction)this.table["Start"];
//        if (func != null)
//            func.call(this.table);
//    }
//
//    void Update()
//    {
//        LuaFunction func = (LuaFunction)this.table["Update"];
//        if (func != null)
//            func.call(this.table);
//    }
//
//    void OnDestroy()
//    {
//        LuaFunction func = (LuaFunction)this.table["OnDestroy"];
//        if (func != null)
//            func.call(this.table);
//    }
}
