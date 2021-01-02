
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UI.Tables;

namespace UI.Xml.Tags
{
    public class TableLayoutTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<UI.Tables.TableLayout>();
            }
        }

        public override string prefabPath
        {
            get
            {
                return "Prefabs/TableLayout/TableLayout";
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributes)
        {            
            base.ApplyAttributes(attributes);

            var tableLayout = primaryComponent as TableLayout;

            if (attributes.ContainsKey("columncount"))
            {
                var columnCount = int.Parse(attributes["columncount"]);
                
                for (var x = 0; x < columnCount; x++)
                {
                    tableLayout.ColumnWidths.Add(0);
                }

                if (!attributes.ContainsKey("automaticallyRemoveEmptyColumns"))
                {
                    tableLayout.AutomaticallyRemoveEmptyColumns = false;
                }
            }
        }
    }
}

