using UnityEngine;
using System.Collections.Generic;
using UI.Tables;

public class TableLayoutExampleController : MonoBehaviour 
{
    public List<TableLayout> Examples = new List<TableLayout>();

    public void ShowExample(TableLayout example)
    {
        Examples.ForEach(t => { if (t != example) t.gameObject.SetActive(false); });

        if (!example.gameObject.activeInHierarchy) example.gameObject.SetActive(true);
    }
}
