using UnityEngine;
using System.Collections;
using System.IO;

public class read : MonoBehaviour
{
    //public TextAsset text;
    public GUIText guitext;
    void Start()
    {
        //guitext.text = text.text;
        //print(text.text);
        readCSV();
    }
    /// <summary>
    /// 读取CSV文件
    /// </summary>
    void readCSV()
    {
        //读取csv二进制文件
        TextAsset binAsset = Resources.Load("CSV/prop", typeof(TextAsset)) as TextAsset;
        //显示在GUITexture中
        guitext.text = binAsset.text;

        string[] data = binAsset.text.Split("|"[0]);
        foreach (var dat in data)
        {
            Debug.Log(dat);
        }

        ////读取每一行的内容
        string[] lineArray = binAsset.text.Split("\r"[0]);
        ////按‘|’进行拆分
        string[] lineArray1 = binAsset.text.Split("\t"[0]);

        //创建二维数组
        string[][] Array = new string[lineArray.Length][];

        //把csv中的数据储存在二位数组中
        for (int i = 0; i < lineArray.Length; i++)
        {
            Array[i] = lineArray[i].Split("\r"[0]);
            Debug.Log(Array[i][0].ToString());
        }
    }
}
