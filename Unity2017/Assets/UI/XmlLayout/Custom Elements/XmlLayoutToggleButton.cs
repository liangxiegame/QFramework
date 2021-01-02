using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI.Tables;

namespace UI.Xml
{
    [RequireComponent(typeof(Toggle))]
    public class XmlLayoutToggleButton : XmlLayoutButton
    {
        public Text TextComponent;

        public Color SelectedBackgroundColor;
        public Color SelectedTextColor;
        public Color SelectedIconColor;
        public Color DeselectedBackgroundColor;
        public Color DeselectedTextColor;
        public Color DeselectedIconColor;

        private Toggle m_Toggle;
        public Toggle Toggle
        {
            get
            {
                if (m_Toggle == null) m_Toggle = this.GetComponent<Toggle>();

                return m_Toggle;
            }
        }

        private Image m_Image;
        public Image Image
        {
            get
            {
                if (m_Image == null) m_Image = this.GetComponent<Image>();

                return m_Image;
            }
        }

        private EventSystem m_eventSystem;
        protected EventSystem eventSystem
        {
            get
            {
                if (m_eventSystem == null) m_eventSystem = GameObject.FindObjectOfType<EventSystem>();

                return m_eventSystem;
            }
        }

        void Start()
        {
            Toggle.onValueChanged.AddListener((e) =>
                {
                    ToggleValue(e);
                });

            ToggleValue(Toggle.isOn);
        }

        void OnValidate()
        {
            ToggleValue(Toggle.isOn);
        }

        void ToggleValue(bool isOn)
        {            
            if(isOn) 
            {
                ToggleOn();
            } 
            else 
            {
                ToggleOff();
            }
            
            // deselect the button (otherwise it will remain in the highlighted state and not appear to gain the Selected/Deselected BackgroundColor)
            if(eventSystem != null && eventSystem.currentSelectedGameObject == this.gameObject) eventSystem.SetSelectedGameObject(null);
        }

        void ToggleOn()
        {            
            Toggle.colors = Toggle.colors.SetNormalColor(SelectedBackgroundColor);
            TextComponent.color = SelectedTextColor;
            IconComponent.color = SelectedIconColor;
            IconHoverColor = SelectedIconColor;
            IconColor = SelectedIconColor;
        }

        void ToggleOff()
        {            
            Toggle.colors = Toggle.colors.SetNormalColor(DeselectedBackgroundColor);
            TextComponent.color = DeselectedTextColor;
            IconComponent.color = DeselectedIconColor;
            IconHoverColor = DeselectedIconColor;
            IconColor = DeselectedIconColor;
        }
    }
}
