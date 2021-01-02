using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI.Tables;

namespace UI.Xml
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Dropdown))]
    public class XmlLayoutDropdown : MonoBehaviour
    {
        public Image Arrow;
        public Dropdown Dropdown;
        public Toggle ItemTemplate;
        public Scrollbar DropdownScrollbar;
    }
}
