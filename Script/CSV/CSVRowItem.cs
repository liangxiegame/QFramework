using UnityEngine;
using System.Collections;

/// <summary>
/// 一个项目的属性
/// </summary>
public class CSVRowItem {
	public string Name { get; set;}
	public int Index { get; set; }

	public string[] Values;

	public CSVRowItem(string[] colsText)
	{
		Values = new string[colsText.Length];

		for (int i = 0 ; i  < Values.Length;i++)
		{
			Values [i] = colsText [i];
		}

		Index = int.Parse(Values [0]);
		Name =  Values [1];
		QPrint.Warn ("index:" + Index + " name:" + Name);
	}
}