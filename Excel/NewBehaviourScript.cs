using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Excel;
using System.Data;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour 
{
    public Text readData;
	void Start () 
	{		
		XLSX();
	}
	
	void XLSX()
	{
		FileStream stream = File.Open(Application.dataPath + "/QFramework/Excel/UserLevel.xlsx", FileMode.Open, FileAccess.Read);
		IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
	
		DataSet result = excelReader.AsDataSet();
	
		int columns = result.Tables[0].Columns.Count;
		int rows = result.Tables[0].Rows.Count;
		
		
		for(int i = 0;  i< rows; i++)
		{
            for (int j = 0; j < columns; j++)
            {
                string nvalue = result.Tables[0].Rows[i][j].ToString();
                Debug.Log(nvalue);
                if (i > 0)
                {
                    readData.text += "\t\t" + nvalue;
                }
                else
                {
                    readData.text +="   \t" + nvalue;                    
                }
            }
            readData.text += "\n";
		}	
	}

}
