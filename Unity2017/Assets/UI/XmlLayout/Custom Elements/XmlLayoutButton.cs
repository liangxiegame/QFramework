using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI.Tables;

namespace UI.Xml
{
    [ExecuteInEditMode]
    public class XmlLayoutButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Image IconComponent;
        public TableLayout ButtonTableLayout;
        public TableCell IconCell;
        public TableCell TextCell;

        public Color IconColor;
        public Color IconHoverColor;

        void Start()
        {
            if (IconColor == default(Color)) IconColor = Color.white;
            if (IconHoverColor == default(Color)) IconHoverColor = IconColor;

            IconComponent.color = IconColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IconComponent.color = IconHoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IconComponent.color = IconColor;
        }
    }
}
