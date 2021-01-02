using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Xml
{
    public class XmlLayoutToggleGroup : ToggleGroup
    {
        public Sprite ToggleBackgroundImage;
        public Color ToggleBackgroundColor;

        public Sprite ToggleSelectedImage;
        public Color ToggleSelectedColor;

        protected List<Toggle> m_toggleElements = new List<Toggle>();
        protected List<Action<int>> m_EventHandlers = new List<Action<int>>();
        protected List<Action<string>> m_TextEventHandlers = new List<Action<string>>();

        protected int m_previousValue = -1;

        private bool isHandlingSetSelectedValue = false;

// I'm not sure why but ToggleGroup.OnValidate seems to be implemented in some platforms and not in others...
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
#else
        protected void OnValidate()
        {            
#endif

            UpdateToggleElements();
        }

        public void UpdateToggleElements()
        {
            m_toggleElements.ForEach(t => UpdateToggleElement(t));
        }

        public void UpdateToggleElement(Toggle toggle)
        {
            var xmlLayoutToggleButton = toggle.GetComponent<XmlLayoutToggleButton>();

            if (xmlLayoutToggleButton != null)
            {
                // This is a Toggle Button Element
            }
            else
            {
                // This is a Toggle Element
                var background = toggle.targetGraphic.GetComponent<Image>();
                background.sprite = ToggleBackgroundImage;
                background.color = ToggleBackgroundColor;

                var checkmark = toggle.graphic.GetComponent<Image>();
                checkmark.sprite = ToggleSelectedImage;
                checkmark.color = ToggleSelectedColor;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            UpdateToggleElements();
        }

        public void AddToggle(Toggle toggle)
        {
            toggle.group = this;
            m_toggleElements.Add(toggle);
        }

        public void AddOnValueChangedEventHandler(Action handler)
        {
            m_EventHandlers.Add((e) => handler.Invoke());
        }

        public void AddOnValueChangedEventHandler(Action<int> handler)
        {
            m_EventHandlers.Add(handler);
        }

        public void AddOnValueChangedEventHandler(Action<string> handler)
        {
            m_TextEventHandlers.Add(handler);
        }

        void ValueChanged(int newValue)
        {
            if (m_EventHandlers.Any())
            {                
                m_EventHandlers.ForEach(e => e.Invoke(newValue));
            }

            if (m_TextEventHandlers.Any())
            {
                var toggleElement = m_toggleElements[newValue];
                var toggleButton = toggleElement.GetComponent<XmlLayoutToggleButton>();

                if (toggleButton != null)
                {
                    m_TextEventHandlers.ForEach(e => e.Invoke(toggleButton.TextComponent.text));
                }                                
            }
        }

        public int GetSelectedValue()
        {    
            for (var x = 0; x < m_toggleElements.Count; x++)
            {
                if (m_toggleElements[x].isOn) return x;
            }

            return -1;
        }

        public string GetSelectedTextValue()
        {
            for (var x = 0; x < m_toggleElements.Count; x++)
            {
                if (m_toggleElements[x].isOn) return GetTextValueForIndex(x);
            }

            return null;
        }

        internal string GetTextValueForIndex(int index)
        {
            var toggleButton = m_toggleElements[index].GetComponent<XmlLayoutToggleButton>();

            if (toggleButton != null)
            {
                return toggleButton.TextComponent.text;
            }

            return null;
        }
        
        public void SetSelectedValue(int newValue, bool fireEvent = true)
        {
            if (isHandlingSetSelectedValue) return;
            if (newValue == -1) return;

            isHandlingSetSelectedValue = true;
                        
            for (var x = 0; x < m_toggleElements.Count; x++)
            {
                var element = m_toggleElements[x];

                if (x == newValue)
                {
                    element.isOn = x == newValue;

                    if (element.isOn && x != m_previousValue)
                    {
                        if (fireEvent) ValueChanged(x);
                    }
                }
            }

            m_previousValue = newValue;
            isHandlingSetSelectedValue = false;
        }

        public void SetSelectedValue(string newValue, bool fireEvent = true)
        {            
            for (var x = 0; x < m_toggleElements.Count; x++)
            {
                if (GetTextValueForIndex(x) == newValue)
                {
                    SetSelectedValue(x, fireEvent);
                    return;
                }
            }
        }

        internal int GetValueForElement(Toggle element)
        {
            for (var x = 0; x < m_toggleElements.Count; x++)
            {
                if (m_toggleElements[x] == element) return x;
            }

            return -1;
        }
    }
}
