
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UI.Tables;

namespace UI.Xml.Tags
{
    public class CellTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<UI.Tables.TableCell>();
            }
        }

        public override string prefabPath
        {
            get
            {
                return "Prefabs/TableLayout/Cell";
            }
        }
    }
}

